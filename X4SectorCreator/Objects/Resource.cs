namespace X4SectorCreator.Objects
{
    public class Resource : ICloneable
    {
        public string Ware { get; set; }
        public string Yield { get; set; }
        public string Size { get; set; }
        public string Speed { get; set; }
        public int Amount { get; set; }
        public bool IsBaseGame { get; set; } = false;

        public object Clone()
        {
            return new Resource
            {
                Ware = Ware,
                Yield = Yield,
                Size = Size,
                Speed = Speed,
                Amount = Amount,
                IsBaseGame = IsBaseGame
            };
        }

        public override string ToString()
        {
            return $"[{Ware}|{Yield}|{Size}|{Speed}|{Amount}]";
        }
    }
}
