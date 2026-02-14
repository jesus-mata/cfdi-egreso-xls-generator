// See https://aka.ms/new-console-template for more information

// /Users/jesus.mata/RiderProjects/CFDI-Egreso-Extractor/CFDI-Egreso-Extractor/xmls_prueba/

using System.Xml.Serialization;
using CFDI_Egreso_Extractor;
using CFDI_Egreso_Extractor.Model;
using JAMS.SAT.CFDI.Xml.v40;

var onlyDescarga = 1;
var requestId = "a3ffe53b-7b1f-44a5-9ed1-b3f9b4f43a85";

if (onlyDescarga == 1)
{
    var dm = new DescargaMasiva();
    await dm.Descargar();
    return;
} else if (onlyDescarga == 2)
{
    var dm = new DescargaMasiva();
    await dm.Authenticar();
    await dm.VerifyAndDownloadPackages(requestId);
    return;
}

try
{
    var version = $"1.0.{ThisAssembly.Git.Commit}";
    Console.WriteLine("Version: " + version);
    Console.WriteLine("Subiendo datos a Google Sheets...");

    var facturas = new List<Factura>();

    // First argument is the path to the xml files. if not provided, it will use the default path
    var xmlDir = args.Length == 1 ? args[0] : string.Empty;
    string path = string.IsNullOrEmpty(xmlDir) ? "./xmls/" : xmlDir;

    DirectoryInfo dir = new DirectoryInfo(path);
    if (!dir.Exists)
    {
        Console.WriteLine(
            $"Error: El directorio {dir.FullName} no existe. Crea el directorio, copia los xmls en el y vuelve a intentarlo.");
        return;
    }


    Console.WriteLine("Procesando archivos...");
    Console.WriteLine("File Name                       Size        Creation Date and Time");
    Console.WriteLine("=================================================================");
    foreach (FileInfo flInfo in dir.GetFiles())
    {
        try
        {
            String name = flInfo.Name;
            long size = flInfo.Length;
            DateTime creationTime = flInfo.CreationTime;
            Console.WriteLine("{0, -30:g} {1,-12:N0} {2} ", name, size, creationTime);

            var serializer = new XmlSerializer(typeof(Comprobante));
            using var reader =
                new StreamReader(flInfo.FullName);

            var comprobante = (Comprobante)serializer.Deserialize(reader);

            if (comprobante == null)
            {
                Console.WriteLine("Comprobante is null");
                return;
            }


            var factura = new Factura
            {
                FechaEmision = comprobante.Fecha,
                Serie = comprobante.Serie,
                Folio = comprobante.Folio,
                Tipo = comprobante.TipoDeComprobante,
                FechaTimbrado = comprobante.TimbreFiscalDigital?.FechaTimbrado,
                UUID = comprobante.TimbreFiscalDigital?.Uuid,
                RFCEmisor = comprobante.Emisor.Rfc,
                NombreEmisor = comprobante.Emisor.Nombre,
                RegimenFiscal = comprobante.Emisor.RegimenFiscal,
                RFCReceptor = comprobante.Receptor.Rfc,
                NombreReceptor = comprobante.Receptor.Nombre,
                UsoCFDI = comprobante.Receptor.UsoCfdi,
                SubTotal = comprobante.SubTotal,
                IVA = comprobante.Impuestos?.Traslados?.Where(x => x.Impuesto == "002").Sum(x => x.Importe),
                IVATipoFactor = comprobante.Impuestos?.Traslados?.FirstOrDefault(x => x.Impuesto == "002")?.TipoFactor,
                RetenidoIVA = comprobante.Impuestos?.Retenciones?.Where(x => x.Impuesto == "002").Sum(x => x.Importe),
                RetenidoISR = comprobante.Impuestos?.Retenciones?.Where(x => x.Impuesto == "001").Sum(x => x.Importe),
                Total = comprobante.Total,
                Moneda = comprobante.Moneda,
                TipoDeCambio = comprobante.TipoCambio ?? 1,
                FormaDePago = comprobante.FormaPago,
                MetodoDePago = comprobante.MetodoPago,
                // NumCtaPago = comprobante.NumCtaPago,
                CondicionDePago = comprobante.CondicionesDePago,
                Conceptos = string.Join(",", comprobante.Conceptos.Select(x => x.Descripcion))
            };

            facturas.Add(factura);
        }
        catch (Exception e)
        {
            throw new ProcessCfdiException(e, flInfo);
        }
    }

    var exporter = new GoogleSheetsExporter();
    exporter.Export(facturas);
}
catch (Exception ex)
{
    Console.WriteLine(
        $"\ud83d\udea8 Error al subir a Google Sheets: {ex.Message}." +
        $"\n{ex.StackTrace}");
}
