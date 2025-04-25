using System.Xml;
using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "god")]
    public class Factories
    {
        [XmlElement(ElementName = "products")]
        public Factory.ProductObjs Products { get; set; }

        [XmlAttribute(AttributeName = "xsi")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation")]
        public string NoNamespaceSchemaLocation { get; set; }

        [XmlIgnore]
        public List<Factory> FactoryList => Products.Products;

        public string SerializeFactories()
        {
            // Create an XmlSerializer for the Factories type
            XmlSerializer serializer = new(typeof(Factories));

            // Create a StringWriter to hold the serialized XML string
            using StringWriter stringWriter = new();
            // Create XmlWriterSettings to format the XML with indentation
            XmlWriterSettings settings = new()
            {
                Indent = true
            };

            // Create an XmlWriter with the specified settings
            using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
            {
                // Serialize the Factories object to the XmlWriter
                serializer.Serialize(writer, this);
            }

            // Return the formatted XML string
            return stringWriter.ToString();
        }

        public static Factories DeserializeFactories(string xml)
        {
            // Create an XmlSerializer for the Jobs type
            XmlSerializer serializer = new(typeof(Factories));

            using StringReader stringReader = new(xml);
            // Deserialize the XML to a Jobs object
            return (Factories)serializer.Deserialize(stringReader);
        }
    }

    [XmlRoot(ElementName = "product")]
    public class Factory
    {
        [XmlElement(ElementName = "quotas")]
        public QuotasObj Quotas { get; set; }

        [XmlElement(ElementName = "location")]
        public LocationObj Location { get; set; }

        [XmlElement(ElementName = "position")]
        public PositionObj Position { get; set; }

        [XmlElement(ElementName = "module")]
        public ModuleObj Module { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "ware")]
        public string Ware { get; set; }

        [XmlAttribute(AttributeName = "owner")]
        public string Owner { get; set; }

        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "startactive")]
        public string Startactive { get; set; }

        public string SerializeFactory()
        {
            // Create an XmlSerializer for the Factory type
            XmlSerializer serializer = new(typeof(Factory));

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

                // Serialize the Factory object to the XmlWriter
                serializer.Serialize(writer, this, namespaces);
            }

            // Return the formatted XML string
            return stringWriter.ToString();
        }

        public static Factory DeserializeFactory(string xml)
        {
            // Create an XmlSerializer for the Jobs type
            XmlSerializer serializer = new(typeof(Factory));

            using StringReader stringReader = new(xml);
            // Deserialize the XML to a Jobs object
            return (Factory)serializer.Deserialize(stringReader);
        }

        public override string ToString()
        {
            return Id ?? GetHashCode().ToString();
        }

        [XmlRoot(ElementName = "quota")]
        public class Quota
        {
            [XmlAttribute(AttributeName = "galaxy")]
            public string Galaxy { get; set; }

            [XmlAttribute(AttributeName = "sector")]
            public string Sector { get; set; }

            [XmlAttribute(AttributeName = "gamestart")]
            public string Gamestart { get; set; }

            [XmlAttribute(AttributeName = "cluster")]
            public string Cluster { get; set; }

            [XmlAttribute(AttributeName = "zone")]
            public string Zone { get; set; }
        }

        [XmlRoot(ElementName = "quotas")]
        public class QuotasObj
        {
            [XmlElement(ElementName = "quota")]
            public Quota Quota { get; set; }
        }

        [XmlRoot(ElementName = "location")]
        public class LocationObj
        {
            [XmlAttribute(AttributeName = "class")]
            public string Class { get; set; }

            [XmlAttribute(AttributeName = "macro")]
            public string Macro { get; set; }

            [XmlAttribute(AttributeName = "solitary")]
            public string Solitary { get; set; }

            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }

            [XmlAttribute(AttributeName = "relation")]
            public string Relation { get; set; }

            [XmlAttribute(AttributeName = "comparison")]
            public string Comparison { get; set; }

            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }

            [XmlElement(ElementName = "economy")]
            public Economy Economy { get; set; }

            [XmlElement(ElementName = "region")]
            public Region Region { get; set; }

            [XmlElement(ElementName = "sunlight")]
            public Sunlight Sunlight { get; set; }

            [XmlAttribute(AttributeName = "excluderinghighway")]
            public string Excluderinghighway { get; set; }

            [XmlAttribute(AttributeName = "excludedtags")]
            public string Excludedtags { get; set; }

            [XmlElement(ElementName = "security")]
            public Security Security { get; set; }

            // By default always include this as false
            [XmlAttribute(AttributeName = "matchextension")]
            public string MatchExtension { get; set; } = "false";
        }

        [XmlRoot(ElementName = "position")]
        public class PositionObj
        {
            [XmlAttribute(AttributeName = "x")]
            public string X { get; set; }

            [XmlAttribute(AttributeName = "y")]
            public string Y { get; set; }

            [XmlAttribute(AttributeName = "z")]
            public string Z { get; set; }

            [XmlAttribute(AttributeName = "pitch")]
            public string Pitch { get; set; }

            [XmlAttribute(AttributeName = "roll")]
            public string Roll { get; set; }

            [XmlAttribute(AttributeName = "yaw")]
            public string Yaw { get; set; }
        }

        [XmlRoot(ElementName = "select")]
        public class Select
        {
            [XmlAttribute(AttributeName = "ware")]
            public string Ware { get; set; }

            [XmlAttribute(AttributeName = "race")]
            public string Race { get; set; }

            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }

            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }
        }

        [XmlRoot(ElementName = "module")]
        public class ModuleObj
        {
            [XmlElement(ElementName = "select")]
            public Select Select { get; set; }
        }

        [XmlRoot(ElementName = "economy")]
        public class Economy
        {
            [XmlAttribute(AttributeName = "max")]
            public string Max { get; set; }

            [XmlAttribute(AttributeName = "maxbound")]
            public string Maxbound { get; set; }
        }

        [XmlRoot(ElementName = "region")]
        public class Region
        {
            [XmlAttribute(AttributeName = "ware")]
            public string Ware { get; set; }

            [XmlAttribute(AttributeName = "max")]
            public string Max { get; set; }
        }

        [XmlRoot(ElementName = "sunlight")]
        public class Sunlight
        {
            [XmlAttribute(AttributeName = "min")]
            public string Min { get; set; }
        }

        [XmlRoot(ElementName = "security")]
        public class Security
        {
            [XmlAttribute(AttributeName = "min")]
            public string Min { get; set; }
        }

        [XmlRoot(ElementName = "products")]
        public class ProductObjs
        {
            [XmlElement(ElementName = "product")]
            public List<Factory> Products { get; set; }
        }

        [XmlRoot(ElementName = "level")]
        public class Level
        {
            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }
        }

        [XmlRoot(ElementName = "loadout")]
        public class Loadout
        {
            [XmlElement(ElementName = "level")]
            public Level Level { get; set; }
        }
    }
}