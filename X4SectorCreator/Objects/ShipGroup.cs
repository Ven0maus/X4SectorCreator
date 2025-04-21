using System.Xml;
using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "group")]
    public class ShipGroup
    {
        [XmlElement(ElementName = "select")]
        public List<Select> SelectObj { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }

        public string Serialize()
        {
            XmlSerializer serializer = new(typeof(ShipGroup));
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

        public static ShipGroup Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(ShipGroup));
            using StringReader stringReader = new(xml);
            return (ShipGroup)serializer.Deserialize(stringReader);
        }

        public override string ToString()
        {
            return Name;
        }

        internal ShipGroup Clone()
        {
            var xml = Serialize();
            return Deserialize(xml);
        }

        [XmlRoot(ElementName = "select")]
        public class Select
        {
            [XmlAttribute(AttributeName = "macro")]
            public string Macro { get; set; }

            [XmlAttribute(AttributeName = "weight")]
            public string Weight { get; set; }

            public override string ToString()
            {
                return Macro + " | " + Weight;
            }
        }
    }

    [XmlRoot(ElementName = "groups")]
    public class ShipGroups
    {
        [XmlElement(ElementName = "group")]
        public List<ShipGroup> Group { get; set; }

        public static ShipGroups Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(ShipGroups));
            using StringReader stringReader = new(xml);
            return (ShipGroups)serializer.Deserialize(stringReader);
        }
    }
}
