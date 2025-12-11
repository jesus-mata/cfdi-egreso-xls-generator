namespace CFDI_Egreso_Extractor;

public class ProcessCfdiException : Exception
{
    public ProcessCfdiException(Exception ex, FileInfo file) : base("Error al procesar el CFDI " + file.Name + $"\n{ex.Message}\n{ex.StackTrace}", ex)
    {
        
    }
}