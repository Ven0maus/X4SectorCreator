using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "module")]
    public class Module
    {
        [XmlElement(ElementName = "category")]
        public Category CategoryObj { get; set; }

        [XmlElement(ElementName = "compatibilities")]
        public Compatibilities CompatibilitiesObj { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "group")]
        public string Group { get; set; }

        [XmlAttribute(AttributeName = "base")]
        public string Base { get; set; }

        [XmlRoot(ElementName = "extension")]
        public class Extension
        {
            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }

            [XmlAttribute(AttributeName = "race")]
            public string Race { get; set; }
        }

        [XmlRoot(ElementName = "category")]
        public class Category
        {
            [XmlElement(ElementName = "extension")]
            public List<Extension> Extension { get; set; }

            [XmlAttribute(AttributeName = "ware")]
            public string Ware { get; set; }

            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }

            [XmlAttribute(AttributeName = "race")]
            public string Race { get; set; }

            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }

            [XmlAttribute(AttributeName = "comment")]
            public string Comment { get; set; }
        }

        [XmlRoot(ElementName = "limits")]
        public class Limits
        {
            [XmlAttribute(AttributeName = "production")]
            public string Production { get; set; }
        }

        [XmlRoot(ElementName = "maxlimits")]
        public class Maxlimits
        {
            [XmlAttribute(AttributeName = "production")]
            public string Production { get; set; }
        }

        [XmlRoot(ElementName = "production")]
        public class Production
        {
            [XmlAttribute(AttributeName = "ware")]
            public string Ware { get; set; }

            [XmlAttribute(AttributeName = "chance")]
            public string Chance { get; set; }

            [XmlAttribute(AttributeName = "ref")]
            public string Ref { get; set; }

            [XmlAttribute(AttributeName = "respectquota")]
            public string Respectquota { get; set; }
        }

        [XmlRoot(ElementName = "compatibilities")]
        public class Compatibilities
        {
            [XmlElement(ElementName = "production")]
            public List<Production> Production { get; set; }

            [XmlElement(ElementName = "limits")]
            public Limits Limits { get; set; }

            [XmlElement(ElementName = "maxlimits")]
            public Maxlimits Maxlimits { get; set; }
        }
    }

    [XmlRoot(ElementName = "modules")]
    public class Modules
    {
        [XmlElement(ElementName = "module")]
        public List<Module> Module { get; set; }

        [XmlAttribute(AttributeName = "xsi")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation")]
        public string NoNamespaceSchemaLocation { get; set; }

        public static Modules Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(Modules));
            using StringReader stringReader = new(xml);
            return (Modules)serializer.Deserialize(stringReader);
        }
    }
}
