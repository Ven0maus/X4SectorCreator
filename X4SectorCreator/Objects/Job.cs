using System.Xml;
using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "jobs")]
    public class Jobs
    {
        [XmlElement(ElementName = "job")]
        public List<Job> JobList { get; set; }

        [XmlAttribute(AttributeName = "xsi")]
        public string Xsi { get; set; }

        [XmlAttribute(AttributeName = "noNamespaceSchemaLocation")]
        public string NoNamespaceSchemaLocation { get; set; }

        public string SerializeJobs()
        {
            // Create an XmlSerializer for the Jobs type
            XmlSerializer serializer = new(typeof(Jobs));

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
                // Serialize the Jobs object to the XmlWriter
                serializer.Serialize(writer, this);
            }

            // Return the formatted XML string
            return stringWriter.ToString();
        }

        public static Jobs DeserializeJobs(string xml)
        {
            // Create an XmlSerializer for the Jobs type
            XmlSerializer serializer = new(typeof(Jobs));

            using StringReader stringReader = new(xml);
            // Deserialize the XML to a Jobs object
            return (Jobs)serializer.Deserialize(stringReader);
        }
    }

    [XmlRoot(ElementName = "job")]
    public class Job
    {
        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlElement(ElementName = "modifiers")]
        public ModifierObjects Modifiers { get; set; }

        [XmlElement(ElementName = "orders")]
        public OrderObjects Orders { get; set; }

        [XmlElement(ElementName = "basket")]
        public BasketObj Basket { get; set; }

        [XmlElement(ElementName = "category")]
        public CategoryObject Category { get; set; }

        [XmlElement(ElementName = "quota")]
        public QuotaObject Quota { get; set; }

        [XmlElement(ElementName = "location")]
        public LocationObject Location { get; set; }

        [XmlElement(ElementName = "environment")]
        public EnvironmentObject Environment { get; set; }

        [XmlElement(ElementName = "ship")]
        public ShipObject Ship { get; set; }

        [XmlElement(ElementName = "subordinates")]
        public SubordinateObjects Subordinates { get; set; }

        [XmlAttribute(AttributeName = "comment")]
        public string Comment { get; set; }

        [XmlAttribute(AttributeName = "startactive")]
        public string Startactive { get; set; }

        [XmlAttribute(AttributeName = "disabled")]
        public string Disabled { get; set; }

        [XmlElement(ElementName = "encounters")]
        public EncounterObjects Encounters { get; set; }

        [XmlElement(ElementName = "time")]
        public TimeObject Time { get; set; }

        [XmlElement(ElementName = "task")]
        public Task Task { get; set; }

        [XmlElement(ElementName = "masstraffic")]
        public MasstrafficObject Masstraffic { get; set; }

        public string SerializeJob()
        {
            // Create an XmlSerializer for the Job type
            XmlSerializer serializer = new(typeof(Job));

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

                // Serialize the Job object to the XmlWriter
                serializer.Serialize(writer, this, namespaces);
            }

            // Return the formatted XML string
            return stringWriter.ToString();
        }

        public static Job DeserializeJob(string xml)
        {
            // Create an XmlSerializer for the Jobs type
            XmlSerializer serializer = new(typeof(Job));

            using StringReader stringReader = new(xml);
            // Deserialize the XML to a Jobs object
            return (Job)serializer.Deserialize(stringReader);
        }

        public override string ToString()
        {
            return Id ?? Name ?? GetHashCode().ToString();
        }

        internal Job Clone()
        {
            return DeserializeJob(SerializeJob());
        }

        [XmlRoot(ElementName = "modifiers")]
        public class ModifierObjects
        {
            [XmlAttribute(AttributeName = "rebuild")]
            public string Rebuild { get; set; }

            [XmlAttribute(AttributeName = "commandeerable")]
            public string Commandeerable { get; set; }

            [XmlAttribute(AttributeName = "subordinate")]
            public string Subordinate { get; set; }
        }

        [XmlRoot(ElementName = "param")]
        public class Param
        {
            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlAttribute(AttributeName = "value")]
            public string Value { get; set; }

            public override string ToString()
            {
                return $"{Name} | {Value}";
            }
        }

        [XmlRoot(ElementName = "order")]
        public class OrderObj
        {
            [XmlElement(ElementName = "param")]
            public List<Param> Param { get; set; }

            [XmlAttribute(AttributeName = "order")]
            public string Order { get; set; }

            [XmlAttribute(AttributeName = "default")]
            public string Default { get; set; }
        }

        [XmlRoot(ElementName = "orders")]
        public class OrderObjects
        {
            [XmlElement(ElementName = "order")]
            public OrderObj Order { get; set; }
        }

        [XmlRoot(ElementName = "basket")]
        public class BasketObj
        {
            [XmlAttribute(AttributeName = "basket")]
            public string Basket { get; set; }
        }

        [XmlRoot(ElementName = "category")]
        public class CategoryObject
        {
            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }

            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }

            [XmlAttribute(AttributeName = "size")]
            public string Size { get; set; }
        }

        [XmlRoot(ElementName = "quota")]
        public class QuotaObject
        {
            [XmlAttribute(AttributeName = "galaxy")]
            public string Galaxy { get; set; }

            [XmlAttribute(AttributeName = "cluster")]
            public string Cluster { get; set; }

            [XmlAttribute(AttributeName = "sector")]
            public string Sector { get; set; }

            [XmlAttribute(AttributeName = "maxgalaxy")]
            public string Maxgalaxy { get; set; }

            [XmlAttribute(AttributeName = "zone")]
            public string Zone { get; set; }

            [XmlAttribute(AttributeName = "wing")]
            public string Wing { get; set; }

            [XmlAttribute(AttributeName = "variation")]
            public string Variation { get; set; }

            [XmlAttribute(AttributeName = "station")]
            public string Station { get; set; }
        }

        [XmlRoot(ElementName = "location")]
        public class LocationObject
        {
            [XmlAttribute(AttributeName = "class")]
            public string Class { get; set; }

            [XmlAttribute(AttributeName = "macro")]
            public string Macro { get; set; }

            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }

            [XmlAttribute(AttributeName = "relation")]
            public string Relation { get; set; }

            [XmlAttribute(AttributeName = "comparison")]
            public string Comparison { get; set; }

            [XmlAttribute(AttributeName = "stationfaction")]
            public string Stationfaction { get; set; }

            [XmlAttribute(AttributeName = "stationtype")]
            public string Stationtype { get; set; }

            [XmlAttribute(AttributeName = "regionbasket")]
            public string Regionbasket { get; set; }

            [XmlAttribute(AttributeName = "policefaction")]
            public string Policefaction { get; set; }

            [XmlAttribute(AttributeName = "excludedtags")]
            public string Excludedtags { get; set; }

            [XmlElement(ElementName = "stationfactions")]
            public Stationfactions Stationfactions { get; set; }

            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }

            [XmlAttribute(AttributeName = "hasgravidarregion")]
            public string Hasgravidarregion { get; set; }

            [XmlAttribute(AttributeName = "factionrace")]
            public string Factionrace { get; set; }

            // By default always include this as false
            [XmlAttribute(AttributeName = "matchextension")]
            public string MatchExtension { get; set; } = "false";
        }

        [XmlRoot(ElementName = "environment")]
        public class EnvironmentObject
        {
            [XmlAttribute(AttributeName = "buildatshipyard")]
            public string Buildatshipyard { get; set; }

            [XmlAttribute(AttributeName = "preferbuilding")]
            public string Preferbuilding { get; set; }

            [XmlAttribute(AttributeName = "gate")]
            public string Gate { get; set; }
        }

        [XmlRoot(ElementName = "select")]
        public class Select
        {
            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }

            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }

            [XmlAttribute(AttributeName = "size")]
            public string Size { get; set; }
        }

        [XmlRoot(ElementName = "level")]
        public class Level
        {
            [XmlAttribute(AttributeName = "min")]
            public string Min { get; set; }

            [XmlAttribute(AttributeName = "max")]
            public string Max { get; set; }

            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }
        }

        [XmlRoot(ElementName = "loadout")]
        public class Loadout
        {
            [XmlElement(ElementName = "level")]
            public Level Level { get; set; }

            [XmlElement(ElementName = "variation")]
            public Variation Variation { get; set; }
        }

        [XmlRoot(ElementName = "owner")]
        public class Owner
        {
            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }

            [XmlAttribute(AttributeName = "overridenpc")]
            public string Overridenpc { get; set; }
        }

        [XmlRoot(ElementName = "ship")]
        public class ShipObject
        {
            [XmlElement(ElementName = "select")]
            public Select Select { get; set; }

            [XmlElement(ElementName = "loadout")]
            public Loadout Loadout { get; set; }

            [XmlElement(ElementName = "owner")]
            public Owner Owner { get; set; }

            [XmlElement(ElementName = "cargo")]
            public Cargo Cargo { get; set; }

            [XmlElement(ElementName = "units")]
            public Units Units { get; set; }

            [XmlElement(ElementName = "pilot")]
            public Pilot Pilot { get; set; }
        }

        [XmlRoot(ElementName = "fillpercent")]
        public class Fillpercent
        {
            [XmlAttribute(AttributeName = "min")]
            public string Min { get; set; }

            [XmlAttribute(AttributeName = "max")]
            public string Max { get; set; }

            [XmlAttribute(AttributeName = "profile")]
            public string Profile { get; set; }
        }

        [XmlRoot(ElementName = "wares")]
        public class Wares
        {
            [XmlElement(ElementName = "fillpercent")]
            public Fillpercent Fillpercent { get; set; }

            [XmlAttribute(AttributeName = "multiple")]
            public string Multiple { get; set; }
        }

        [XmlRoot(ElementName = "cargo")]
        public class Cargo
        {
            [XmlElement(ElementName = "wares")]
            public Wares Wares { get; set; }
        }

        [XmlRoot(ElementName = "variation")]
        public class Variation
        {
            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }
        }

        [XmlRoot(ElementName = "subordinate")]
        public class Subordinate
        {
            [XmlAttribute(AttributeName = "job")]
            public string Job { get; set; }

            [XmlAttribute(AttributeName = "assignment")]
            public string Assignment { get; set; }

            [XmlAttribute(AttributeName = "group")]
            public string Group { get; set; }
        }

        [XmlRoot(ElementName = "subordinates")]
        public class SubordinateObjects
        {
            [XmlElement(ElementName = "subordinate")]
            public List<Subordinate> Subordinate { get; set; }
        }

        [XmlRoot(ElementName = "unit")]
        public class Unit
        {
            [XmlAttribute(AttributeName = "category")]
            public string Category { get; set; }

            [XmlAttribute(AttributeName = "min")]
            public string Min { get; set; }

            [XmlAttribute(AttributeName = "max")]
            public string Max { get; set; }
        }

        [XmlRoot(ElementName = "units")]
        public class Units
        {
            [XmlElement(ElementName = "unit")]
            public Unit Unit { get; set; }
        }

        [XmlRoot(ElementName = "encounters")]
        public class EncounterObjects
        {
            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }
        }

        [XmlRoot(ElementName = "time")]
        public class TimeObject
        {
            [XmlAttribute(AttributeName = "interval")]
            public string Interval { get; set; }

            [XmlAttribute(AttributeName = "start")]
            public string Start { get; set; }
        }

        [XmlRoot(ElementName = "page")]
        public class Page
        {
            [XmlAttribute(AttributeName = "exact")]
            public string Exact { get; set; }

            [XmlAttribute(AttributeName = "comment")]
            public string Comment { get; set; }
        }

        [XmlRoot(ElementName = "pilot")]
        public class Pilot
        {
            [XmlElement(ElementName = "page")]
            public Page Page { get; set; }

            [XmlAttribute(AttributeName = "macro")]
            public string Macro { get; set; }
        }

        [XmlRoot(ElementName = "stationfaction")]
        public class StationFaction
        {
            [XmlAttribute(AttributeName = "stationfaction")]
            public string Stationfaction { get; set; }
        }

        [XmlRoot(ElementName = "stationfactions")]
        public class Stationfactions
        {
            [XmlElement(ElementName = "stationfaction")]
            public List<StationFaction> StationFaction { get; set; }
        }

        [XmlRoot(ElementName = "task")]
        public class TaskObj
        {
            [XmlAttribute(AttributeName = "task")]
            public string Task { get; set; }
        }

        [XmlRoot(ElementName = "masstraffic")]
        public class MasstrafficObject
        {
            [XmlAttribute(AttributeName = "ref")]
            public string Ref { get; set; }

            [XmlElement(ElementName = "owner")]
            public Owner Owner { get; set; }

            [XmlAttribute(AttributeName = "respawndelay")]
            public string Respawndelay { get; set; }

            [XmlAttribute(AttributeName = "relaunchdelay")]
            public string Relaunchdelay { get; set; }
        }
    }


}