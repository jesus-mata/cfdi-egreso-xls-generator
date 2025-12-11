// See https://aka.ms/new-console-template for more information

// /Users/jesus.mata/RiderProjects/CFDI-Egreso-Extractor/CFDI-Egreso-Extractor/xmls_prueba/

using System.Globalization;
using System.Xml.Serialization;
using CFDI_Egreso_Extractor;
using CFDI_Egreso_Extractor.Model;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using JAMS.SAT.CFDI.Xml.v40;

try
{
    var version = $"1.0";
    Console.WriteLine("Version: " + version);
    Console.WriteLine("Generando Excel...");

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


    List<ColumnHeader> columns2 = new List<ColumnHeader>
    {
        new ColumnHeader(1, "Estado SAT", "EstadoSAT"),
        new ColumnHeader(2, "Tipo", "Tipo"),
        new ColumnHeader(3, "Fecha Emision", "FechaEmision"),
        new ColumnHeader(4, "Fecha Timbrado", "FechaTimbrado"),
        new ColumnHeader(5, "Año", "Año"),
        new ColumnHeader(6, "Mes", "Mes"),
        new ColumnHeader(7, "Dia", "Dia"),
        new ColumnHeader(8, "Estado Pago", "EstadoPago"),
        new ColumnHeader(9, "Fecha Pago", "FechaPago"),
        new ColumnHeader(10, "Serie", "Serie"),
        new ColumnHeader(11, "Folio", "Folio"),
        new ColumnHeader(12, "UUID", "UUID"),
        new ColumnHeader(13, "UUID Relacion", "UUIDRelacion"),
        new ColumnHeader(14, "RFC Emisor", "RFCEmisor"),
        new ColumnHeader(15, "Nombre Emisor", "NombreEmisor"),
        new ColumnHeader(16, "Regimen Fiscal", "RegimenFiscal"),
        new ColumnHeader(17, "RFC Receptor", "RFCReceptor"),
        new ColumnHeader(18, "Nombre Receptor", "NombreReceptor"),
        new ColumnHeader(19, "Uso CFDI", "UsoCFDI"),
        new ColumnHeader(20, "SubTotal", "SubTotal"),
        new ColumnHeader(21, "IVA 16%", "IVA"),
        new ColumnHeader(21, "IVA 16% Tipo Factor", "IVATipoFactor"),
        new ColumnHeader(22, "Retenido IVA", "RetenidoIVA"),
        new ColumnHeader(23, "Retenido ISR", "RetenidoISR"),
        new ColumnHeader(24, "Total", "Total"),
        new ColumnHeader(25, "Complemento", "Complemento"),
        new ColumnHeader(26, "Moneda", "Moneda"),
        new ColumnHeader(27, "Tipo De Cambio", "TipoDeCambio"),
        new ColumnHeader(28, "Forma De Pago", "FormaDePago"),
        new ColumnHeader(29, "Metodo De Pago", "MetodoDePago"),
        new ColumnHeader(30, "NumCtaPago", "NumCtaPago"),
        new ColumnHeader(31, "Condicion De Pago", "CondicionDePago"),
        new ColumnHeader(32, "Conceptos", "Conceptos")
    };

    var columns = new List<string>()
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

// Create a spreadsheet document by supplying the filepath.
// By default, AutoSave = true, Editable = true, and Type = xlsx.
    using (SpreadsheetDocument spreadsheetDocument =
           SpreadsheetDocument.Create("./facturas.xlsx",
               SpreadsheetDocumentType.Workbook))
    {
        // Add a WorkbookPart to the document.
        WorkbookPart workbookPart = spreadsheetDocument.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();

        // Add a WorksheetPart to the WorkbookPart.
        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
        worksheetPart.Worksheet = new Worksheet(new SheetData());

        // Add Sheets to the Workbook.
        Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());

        // Append a new worksheet and associate it with the workbook.
        Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Facturas" };
        sheets.Append(sheet);

        int columnIdx = 1;
        foreach (var column in columns)
        {
            var col = Excels.ExcelColumnFromNumber(columnIdx);
            var cell = Excels.InsertCellInWorksheet(col.Trim(), 1, worksheetPart);

            cell.CellValue = new CellValue(column);
            cell.DataType = new EnumValue<CellValues>(CellValues.String);

            columnIdx++;
        }

        // spreadsheetDocument.Save();

        uint rowIdx = 2;
        foreach (var fac in facturas)
        {
            for (int colIdx = 0; colIdx < columns.Count; colIdx++)
            {
                var col = Excels.ExcelColumnFromNumber(colIdx + 1);
                var cell = Excels.InsertCellInWorksheet(col.Trim(), rowIdx, worksheetPart);
                var colName = columns[colIdx];
                var val = fac.GetColumnValue2(colName);
                switch (val.GetType())
                {
                    case { } t when t == typeof(double) || t == typeof(float) || t == typeof(int):
                        cell.CellValue = new CellValue(val.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                        break;
                    case { } t when t == typeof(decimal):
                        var dval = (decimal)val;
                        cell.CellValue =
                            new CellValue(Convert.ToString(dval, CultureInfo.CreateSpecificCulture("es-MX")));
                        // cell.CellValue = new CellValue(dval.ToString("C", CultureInfo.CreateSpecificCulture("es-MX")));
                        cell.DataType = new EnumValue<CellValues>(CellValues.Number);
                        break;
                    case { } t when t == typeof(bool):
                        cell.CellValue = new CellValue(val.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.Boolean);
                        break;
                    case { } t when t == typeof(string):
                        cell.CellValue = new CellValue(val.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        break;
                    // case { } t when t == typeof(DateTime):
                    //     cell.CellValue = new CellValue(((DateTime)val).ToString("yyyy-MM-dd HH:mm:ss"));
                    //     cell.DataType = new EnumValue<CellValues>(CellValues.Date);
                    //     break;
                    // case { } t when t == typeof(DateOnly):
                    //     cell.CellValue = new CellValue(((DateTime)val).ToString("yyyy-MM-dd"));
                    //     cell.DataType = new EnumValue<CellValues>(CellValues.Date);
                    //     break;
                    default:
                        cell.CellValue = new CellValue(val.ToString());
                        cell.DataType = new EnumValue<CellValues>(CellValues.String);
                        break;
                }
                // cell.CellValue = new CellValue(val.ToString());
                // cell.DataType = new EnumValue<CellValues>(CellValues.String);
            }

            rowIdx++;
        }

        spreadsheetDocument.Save();
    }

    Console.WriteLine("Excel Generado con exito!");
}
catch (Exception ex)
{
    Console.WriteLine(
        $"\ud83d\udea8 Error al generar el excel :{ex.Message}." +
        $"\n{ex.StackTrace}");
}