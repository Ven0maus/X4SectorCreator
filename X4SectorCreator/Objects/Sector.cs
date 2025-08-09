using System.Text.Json;
using System.Text.Json.Serialization;

namespace X4SectorCreator.Objects
{
    public class Sector : ICloneable
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string BaseGameMapping { get; set; }
        public bool DisableFactionLogic { get; set; } = false;
        public string Owner { get; set; }
        public float Sunlight { get; set; } = 1.0f;
        public float Economy { get; set; } = 1.0f;
        public float Security { get; set; } = 1.0f;
        public int DiameterRadius { get; set; } = 500000;
        public bool AllowRandomAnomalies { get; set; } = true;
        public string Tags { get; set; }
        public List<Zone> Zones { get; set; } = [];
        public List<Region> Regions { get; set; } = [];
        public SectorPlacement Placement { get; set; }

        [JsonIgnore]
        public Point PlacementDirection => DeterminePlacementDirection();

        [JsonIgnore]
        public (long X, long Y) Offset { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
        public (long X, long Y) SectorRealOffset { get; set; }

        [JsonIgnore]
        public bool IsBaseGame => !string.IsNullOrWhiteSpace(BaseGameMapping);

        private Point DeterminePlacementDirection()
        {
            return Placement switch
            {
                SectorPlacement.TopLeft => new Point(-1, 1),
                SectorPlacement.TopRight => new Point(1, 1),
                SectorPlacement.BottomLeft => new Point(-1, -1),
                SectorPlacement.BottomRight => new Point(1, -1),
                SectorPlacement.MiddleLeft => new Point(-1, 0),
                SectorPlacement.MiddleRight => new Point(1, 0),
                SectorPlacement.MiddleTop => new Point(0, 1),
                SectorPlacement.MiddleBottom => new Point(0, -1),
                _ => throw new NotImplementedException($"\"{Placement}\" not implemented."),
            };
        }

        /// <summary>
        /// Called when a sector is creator or the diameter radius is updated
        /// </summary>
        public void InitializeOrUpdateZones()
        {
            List<(int x, int z)> zonePositions = new List<(int x, int z)>();

            // Define the side length of the large square (in km)
            long largeSquareSide = DiameterRadius / 2; // Divide by 2 to scale between 250-500km

            // Define the minimum and maximum side lengths for scaling zones
            const long minSide = 250000; // Min side length in km (250,000 km)
            const long maxSide = 500000; // Max side length in km (500,000 km)

            // Define the minimum and maximum number of zones
            const int minZones = 5;
            const int maxZones = 10;

            // Calculate the number of zones based on the large square's side using linear scaling
            double scaledZones = minZones + ((double)(largeSquareSide - minSide) / (maxSide - minSide)) * (maxZones - minZones);

            // Ensure that the number of zones is within the desired range (5-10)
            int desiredNumZones = (int)Math.Clamp(Math.Round(scaledZones), minZones, maxZones);

            // Calculate the optimal zone size based on the desired number of zones
            long optimalZoneSize = (long)Math.Ceiling(Math.Sqrt((double)(largeSquareSide * largeSquareSide) / desiredNumZones));

            // Now, we need to calculate the number of rows and columns
            int numZonesPerRow = (int)Math.Ceiling((double)largeSquareSide / optimalZoneSize);
            int numZonesPerColumn = desiredNumZones / numZonesPerRow;

            // Adjust the number of columns if the desired number of zones isn't an exact multiple of the rows
            if (desiredNumZones % numZonesPerRow != 0)
            {
                numZonesPerColumn += 1;
            }

            int rowOffset = (numZonesPerColumn - 1) / 2; // Offset to center the grid vertically
            int colOffset = (numZonesPerRow - 1) / 2;   // Offset to center the grid horizontally

            // Calculate and output the positions of each zone
            int zoneCount = 0;
            for (int row = 0; row < numZonesPerColumn; row++)
            {
                for (int col = 0; col < numZonesPerRow; col++)
                {
                    if (zoneCount >= desiredNumZones) break;

                    // Calculate the top-left corner (x, y) of each square using long for x and y
                    long x = (col - colOffset) * optimalZoneSize;
                    long y = (row - rowOffset) * optimalZoneSize;
                    zonePositions.Add(((int)x, (int)y));

                    // Increment the zone counter
                    zoneCount++;
                }
                if (zoneCount >= desiredNumZones) break;
            }

            // First remove all previously generated zones, if any exist
            Zones.RemoveAll(a => a.IsGeneratedZone);

            // Create new zones
            foreach (var (x, z) in zonePositions)
            {
                Zones.Add(new Zone
                {
                    Id = Zones.DefaultIfEmpty(new Zone()).Max(a => a.Id) + 1,
                    Position = new Point(x, z),
                    IsGeneratedZone = true
                });
            }
        }

        public object Clone()
        {
            return new Sector
            {
                Description = Description,
                AllowRandomAnomalies = AllowRandomAnomalies,
                BaseGameMapping = BaseGameMapping,
                DiameterRadius = DiameterRadius,
                DisableFactionLogic = DisableFactionLogic,
                Economy = Economy,
                Id = Id,
                Name = Name,
                Offset = Offset,
                Owner = Owner,
                Placement = Placement,
                Security = Security,
                Sunlight = Sunlight,
                Tags = Tags,
                Zones = Zones.Select(a => (Zone)a.Clone()).ToList(),
                Regions = Regions.Select(a => (Region)a.Clone()).ToList(),
            };
        }

        public override string ToString()
        {
            return Name ?? "Unknown";
        }
    }

    public enum SectorPlacement
    {
        TopLeft,
        TopRight,
        BottomLeft,
        BottomRight,
        MiddleLeft,
        MiddleRight,
        MiddleTop,
        MiddleBottom,
    }
}
