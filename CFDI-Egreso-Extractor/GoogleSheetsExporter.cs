using System.Globalization;
using CFDI_Egreso_Extractor.Model;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Google.Apis.Sheets.v4;
using Google.Apis.Sheets.v4.Data;

namespace CFDI_Egreso_Extractor;

public class GoogleSheetsExporter
{
    private const string SpreadsheetId = "19H9wvao5pguikjiMXHYdy5eeGLqRhdatzQ7Wx3R6Mbs";
    private const string SheetName = "Facturas";

    private static readonly string[] Columns =
    {
        "Estado SAT",
        "Tipo",
        "Fecha Emision",
        "Fecha Timbrado",
        "Año",
        "Mes",
        "Dia",
        "Estado Pago",
        "Fecha Pago",
        "Serie",
        "Folio",
        "UUID",
        "UUID Relacion",
        "RFC Emisor",
        "Nombre Emisor",
        "Regimen Fiscal",
        "RFC Receptor",
        "Nombre Receptor",
        "Uso CFDI",
        "SubTotal",
        "IVA 16%",
        "IVA 16% Tipo Factor",
        "Retenido IVA",
        "Retenido ISR",
        "Total",
        "Complemento",
        "Moneda",
        "Tipo De Cambio",
        "Forma De Pago",
        "Metodo de Pago",
        "NumCtaPago",
        "Condicion de Pago",
        "Conceptos"
    };

    private SheetsService CreateSheetsService()
    {
        const string serviceAccountFile = "service-account.json";

        // Try to find the service account JSON in several locations
        string? path = null;
        var candidates = new[]
        {
            Path.Combine(AppContext.BaseDirectory, serviceAccountFile),
            Path.Combine(Directory.GetCurrentDirectory(), serviceAccountFile),
            Path.Combine(Directory.GetCurrentDirectory(), "..", serviceAccountFile)
        };

        foreach (var candidate in candidates)
        {
            if (File.Exists(candidate))
            {
                path = candidate;
                break;
            }
        }

        if (path is null)
            throw new FileNotFoundException(
                $"No se encontró '{serviceAccountFile}' en ninguna de las siguientes ubicaciones:\n" +
                string.Join("\n", candidates.Select(c => $"  - {Path.GetFullPath(c)}")));

        using var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
        var credential = ServiceAccountCredential.FromServiceAccountData(stream);
        var scopedCredential = new ServiceAccountCredential(
            new ServiceAccountCredential.Initializer(credential.Id)
            {
                Scopes = new[] { SheetsService.Scope.Spreadsheets },
                Key = credential.Key
            });

        return new SheetsService(new BaseClientService.Initializer
        {
            HttpClientInitializer = scopedCredential,
            ApplicationName = "CFDI-Egreso-Extractor"
        });
    }

    public void Export(List<Factura> facturas)
    {
        var service = CreateSheetsService();
        var range = $"{SheetName}";

        // 1. Clear existing data
        var clearRequest = service.Spreadsheets.Values.Clear(
            new ClearValuesRequest(), SpreadsheetId, range);
        clearRequest.Execute();

        // 2. Build rows
        var rows = new List<IList<object>>();

        // Header row
        rows.Add(Columns.Cast<object>().ToList());

        // Data rows
        foreach (var factura in facturas)
        {
            var row = new List<object>();
            foreach (var column in Columns)
            {
                var value = factura.GetColumnValue2(column);
                row.Add(ConvertToSheetValue(value));
            }
            rows.Add(row);
        }

        // 3. Write all data in one batch
        var body = new ValueRange { Values = rows };
        var updateRequest = service.Spreadsheets.Values.Update(body, SpreadsheetId, $"{SheetName}!A1");
        updateRequest.ValueInputOption = SpreadsheetsResource.ValuesResource.UpdateRequest.ValueInputOptionEnum.USERENTERED;
        updateRequest.Execute();

        Console.WriteLine($"Datos subidos a Google Sheets ({facturas.Count} facturas).");
    }

    private static object ConvertToSheetValue(object value)
    {
        return value switch
        {
            null => string.Empty,
            decimal d => d.ToString(CultureInfo.InvariantCulture),
            double d => d.ToString(CultureInfo.InvariantCulture),
            float f => f.ToString(CultureInfo.InvariantCulture),
            DateTime dt => dt.ToString("yyyy-MM-dd HH:mm:ss"),
            _ => value.ToString() ?? string.Empty
        };
    }
}
