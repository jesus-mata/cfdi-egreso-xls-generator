using System.Xml.Serialization;
using CFDI_Egreso_Extractor.Utils;
using JAMS.SAT.CFDI.Xml.TFD.v11;

namespace JAMS.SAT.CFDI.Xml.v40;

public partial class Comprobante
{
    [XmlAttributeAttribute("schemaLocation", Namespace = "http://www.w3.org/2001/XMLSchema-instance")]
    public string xsiSchemaLocation =
        "http://www.sat.gob.mx/cfd/4 http://www.sat.gob.mx/sitio_internet/cfd/4/cfdv40.xsd";

    [XmlIgnore] private TimbreFiscalDigital? _timbreFiscalDigital;

    [XmlIgnore]
    public TimbreFiscalDigital? TimbreFiscalDigital
    {
        get
        {
            if (_timbreFiscalDigital != null) return _timbreFiscalDigital;

            var complementos = Complemento?.Any;

            var complementoEl = complementos?.FirstOrDefault(c => c.LocalName == "TimbreFiscalDigital");
            if (complementoEl == null)
            {
                return null;
            }

            _timbreFiscalDigital = ComprobanteUtils.DeserializeFromXmlElement<TimbreFiscalDigital>(complementoEl);

            return _timbreFiscalDigital;
        }
        set
        {
            _timbreFiscalDigital = value;
            if (value == null)
            {
                return;
            }

            var element = ComprobanteUtils.SerializeToXmlElement(value);
            if (element != null)
            {
                element.Prefix = "tfd";
                Complemento.Any.Add(element);
            }
        }
    }

    [XmlIgnore] private Pagos.v20.Pagos? _pagos;

    [XmlIgnore]
    public Pagos.v20.Pagos? Pagos
    {
        get
        {
            if (_pagos != null) return _pagos;

            var complementos = Complemento?.Any;

            var complementoEl = complementos?.FirstOrDefault(c => c.LocalName == "Pagos");
            if (complementoEl == null)
            {
                return null;
            }

            _pagos = ComprobanteUtils.DeserializeFromXmlElement<Pagos.v20.Pagos>(complementoEl);

            return _pagos;
        }
        set
        {
            _pagos = value;
            if (value == null)
            {
                return;
            }

            var element = ComprobanteUtils.SerializeToXmlElement(value);
            if (element != null)
            {
                element.Prefix = "pago20";
                Complemento.Any.Add(element);
            }
        }
    }
}