using System.Reflection;

namespace CFDI_Egreso_Extractor.Model;

using System;
using System.Collections.Generic;

public class Factura
{
    [ColumName("Estado SAT")]
    public string EstadoSAT { get; set; }
    [ColumName("Tipo")]
    public string Tipo { get; set; }
    [ColumName("Fecha Emision")]
    public DateTime FechaEmision { get; set; }
    [ColumName("Fecha Timbrado")]
    public DateTime? FechaTimbrado { get; set; }
    [ColumName("Año")]
    public int? Año { get; set; }
    [ColumName("Mes")]
    public int? Mes { get; set; }
    [ColumName("Dia")]
    public int? Dia { get; set; }
    [ColumName("Estado Pago")]
    public string EstadoPago { get; set; }
    [ColumName("Fecha Pago")]
    public DateTime? FechaPago { get; set; }
    [ColumName("Serie")]
    public string Serie { get; set; }
    [ColumName("Folio")]
    public string Folio { get; set; }
    [ColumName("UUID")]
    public string? UUID { get; set; }
    [ColumName("UUID Relacion")]
    public string UUIDRelacion { get; set; }
    [ColumName("RFC Emisor")]
    public string RFCEmisor { get; set; }
    [ColumName("Nombre Emisor")]
    public string NombreEmisor { get; set; }
    [ColumName("Regimen Fiscal")]
    public string RegimenFiscal { get; set; }
    [ColumName("RFC Receptor")]
    public string RFCReceptor { get; set; }
    [ColumName("Nombre Receptor")]
    public string NombreReceptor { get; set; }
    [ColumName("Uso CFDI")]
    public string UsoCFDI { get; set; }
    [ColumName("SubTotal")]
    public decimal SubTotal { get; set; }
    [ColumName("IVA 16%")]
    public decimal? IVA { get; set; }
    [ColumName("IVA 16% Tipo Factor")]
    public string? IVATipoFactor { get; set; }
    [ColumName("Retenido IVA")]
    public decimal? RetenidoIVA { get; set; }
    [ColumName("Retenido ISR")]
    public decimal? RetenidoISR { get; set; }
    [ColumName("Total")]
    public decimal Total { get; set; }
    [ColumName("Complemento")]
    public string Complemento { get; set; }
    [ColumName("Moneda")]
    public string Moneda { get; set; }
    [ColumName("Tipo De Cambio")]
    public decimal TipoDeCambio { get; set; }
    [ColumName("Forma De Pago")]
    public string FormaDePago { get; set; }
    [ColumName("Metodo de Pago")]
    public string MetodoDePago { get; set; }
    [ColumName("NumCtaPago")]
    public string NumCtaPago { get; set; }
    [ColumName("Condicion de Pago")]
    public string CondicionDePago { get; set; }
    [ColumName("Conceptos")]
    public string Conceptos { get; set; }

    public Factura()
    {
    }

    public void SetFechaEmision(DateTime fechaEmision)
    {
        FechaEmision = fechaEmision;
        Año = fechaEmision.Year;
        Mes = fechaEmision.Month;
        Dia = fechaEmision.Day;
    }
    
    public string GetColumnValue(string columnName)
    {
        var property = this.GetType().GetProperty(columnName);
        if (property != null)
        {
            return property.GetValue(this).ToString();
        }
        return string.Empty;
    }
    
    public object GetColumnValue2(string columnName)
    {
        var fields = this.GetType().GetProperties();
        var property = fields
            .FirstOrDefault(f => f.GetCustomAttribute<ColumNameAttribute>()?.Name == columnName);
            
        if (property != null)
        {
            var val = property.GetValue(this);
            return val ?? string.Empty;
        }
        throw new Exception("Column not found '" + columnName + "'");
    }
}