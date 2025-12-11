namespace CFDI_Egreso_Extractor.Model;

public class ColumnHeader
{
    public int Column { get; set; }
    public string Label { get; set; }
    public string Value { get; set; }
    
    public ColumnHeader(int column, string label, string value)
    {
        Column = column;
        Label = label;
        Value = value;
    }
}