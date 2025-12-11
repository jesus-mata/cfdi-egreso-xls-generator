namespace CFDI_Egreso_Extractor.Model;

public class ColumNameAttribute : Attribute
{
    
    public ColumNameAttribute(string name)
    {
        Name = name;
    }
    
    public string Name { get; set; }
    
}