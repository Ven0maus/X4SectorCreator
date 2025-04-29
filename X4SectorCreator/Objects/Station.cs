namespace X4SectorCreator.Objects
{
    public class Station : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public string Faction { get; set; }
        public string Owner { get; set; }
        public string Race { get; set; }
        public string CustomConstructionPlan { get; set; }
        public Point Position { get; set; }

        public object Clone()
        {
            return new Station
            {
                Id = Id,
                Name = Name,
                Type = Type,
                Faction = Faction,
                Owner = Owner,
                Race = Race,
                Position = Position,
                CustomConstructionPlan = CustomConstructionPlan
            };
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
