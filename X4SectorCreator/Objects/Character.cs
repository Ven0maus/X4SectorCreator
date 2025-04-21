using System.Xml;
using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "character")]
    public class Character
    {
        [XmlElement(ElementName = "category")]
        public Category CategoryObj { get; set; }

        [XmlElement(ElementName = "skills")]
        public Skills SkillsObj { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "macro")]
        public string Macro { get; set; }

        [XmlElement(ElementName = "owner")]
        public Owner OwnerObj { get; set; }

        [XmlAttribute(AttributeName = "group")]
        public string Group { get; set; }

        [XmlElement(ElementName = "stock")]
        public Stock StockObj { get; set; }

        [XmlElement(ElementName = "page")]
        public Page PageObj { get; set; }

        public Character Clone()
        {
            var xml = Serialize();
            return Deserialize(xml);
        }

        public string Serialize()
        {
            XmlSerializer serializer = new(typeof(Character));
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

        public static Character Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(Character));
            using StringReader stringReader = new(xml);
            return (Character)serializer.Deserialize(stringReader);
        }

        [XmlRoot(ElementName = "owner")]
        public class Owner
        {
            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }

            [XmlAttribute(AttributeName = "list")]
            public string List { get; set; }
        }

        [XmlRoot(ElementName = "stock")]
        public class Stock
        {
            [XmlAttribute(AttributeName = "ref")]
            public string Ref { get; set; }
        }

        [XmlRoot(ElementName = "page")]
        public class Page
        {
            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }
        }

        [XmlRoot(ElementName = "category")]
        public class Category
        {
            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }

            [XmlAttribute(AttributeName = "race")]
            public string Race { get; set; }

            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }

            [XmlAttribute(AttributeName = "comment")]
            public string Comment { get; set; }
        }

        [XmlRoot(ElementName = "skill")]
        public class Skill
        {
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }

            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }

            [XmlAttribute(AttributeName = "min")]
            public string Min { get; set; }

            [XmlAttribute(AttributeName = "max")]
            public string Max { get; set; }

            [XmlAttribute(AttributeName = "profile")]
            public string Profile { get; set; }
        }

        [XmlRoot(ElementName = "skills")]
        public class Skills
        {
            [XmlElement(ElementName = "skill")]
            public List<Skill> Skill { get; set; }
        }
    }

    [XmlRoot(ElementName = "characters")]
    public class Characters
    {
        [XmlElement(ElementName = "character")]
        public List<Character> Character { get; set; }

        [XmlAttribute(AttributeName = "xsi")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation")]
        public string NoNamespaceSchemaLocation { get; set; }

        public static Characters Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(Characters));
            using StringReader stringReader = new(xml);
            return (Characters)serializer.Deserialize(stringReader);
        }
    }
}
