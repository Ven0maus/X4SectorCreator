using System.Xml.Serialization;

namespace X4SectorCreator.Objects
{
    [XmlRoot(ElementName = "baskets")]
    public class Baskets
    {
        [XmlElement(ElementName = "basket")]
        public Basket Basket { get; set; }
    }

    [XmlRoot(ElementName = "basket")]
    public class Basket
    {
        [XmlElement(ElementName = "wares")]
        public WareObjects Wares { get; set; }

        [XmlAttribute(AttributeName = "id")]
        public string Id { get; set; }

        [XmlRoot(ElementName = "wares")]
        public class WareObjects
        {
            [XmlElement(ElementName = "ware")]
            public WareObj Ware { get; set; }

            [XmlRoot(ElementName = "ware")]
            public class WareObj
            {
                [XmlAttribute(AttributeName = "ware")]
                public string Ware { get; set; }
            }
        }
    }
}