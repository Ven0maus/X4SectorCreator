using System.Xml;
using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "plan")]
    public class Constructionplan
    {
        [XmlElement(ElementName = "entry")]
        public List<Entry> EntryObj { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        internal Constructionplan Clone()
        {
            var xml = Serialize();
            return Deserialize(xml);
        }

        public string Serialize()
        {
            XmlSerializer serializer = new(typeof(Constructionplan));
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

        public static Constructionplan Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(Constructionplan));
            using StringReader stringReader = new(xml);
            return (Constructionplan)serializer.Deserialize(stringReader);
        }

        [XmlRoot(ElementName = "position")]
        public class Position
        {
            [XmlAttribute(AttributeName = "x")]
            public string X { get; set; }

            [XmlAttribute(AttributeName = "z")]
            public string Z { get; set; }

            [XmlAttribute(AttributeName = "y")]
            public string Y { get; set; }
        }

        [XmlRoot(ElementName = "offset")]
        public class Offset
        {
            [XmlElement(ElementName = "position")]
            public Position Position { get; set; }

            [XmlElement(ElementName = "rotation")]
            public Rotation Rotation { get; set; }
        }

        [XmlRoot(ElementName = "entry")]
        public class Entry
        {
            [XmlElement(ElementName = "offset")]
            public Offset Offset { get; set; }

            [XmlAttribute(AttributeName = "index")]
            public string Index { get; set; }

            [XmlAttribute(AttributeName = "macro")]
            public string Macro { get; set; }

            [XmlElement(ElementName = "predecessor")]
            public Predecessor Predecessor { get; set; }

            [XmlAttribute(AttributeName = "connection")]
            public string Connection { get; set; }
        }

        [XmlRoot(ElementName = "predecessor")]
        public class Predecessor
        {
            [XmlAttribute(AttributeName = "index")]
            public string Index { get; set; }

            [XmlAttribute(AttributeName = "connection")]
            public string Connection { get; set; }
        }

        [XmlRoot(ElementName = "rotation")]
        public class Rotation
        {
            [XmlAttribute(AttributeName = "yaw")]
            public string Yaw { get; set; }
        }
    }

    [XmlRoot(ElementName = "plans")]
    public class Constructionplans
    {
        [XmlElement(ElementName = "plan")]
        public List<Constructionplan> Plan { get; set; }

        public static Constructionplans Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(Constructionplans));
            using StringReader stringReader = new(xml);
            return (Constructionplans)serializer.Deserialize(stringReader);
        }
    }
}
