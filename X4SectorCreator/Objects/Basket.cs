using System.Xml;
using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "baskets")]
    public class Baskets
    {
        [XmlElement(ElementName = "basket")]
        public List<Basket> BasketList { get; set; }

        public static Baskets DeserializeBaskets(string xml)
        {
            // Create an XmlSerializer for the Jobs type
            XmlSerializer serializer = new(typeof(Baskets));

            using StringReader stringReader = new(xml);
            // Deserialize the XML to a Jobs object
            return (Baskets)serializer.Deserialize(stringReader);
        }
    }

    [XmlRoot(ElementName = "basket")]
    public class Basket
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlElement(ElementName = "wares")]
        public WareObjects Wares { get; set; }

        [XmlIgnore]
        public bool IsBaseGame { get; set; }

        [XmlRoot(ElementName = "wares")]
        public class WareObjects
        {
            [XmlElement(ElementName = "ware")]
            public List<WareObj> Wares { get; set; }

            [XmlRoot(ElementName = "ware")]
            public class WareObj
            {
                [XmlAttribute(AttributeName = "ware")]
                public string Ware { get; set; }
            }
        }

        public override string ToString()
        {
            return (Id ?? GetHashCode().ToString()).Replace("PREFIX_", "");
        }

        internal string SerializeBasket()
        {
            // Create an XmlSerializer for the Jobs type
            XmlSerializer serializer = new(typeof(Basket));

            // Create a StringWriter to hold the serialized XML string
            using StringWriter stringWriter = new();
            // Create XmlWriterSettings to format the XML with indentation
            XmlWriterSettings settings = new()
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            // Create an XmlWriter with the specified settings
            using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
            {
                // Create XmlSerializerNamespaces to avoid writing the default namespace
                XmlSerializerNamespaces namespaces = new();
                namespaces.Add("", "");  // Add an empty namespace

                // Serialize the Jobs object to the XmlWriter
                serializer.Serialize(writer, this, namespaces);
            }

            // Return the formatted XML string
            return stringWriter.ToString();
        }
    }
}