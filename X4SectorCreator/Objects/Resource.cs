namespace X4SectorCreator.Objects
{
    public class Resource
    {
        public string Ware { get; set; }
        public string Yield { get; set; }

        public override string ToString()
        {
            return $"[{Ware}|{Yield}]";
        }
    }
}
