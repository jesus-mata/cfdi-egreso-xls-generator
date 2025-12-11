using System.Xml;
using System.Xml.Serialization;
using JAMS.SAT.CFDI.Xml.Pagos.v20;
using JAMS.SAT.CFDI.Xml.TFD.v11;

namespace CFDI_Egreso_Extractor.Utils;

public class ComprobanteUtils
{
    public static XmlElement? SerializeToXmlElement(object obj)
    {
        XmlDocument doc = new XmlDocument();

        XmlSerializerNamespaces ns = GetNamespaces(obj);
        using (XmlWriter? writer = doc.CreateNavigator()?.AppendChild())
        {
            if (writer == null)
            {
                return null;
            }

            new XmlSerializer(obj.GetType()).Serialize(writer, obj, ns);
        }

        return doc.DocumentElement;
    }

    public static string SerializeToXmlString(object obj)
    {

        XmlSerializerNamespaces ns = GetNamespaces(obj);
        using (var writer = new StringWriter())
        {
            new XmlSerializer(obj.GetType()).Serialize(writer, obj, ns);
            return writer.ToString();
        }
        
    }
    
    private static XmlSerializerNamespaces GetNamespaces(object obj)
    {
        XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
        switch (obj)
        {
            case TimbreFiscalDigital _:
                ns.Add("tfd", "http://www.sat.gob.mx/TimbreFiscalDigital");
                break;
            case Pagos _:
                ns.Add("pago20", "http://www.sat.gob.mx/Pagos20");
                break;
            default:
                break;
        }
        
        ns.Add("xsi", "http://www.w3.org/2001/XMLSchema-instance");
        return ns;
    }

    public static T? DeserializeFromXmlElement<T>(XmlElement element)
    {
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(new XmlNodeReader(element));
    }
}