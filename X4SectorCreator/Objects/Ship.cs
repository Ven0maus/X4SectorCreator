using System.Xml;
using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "ship")]
    public class Ship
    {
        [XmlElement(ElementName = "category")]
        public Category CategoryObj { get; set; }

        [XmlElement(ElementName = "pilot")]
        public Pilot PilotObj { get; set; }

        [XmlElement(ElementName = "drop")]
        public Drop DropObj { get; set; }

        [XmlElement(ElementName = "people")]
        public People PeopleObj { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "group")]
        public string Group { get; set; }

        [XmlElement(ElementName = "basket")]
        public Basket BasketObj { get; set; }

        [XmlElement(ElementName = "owner")]
        public Owner OwnerObj { get; set; }

        public string Serialize()
        {
            XmlSerializer serializer = new(typeof(Ship));
            using StringWriter stringWriter = new();
            XmlWriterSettings settings = new()
            {
                Indent = true,
                OmitXmlDeclaration = true
            };

            using (XmlWriter writer = XmlWriter.Create(stringWriter, settings))
            {
                // Create XmlSerializerNamespaces to avoid writing the default namespace
                XmlSerializerNamespaces namespaces = new();
                namespaces.Add("", "");  // Add an empty namespace
                serializer.Serialize(writer, this, namespaces);
            }

            return stringWriter.ToString();
        }

        public static Ship Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(Ship));
            using StringReader stringReader = new(xml);
            return (Ship)serializer.Deserialize(stringReader);
        }

        [XmlRoot(ElementName = "basket")]
        public class Basket
        {
            [XmlAttribute(AttributeName = "basket")]
            public string BasketValue { get; set; }
        }

        [XmlRoot(ElementName = "owner")]
        public class Owner
        {
            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }

            [XmlAttribute(AttributeName = "overridenpc")]
            public string Overridenpc { get; set; }
        }

        [XmlRoot(ElementName = "category")]
        public class Category
        {
            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }

            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }

            [XmlAttribute(AttributeName = "size")]
            public string Size { get; set; }
        }

        [XmlRoot(ElementName = "select")]
        public class Select
        {
            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }

            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }
        }

        [XmlRoot(ElementName = "pilot")]
        public class Pilot
        {
            [XmlElement(ElementName = "select")]
            public Select Select { get; set; }
        }

        [XmlRoot(ElementName = "drop")]
        public class Drop
        {
            [XmlAttribute(AttributeName = "ref")]
            public string Ref { get; set; }
        }

        [XmlRoot(ElementName = "people")]
        public class People
        {
            [XmlAttribute(AttributeName = "ref")]
            public string Ref { get; set; }
        }

        public override string ToString()
        {
            return Id;
        }

        internal Ship Clone()
        {
            var xml = Serialize();
            return Deserialize(xml);
        }
    }

    [XmlRoot(ElementName = "ships")]
    public class Ships
    {
        [XmlElement(ElementName = "ship")]
        public List<Ship> Ship { get; set; }

        public static Ships Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(Ships));
            using StringReader stringReader = new(xml);
            return (Ships)serializer.Deserialize(stringReader);
        }
    }
}
