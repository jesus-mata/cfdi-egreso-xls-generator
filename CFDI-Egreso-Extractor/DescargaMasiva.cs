using Fiscalapi.XmlDownloader;
using Fiscalapi.XmlDownloader.Query.Models;

namespace CFDI_Egreso_Extractor;

public class DescargaMasiva
{
    private readonly XmlDownloaderService _service = new XmlDownloaderService();

    public async Task Authenticar()
    {
        try
        {
            var cerFile =
                "/Users/jesus.mata/Documents/Personal/FIEL_MASJ890824B94_20250222180009/FIEL_MASJ890824B94_20250222180009/00001000000714025625.cer"; // Replace with your .cer file path
            var keyFile =
                "/Users/jesus.mata/Documents/Personal/FIEL_MASJ890824B94_20250222180009/FIEL_MASJ890824B94_20250222180009/Claveprivada_FIEL_MASJ890824B94_20250222_180009.key"; // Replace with your .key file path

            var certBytes = File.ReadAllBytes(cerFile);

            var privBytes = File.ReadAllBytes(keyFile);


            var certBase64 = Convert.ToBase64String(certBytes);
            var keyBase64 = Convert.ToBase64String(privBytes);
            var password = "Sc0rp10n";


            // 1. Autenticación con FIEL
            Console.WriteLine("Autenticando...");
            await _service.AuthenticateAsync(certBase64, keyBase64, password);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error al authenticarse");
        }
    }

    public async Task Descargar()
    {
        


        try
        {
            // 1. Autenticación con FIEL
            Authenticar();

            // 2. Crear solicitud de descarga
            Console.WriteLine("Creando solicitud...");
            var queryParams = new QueryParameters()
            {
                StartDate = new DateTime(2026, 1, 1).ToStartOfDay(),
                EndDate = new DateTime(2026, 1, 30).ToEndOfDay(),
                // RecipientTin = "MASJ890824B94", // RFC del receptor
                IssuerTin = "MASJ890824B94",
                RequestType = QueryType.CFDI,
                InvoiceStatus = InvoiceStatus.Vigente,
            };

            var queryResponse = await _service.CreateRequestAsync(queryParams);

            if (!queryResponse.Succeeded)
            {
                Console.WriteLine($"Error creando solicitud: {queryResponse.SatMessage}");
                return;
            }

            Console.WriteLine($"----> Solicitud creada exitosamente: {queryResponse.RequestId}");

            Thread.Sleep(1000);
            var attempts = 12;
            while (attempts >= 0)
            {
                if (await VerifyAndDownloadPackages(queryResponse.RequestId)) return;


                Thread.Sleep(10000);
                attempts--;
            }

            Console.WriteLine("Proceso completado exitosamente");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error general: {ex.Message}\n${ex.StackTrace}");
        }
    }

    public async Task<bool> VerifyAndDownloadPackages(string requestId)
    {
        XmlDownloaderService service;
        // 3. Verificar estado de la solicitud
        Console.WriteLine($"Verificando estado de la solicitud con ID ${requestId}...");
        var verifyResponse = await _service.VerifyAsync(requestId);

        if (!verifyResponse.Succeeded)
        {
            Console.WriteLine($"Error verificando solicitud: {verifyResponse.SatMessage}");
            return true;
        }

        Console.WriteLine($"Estado SAT: {verifyResponse.SatStatus}");
        Console.WriteLine($"Estado Solicitud: {verifyResponse.RequestStatus}");
        Console.WriteLine($"Facturas encontradas: {verifyResponse.InvoiceCount}");

        // 4. Descargar paquetes si están listos
        if (verifyResponse.IsReadyToDownload)
        {
            Console.WriteLine($"Descargando {verifyResponse.PackageIds.Count} paquete(s)...");

            foreach (var packageId in verifyResponse.PackageIds)
            {
                Console.WriteLine($"Descargando paquete: {packageId}");
                var downloadResponse = await _service.DownloadAsync(packageId);

                if (downloadResponse.Succeeded)
                {
                    // Guardar paquete en disco
                    var packagePath = Path.Combine("/Users/jesus.mata/temp/packages", $"{packageId}.zip");
                    await _service.WritePackageAsync(packagePath, downloadResponse.PackageBytes);
                    Console.WriteLine($"Paquete guardado en: {packagePath}");

                    // Procesar comprobantes del paquete CFDI
                    Console.WriteLine("Procesando comprobantes...");
                    await foreach (var comprobante in _service.GetComprobantesAsync(
                                       downloadResponse.PackageBytes))
                    {
                        Console.WriteLine(
                            $"CFDI procesado - Serie: {comprobante.Serie}, Folio: {comprobante.Folio}");
                    }

                    // Procesar items del paquete Metadata
                    // await foreach (var item in service.GetMetadataAsync(downloadResponse.PackageBytes, CancellationToken.None))
                    // {
                    //   Console.WriteLine($"Procesando MetaItem Uuid:{item.InvoiceUuid} Amount: {item.Amount}   IsCancelled: {item.IsCancelled}");
                    // }
                    break;
                }
                else
                {
                    Console.WriteLine(
                        $"Error descargando paquete {packageId}: {downloadResponse.SatMessage}");
                }
            }
        }
        else
        {
            Console.WriteLine(
                $"La solicitud ${requestId} no está lista para descarga. Estado: {verifyResponse.RequestStatus}");
            Thread.Sleep(10000);
        }

        return false;
    }
}