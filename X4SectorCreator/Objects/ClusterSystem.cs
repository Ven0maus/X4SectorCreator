using System.Text.Json.Serialization;
using System.Xml.Linq;

namespace X4SectorCreator.Objects
{
    public class ClusterSystem
    {
        public string DatasetMacro { get; set; }
        public string SpaceEnvironment { get; set; }
        public List<Sun> Suns { get; set; } = [];
        public List<Planet> Planets { get; set; } = [];
        public ClusterSystem SharedData { get; set; }

        [JsonIgnore]
        public HashSet<string> Parts
        {
            get
            {
                var parts = GetParts();
                if (SharedData != null)
                    parts = parts.Concat(SharedData.GetParts()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                return parts;
            }
        }

        [JsonIgnore]
        public HashSet<string> AtmoParts
        {
            get
            {
                var parts = GetAtmoParts();
                if (SharedData != null)
                    parts = parts.Concat(SharedData.GetAtmoParts()).ToHashSet(StringComparer.OrdinalIgnoreCase);
                return parts;
            }
        }

        public static ClusterSystem ConvertFrom(Cluster cluster, Dictionary<string, XElement> datasets)
        {
            // This is a weird convention
            // Multiple different systems can point to one system
            // This effectively means that all these systems share their entire system tags combined into one
            // Which can then be reference further to other systems

            // EG: System A and B reference System Template
            // System a has a system tag
            // B and Template both get the system for A
            // If we add System C with its own system tag, both A and C system is combined into one
            // Which all A, B, C and Template will use

            if (datasets.TryGetValue(cluster.BaseGameMapping, out var dataset))
            {
                var systemValue = dataset
                    .Element("properties")?
                    .Element("identification")?
                    .Attribute("system")?.Value;

                var clusterSystem = new ClusterSystem
                {
                    DatasetMacro = dataset.Attribute("macro").Value
                };

                // We have a system id, meaning we need to look through all matching systems
                if (!string.IsNullOrWhiteSpace(systemValue))
                {
                    clusterSystem.SharedData = new ClusterSystem();

                    // Collect all mapdefaults with matching clusterid
                    var matchingDatasets = datasets.Values
                        .Where(a => a
                            .Element("properties")?
                            .Element("identification")?
                            .Attribute("system")?.Value == systemValue);

                    // include the clusterid system itself
                    var leadingZeroClusterId = int.Parse(systemValue).ToString("D2");
                    var clusterMapping = $"cluster_{leadingZeroClusterId}";
                    if (datasets.TryGetValue(clusterMapping, out var clusterIdDataset))
                        matchingDatasets = matchingDatasets.Append(clusterIdDataset);

                    var allDatasetsDistincted = matchingDatasets
                        .DistinctBy(a => a.Attribute("macro").Value, StringComparer.OrdinalIgnoreCase);

                    // Collect all shared data but store it seperately
                    foreach (var datasetDistinct in allDatasetsDistincted)
                        SetupClusterSystemByDataset(clusterSystem.SharedData, datasetDistinct);
                }

                SetupClusterSystemByDataset(clusterSystem, dataset);

                return clusterSystem;
            }
            else
            {
                return null;
            }
        }

        public HashSet<string> GetParts()
        {
            return Planets.Select(a => a.Part)
                .Concat(Planets.SelectMany(a => a.Moons).Select(a => a.Part))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        public HashSet<string> GetAtmoParts()
        {
            return Planets.Select(a => a.Atmopart)
                .Concat(Planets.SelectMany(a => a.Moons).Select(a => a.Atmopart))
                .ToHashSet(StringComparer.OrdinalIgnoreCase);
        }

        private static void SetupClusterSystemByDataset(ClusterSystem clusterSystem, XElement dataset)
        {
            var system = dataset.Element("properties")?.Element("system");
            if (system == null) return;

            var spaceEnv = system.Element("space")?.Attribute("environment")?.Value;
            if (spaceEnv != null)
                clusterSystem.SpaceEnvironment = spaceEnv;

            var suns = system.Element("suns")?.Elements("sun") ?? [];
            foreach (var sun in suns)
            {
                clusterSystem.Suns.Add(new Sun
                {
                    Class = sun.Attribute("class")?.Value,
                    Name = sun.Attribute("name")?.Value,
                });
            }

            var planets = system.Element("planets")?.Elements("planet") ?? [];
            foreach (var planet in planets)
            {
                var planetObj = new Planet
                {
                    Class = planet.Attribute("class")?.Value,
                    Name = planet.Attribute("name")?.Value,
                    Geology = planet.Attribute("geology")?.Value,
                    Settlements = planet.Attribute("settlements")?.Value,
                    Atmosphere = planet.Attribute("atmosphere")?.Value,
                    Population = planet.Attribute("population")?.Value,
                    Part = planet.Attribute("part")?.Value,
                    Atmopart = planet.Attribute("atmopart")?.Value,
                    MaxPopulation = planet.Attribute("maxpopulation")?.Value
                };
                clusterSystem.Planets.Add(planetObj);

                var moons = planet.Element("moons")?.Elements("moon") ?? [];
                foreach (var moon in moons)
                {
                    planetObj.Moons.Add(new Moon
                    {
                        Name = moon.Attribute("name")?.Value,
                        Geology = moon.Attribute("geology")?.Value,
                        Settlements = moon.Attribute("settlements")?.Value,
                        Atmosphere = moon.Attribute("atmosphere")?.Value,
                        Population = moon.Attribute("population")?.Value,
                        Part = moon.Attribute("part")?.Value,
                        Atmopart = moon.Attribute("atmopart")?.Value,
                        MaxPopulation = moon.Attribute("maxpopulation")?.Value
                    });
                }
            }
        }

        public class Sun
        {
            public string Name { get; set; }
            public string Class { get; set; }

            public override string ToString()
            {
                return $"{Name ?? Class}";
            }
        }

        public class Planet : Moon
        {
            public string Class { get; set; }

            public List<Moon> Moons { get; set; } = [];

            public override string ToString()
            {
                return $"{Name}";
            }
        }

        public class Moon
        {
            public string Name { get; set; }
            public string Geology { get; set; }
            public string Atmosphere { get; set; }
            public string Population { get; set; }
            public string MaxPopulation { get; set; }
            public string Settlements { get; set; }
            public string Part { get; set; }
            public string Atmopart { get; set; }

            public override string ToString()
            {
                return $"{Name}";
            }
        }
    }
}
