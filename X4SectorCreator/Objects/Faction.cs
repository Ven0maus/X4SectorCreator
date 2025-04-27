using System.Xml;
using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "faction")]
    public class Faction
    {
        [XmlElement(ElementName = "color")]
        public ColorDataObj ColorData { get; set; }

        [XmlElement(ElementName = "icon")]
        public IconObj IconData { get; set; }

        [XmlElement(ElementName = "licences")]
        public LicencesObj Licences { get; set; }

        [XmlElement(ElementName = "relations")]
        public RelationsObj Relations { get; set; }

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

        #region Faction Data
        [XmlIgnore]
        public Color Color { get; set; }

        [XmlIgnore]
        public string Icon { get; set; }

        [XmlIgnore]
        public string PrefferedHqSpace { get; set; }

        [XmlIgnore]
        public List<string> PrefferedHqStationTypes { get; set; }

        [XmlIgnore]
        public List<ShipGroup> ShipGroups { get; set; }

        [XmlIgnore]
        public List<Ship> Ships { get; set; }

        [XmlIgnore]
        public List<string> StationTypes { get; set; }

        [XmlIgnore]
        public string DesiredWharfs { get; set; }

        [XmlIgnore]
        public string DesiredShipyards { get; set; }

        [XmlIgnore]
        public string DesiredEquipmentDocks { get; set; }

        [XmlIgnore]
        public string DesiredTradeStations { get; set; }

        [XmlIgnore]
        public string AggressionLevel { get; set; }

        [XmlIgnore]
        public string AvariceLevel { get; set; }

        [XmlIgnore]
        public string Lawfulness { get; set; }
        #endregion

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

        internal Faction Clone()
        {
            return Deserialize(Serialize());
        }

        [XmlRoot(ElementName = "color")]
        public class ColorDataObj
        {
            [XmlAttribute(AttributeName = "ref")]
            public string Ref { get; set; }
        }

        [XmlRoot(ElementName = "icon")]
        public class IconObj
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
        public class LicencesObj
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
        public class RelationsObj
        {
            [XmlAttribute(AttributeName = "locked")]
            public string Locked { get; set; }

            [XmlElement(ElementName = "relation")]
            public List<Relation> Relation { get; set; }
        }
    }
}