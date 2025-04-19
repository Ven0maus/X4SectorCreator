using System.Xml;
using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "ware")]
    public class Ware
    {
        [XmlAttribute(AttributeName = "ware")]
        public string WareValue { get; set; }

        [XmlAttribute(AttributeName = "amount")]
        public string Amount { get; set; }

        [XmlElement(ElementName = "price")]
        public Price PriceObj { get; set; }

        [XmlElement(ElementName = "production")]
        public List<Production> ProductionObj { get; set; }

        [XmlElement(ElementName = "icon")]
        public Icon IconObj { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlAttribute(AttributeName = "name")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "description")]
        public string Description { get; set; }

        [XmlAttribute(AttributeName = "factoryname")]
        public string Factoryname { get; set; }

        [XmlAttribute(AttributeName = "group")]
        public string Group { get; set; }

        [XmlAttribute(AttributeName = "transport")]
        public string Transport { get; set; }

        [XmlAttribute(AttributeName = "volume")]
        public string Volume { get; set; }

        [XmlAttribute(AttributeName = "tags")]
        public string Tags { get; set; }

        [XmlElement(ElementName = "component")]
        public Component ComponentObj { get; set; }

        [XmlElement(ElementName = "sources")]
        public Sources SourcesObj { get; set; }

        [XmlElement(ElementName = "use")]
        public List<Use> UseObj { get; set; }

        [XmlElement(ElementName = "restriction")]
        public Restriction RestrictionObj { get; set; }

        [XmlElement(ElementName = "owner")]
        public List<Owner> OwnerObj { get; set; }

        [XmlElement(ElementName = "illegal")]
        public string IllegalObj { get; set; }

        [XmlAttribute(AttributeName = "licence")]
        public string Licence { get; set; }

        [XmlElement(ElementName = "container")]
        public Container ContainerObj { get; set; }

        [XmlElement(ElementName = "research")]
        public Research ResearchObj { get; set; }

        [XmlAttribute(AttributeName = "shortname")]
        public string Shortname { get; set; }

        [XmlAttribute(AttributeName = "sortorder")]
        public string Sortorder { get; set; }

        [XmlElement(ElementName = "software")]
        public Software SoftwareObj { get; set; }

        public string Serialize()
        {
            XmlSerializer serializer = new(typeof(Ware));
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

        public static Ware Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(Ware));
            using StringReader stringReader = new(xml);
            return (Ware)serializer.Deserialize(stringReader);
        }

        [XmlRoot(ElementName = "default")]
        public class Default
        {
            [XmlAttribute(AttributeName = "race")]
            public string Race { get; set; }
        }

        [XmlRoot(ElementName = "method")]
        public class Method
        {
            [XmlElement(ElementName = "default")]
            public Default Default { get; set; }

            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }

            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }
        }

        [XmlRoot(ElementName = "production")]
        public class Production
        {
            [XmlElement(ElementName = "method")]
            public List<Method> Method { get; set; }

            [XmlElement(ElementName = "effects")]
            public Effects Effects { get; set; }

            [XmlAttribute(AttributeName = "time")]
            public string Time { get; set; }

            [XmlAttribute(AttributeName = "amount")]
            public string Amount { get; set; }

            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlElement(ElementName = "primary")]
            public Primary Primary { get; set; }

            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }

            [XmlAttribute(AttributeName = "dismantlefactor")]
            public string Dismantlefactor { get; set; }

            [XmlElement(ElementName = "research")]
            public Research Research { get; set; }
        }

        [XmlRoot(ElementName = "price")]
        public class Price
        {
            [XmlAttribute(AttributeName = "min")]
            public string Min { get; set; }

            [XmlAttribute(AttributeName = "average")]
            public string Average { get; set; }

            [XmlAttribute(AttributeName = "max")]
            public string Max { get; set; }
        }

        [XmlRoot(ElementName = "effect")]
        public class Effect
        {
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }

            [XmlAttribute(AttributeName = "product")]
            public string Product { get; set; }
        }

        [XmlRoot(ElementName = "effects")]
        public class Effects
        {
            [XmlElement(ElementName = "effect")]
            public List<Effect> Effect { get; set; }
        }

        [XmlRoot(ElementName = "container")]
        public class Container
        {
            [XmlAttribute(AttributeName = "ref")]
            public string Ref { get; set; }
        }

        [XmlRoot(ElementName = "icon")]
        public class Icon
        {
            [XmlAttribute(AttributeName = "active")]
            public string Active { get; set; }

            [XmlAttribute(AttributeName = "video")]
            public string Video { get; set; }
        }

        [XmlRoot(ElementName = "defaults")]
        public class Defaults
        {
            [XmlElement(ElementName = "price")]
            public Price Price { get; set; }

            [XmlElement(ElementName = "production")]
            public Production Production { get; set; }

            [XmlElement(ElementName = "container")]
            public Container Container { get; set; }

            [XmlElement(ElementName = "icon")]
            public Icon Icon { get; set; }

            [XmlAttribute(AttributeName = "id")]
            public string Id { get; set; }

            [XmlAttribute(AttributeName = "name")]
            public string Name { get; set; }

            [XmlAttribute(AttributeName = "transport")]
            public string Transport { get; set; }

            [XmlAttribute(AttributeName = "volume")]
            public string Volume { get; set; }

            [XmlAttribute(AttributeName = "tags")]
            public string Tags { get; set; }
        }

        [XmlRoot(ElementName = "primary")]
        public class Primary
        {
            [XmlElement(ElementName = "ware")]
            public List<Ware> Ware { get; set; }
        }

        [XmlRoot(ElementName = "component")]
        public class Component
        {
            [XmlAttribute(AttributeName = "ref")]
            public string Ref { get; set; }
        }

        [XmlRoot(ElementName = "source")]
        public class Source
        {
            [XmlAttribute(AttributeName = "type")]
            public string Type { get; set; }

            [XmlAttribute(AttributeName = "location")]
            public string Location { get; set; }
        }

        [XmlRoot(ElementName = "sources")]
        public class Sources
        {
            [XmlElement(ElementName = "source")]
            public List<Source> Source { get; set; }
        }

        [XmlRoot(ElementName = "use")]
        public class Use
        {
            [XmlAttribute(AttributeName = "threshold")]
            public string Threshold { get; set; }

            [XmlAttribute(AttributeName = "factions")]
            public string Factions { get; set; }

            [XmlAttribute(AttributeName = "purposes")]
            public string Purposes { get; set; }
        }

        [XmlRoot(ElementName = "restriction")]
        public class Restriction
        {
            [XmlAttribute(AttributeName = "licence")]
            public string Licence { get; set; }

            [XmlAttribute(AttributeName = "buy")]
            public string Buy { get; set; }
        }

        [XmlRoot(ElementName = "owner")]
        public class Owner
        {
            [XmlAttribute(AttributeName = "faction")]
            public string Faction { get; set; }
        }

        [XmlRoot(ElementName = "illegal")]
        public class Illegal
        {
            [XmlAttribute(AttributeName = "factions")]
            public string Factions { get; set; }
        }

        [XmlRoot(ElementName = "research")]
        public class Research
        {
            [XmlElement(ElementName = "ware")]
            public Ware Ware { get; set; }

            [XmlElement(ElementName = "research")]
            public Research ResearchObj { get; set; }


            [XmlAttribute(AttributeName = "time")]
            public string Time { get; set; }

            [XmlElement(ElementName = "primary")]
            public Primary Primary { get; set; }
        }

        [XmlRoot(ElementName = "software")]
        public class Software
        {
            [XmlAttribute(AttributeName = "predecessor")]
            public string Predecessor { get; set; }
        }
    }

    [XmlRoot(ElementName = "wares")]
    public class Wares
    {
        [XmlElement(ElementName = "production")]
        public Ware.Production Production { get; set; }

        [XmlElement(ElementName = "defaults")]
        public Ware.Defaults Defaults { get; set; }

        [XmlElement(ElementName = "ware")]
        public List<Ware> Ware { get; set; }

        public static Wares Deserialize(string xml)
        {
            XmlSerializer serializer = new(typeof(Wares));
            using StringReader stringReader = new(xml);
            return (Wares)serializer.Deserialize(stringReader);
        }
    }
}
