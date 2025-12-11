using System.Xml.Serialization;

namespace CFDI_Egreso_Extractor;

public class XmlUtils
{
    public T? DeserilazeFromXml<T>(string filePath)
    {
        try
        {
            XmlSerializer serializer = new XmlSerializer(typeof(T));
            using StreamReader reader= new StreamReader(filePath);
            return (T)serializer.Deserialize(reader);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception occur during XML Deserialization :{ex.Message}");
        }
        return default(T);
    }
}