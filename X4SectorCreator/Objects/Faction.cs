using System.Xml;
using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "faction")]
    public class Faction
    {
        [XmlElement(ElementName = "color")]
        public ColorData ColorData { get; set; }

        [XmlElement(ElementName = "icon")]
        public Icon Icon { get; set; }

        [XmlElement(ElementName = "licences")]
        public Licences Licences { get; set; }

        [XmlElement(ElementName = "relations")]
        public Relations Relations { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }

        [XmlAttribute(AttributeName = "shortname")]
        public string Shortname { get; set; }

        [XmlAttribute(AttributeName = "prefixname")]
        public string Prefixname { get; set; }

        [XmlAttribute(AttributeName = "primaryrace")]
        public string Primaryrace { get; set; }

        [XmlAttribute(AttributeName = "behaviourset")]
        public string Behaviourset { get; set; }

        [XmlAttribute(AttributeName = "tags")]
        public string Tags { get; set; }

        [XmlAttribute(AttributeName = "policefaction")]
        public string PoliceFaction { get; set; }

        [XmlIgnore]
        public Color Color { get; set; }

        public string Serialize()
        {
            XmlSerializer serializer = new(typeof(Faction));
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

        public static Faction Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(Faction));
            using StringReader stringReader = new(xml);
            return (Faction)serializer.Deserialize(stringReader);
        }

        public override string ToString()
        {
            return Id;
        }
    }

    [XmlRoot(ElementName = "color")]
    public class ColorData
    {
        [XmlAttribute(AttributeName = "ref")]
        public string Ref { get; set; }
    }

    [XmlRoot(ElementName = "icon")]
    public class Icon
    {
        [XmlAttribute(AttributeName = "active")]
        public string Active { get; set; }

        [XmlAttribute(AttributeName = "inactive")]
        public string Inactive { get; set; }
    }

    [XmlRoot(ElementName = "licence")]
    public class Licence
    {
        [XmlAttribute(AttributeName = "type")]
        public string Type { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "icon")]
        public string Icon { get; set; }

        [XmlAttribute(AttributeName = "minrelation")]
        public string Minrelation { get; set; }

        [XmlAttribute(AttributeName = "precursor")]
        public string Precursor { get; set; }

        [XmlAttribute(AttributeName = "tags")]
        public string Tags { get; set; }

        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }

        [XmlAttribute(AttributeName = "price")]
        public string Price { get; set; }

        [XmlAttribute(AttributeName = "maxlegalscan")]
        public string Maxlegalscan { get; set; }
    }

    [XmlRoot(ElementName = "licences")]
    public class Licences
    {
        [XmlElement(ElementName = "licence")]
        public List<Licence> Licence { get; set; }
    }

    [XmlRoot(ElementName = "relation")]
    public class Relation
    {
        [XmlAttribute(AttributeName = "faction")]
        public string Faction { get; set; }

        [XmlAttribute(AttributeName = "relation")]
        public string RelationValue { get; set; }
    }

    [XmlRoot(ElementName = "relations")]
    public class Relations
    {
        [XmlElement(ElementName = "relation")]
        public List<Relation> Relation { get; set; }
    }
}
