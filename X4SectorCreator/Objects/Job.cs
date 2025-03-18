namespace X4SectorCreator.Objects
{
    public class Job
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public Modifiers Modifiers { get; set; }
        public List<Order> Orders { get; set; } = [];
        public Category Category { get; set; }
        public Quota Quota { get; set; }
        public Location Location { get; set; }
        public Ship Ship { get; set; }
        public string Basket { get; set; }
        public bool BuildAtShipyard { get; set; }
        public List<string> SubordinateJobIds { get; set; } = [];
    }

    public class Modifiers
    {
        public bool Rebuild { get; set; }
        public bool Commandeerable { get; set; }
        public bool Subordinate { get; set; }
    }

    public class Param
    {
        public string Name { get; set; }

        public int Value { get; set; }
    }

    public class Order
    {
        public List<Param> Param { get; set; } = [];
        public string Name { get; set; }
        public bool Default { get; set; }
    }

    public class Category
    {
        public string Faction { get; set; }
        public string Tags { get; set; }
        public string Size { get; set; }
    }

    public class Quota
    {
        public int Galaxy { get; set; }
        public int Cluster { get; set; }
        public int Sector { get; set; }
    }

    public class Location
    {
        public string Class { get; set; }
        public string Macro { get; set; }
        public string Faction { get; set; }
        public string Relation { get; set; }
        public string Comparison { get; set; }
        public bool MatchExtension { get; set; }
        public string Name { get; set; }
    }

    public class Ship
    {
        public Category Select { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }
        public double? Exact { get; set; }
        public string Owner { get; set; }
    }
}
