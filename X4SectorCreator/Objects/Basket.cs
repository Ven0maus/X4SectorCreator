using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "baskets")]
    public class Baskets
    {
        [XmlElement(ElementName = "basket")]
        public List<Basket> BasketList { get; set; }

        public static Baskets DeserializeBaskets(string xml)
        {
            // Create an XmlSerializer for the Jobs type
            XmlSerializer serializer = new(typeof(Baskets));

            using StringReader stringReader = new(xml);
            // Deserialize the XML to a Jobs object
            return (Baskets)serializer.Deserialize(stringReader);
        }
    }

    [XmlRoot(ElementName = "basket")]
    public class Basket
    {
        [XmlElement(ElementName = "wares")]
        public WareObjects Wares { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlIgnore]
        public bool IsBaseGame { get; set; }

        [XmlRoot(ElementName = "wares")]
        public class WareObjects
        {
            [XmlElement(ElementName = "ware")]
            public List<WareObj> Wares { get; set; }

            [XmlRoot(ElementName = "ware")]
            public class WareObj
            {
                [XmlAttribute(AttributeName = "ware")]
                public string Ware { get; set; }
            }
        }

        public override string ToString()
        {
            return Id ?? GetHashCode().ToString();
        }
    }
}