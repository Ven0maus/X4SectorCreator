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

        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }

        [XmlAttribute(AttributeName = "tags")]
        public string Tags { get; set; }

        [XmlElement(ElementName = "patches")]
        public Patches PatchesObj { get; set; }

        [XmlAttribute(AttributeName = "prefab")]
        public string Prefab { get; set; }

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

        public override string ToString()
        {
            return Id;
        }

        [XmlRoot(ElementName = "position")]
        public class Position
        {
            [XmlAttribute(AttributeName = "x")]
            public string X { get; set; }

            [XmlAttribute(AttributeName = "y")]
            public string Y { get; set; }

            [XmlAttribute(AttributeName = "z")]
            public string Z { get; set; }
        }

        [XmlRoot(ElementName = "rotation")]
        public class Rotation
        {
            [XmlAttribute(AttributeName = "yaw")]
            public string Yaw { get; set; }

            [XmlAttribute(AttributeName = "roll")]
            public string Roll { get; set; }

            [XmlAttribute(AttributeName = "pitch")]
            public string Pitch { get; set; }
        }

        [XmlRoot(ElementName = "offset")]
        public class Offset
        {
            [XmlElement(ElementName = "position")]
            public Position Position { get; set; }

            [XmlElement(ElementName = "rotation")]
            public Rotation Rotation { get; set; }

            [XmlElement(ElementName = "quaternion")]
            public Quaternion Quaternion { get; set; }
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

            [XmlAttribute(AttributeName = "fixed")]
            public string Fixed { get; set; }

            [XmlElement(ElementName = "upgrades")]
            public Upgrades Upgrades { get; set; }

            [XmlAttribute(AttributeName = "prefab")]
            public string Prefab { get; set; }
        }

        [XmlRoot(ElementName = "predecessor")]
        public class Predecessor
        {
            [XmlAttribute(AttributeName = "index")]
            public string Index { get; set; }

            [XmlAttribute(AttributeName = "connection")]
            public string Connection { get; set; }
        }

        [XmlRoot(ElementName = "shields")]
        public class Shields
        {
            [XmlAttribute(AttributeName = "macro")]
            public string Macro { get; set; }

            [XmlAttribute(AttributeName = "path")]
            public string Path { get; set; }

            [XmlAttribute(AttributeName = "group")]
            public string Group { get; set; }

            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }
        }

        [XmlRoot(ElementName = "turrets")]
        public class Turrets
        {
            [XmlAttribute(AttributeName = "macro")]
            public string Macro { get; set; }

            [XmlAttribute(AttributeName = "path")]
            public string Path { get; set; }

            [XmlAttribute(AttributeName = "group")]
            public string Group { get; set; }

            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }
        }

        [XmlRoot(ElementName = "groups")]
        public class Groups
        {
            [XmlElement(ElementName = "shields")]
            public List<Shields> Shields { get; set; }

            [XmlElement(ElementName = "turrets")]
            public List<Turrets> Turrets { get; set; }
        }

        [XmlRoot(ElementName = "upgrades")]
        public class Upgrades
        {
            [XmlElement(ElementName = "groups")]
            public Groups Groups { get; set; }

            [XmlElement(ElementName = "ammunition")]
            public Ammunition Ammunition { get; set; }

            [XmlAttribute(AttributeName = "generated")]
            public string Generated { get; set; }
        }

        [XmlRoot(ElementName = "quaternion")]
        public class Quaternion
        {
            [XmlAttribute(AttributeName = "qx")]
            public string Qx { get; set; }

            [XmlAttribute(AttributeName = "qw")]
            public string Qw { get; set; }

            [XmlAttribute(AttributeName = "qy")]
            public string Qy { get; set; }

            [XmlAttribute(AttributeName = "qz")]
            public string Qz { get; set; }
        }

        [XmlRoot(ElementName = "unit")]
        public class Unit
        {
            [XmlAttribute(AttributeName = "macro")]
            public string Macro { get; set; }

            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }
        }

        [XmlRoot(ElementName = "ammunition")]
        public class Ammunition
        {
            [XmlElement(ElementName = "unit")]
            public List<Unit> Unit { get; set; }

            [XmlElement(ElementName = "ammunition")]
            public Ammunition AmmunitionObj { get; set; }
        }

        [XmlRoot(ElementName = "patch")]
        public class Patch
        {
            [XmlAttribute(AttributeName = "extension")]
            public string Extension { get; set; }

            [XmlAttribute(AttributeName = "version")]
            public string Version { get; set; }

            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }
        }

        [XmlRoot(ElementName = "patches")]
        public class Patches
        {
            [XmlElement(ElementName = "patch")]
            public Patch Patch { get; set; }
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