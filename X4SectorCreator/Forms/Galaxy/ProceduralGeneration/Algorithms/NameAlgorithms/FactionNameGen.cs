namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.NameAlgorithms
{
    internal class FactionNameGen(int seed)
    {
        private readonly Random _random = new Random(seed);
        private readonly HashSet<string> _usedNames = [];

        public string Generate(FactionNameStyle style = FactionNameStyle.Human)
        {
            string name;

            do
            {
                name = style switch
                {
                    FactionNameStyle.Human => GenerateHumanFactionName(),
                    FactionNameStyle.Alien => GenerateAlienFactionName(),
                    FactionNameStyle.Robot => GenerateRobotFactionName(),
                    _ => GenerateHumanFactionName()
                };
            } while (_usedNames.Contains(name));

            _usedNames.Add(name);
            return name;
        }

        private string GenerateHumanFactionName()
        {
            var prefixes = new[] { "United", "New", "Solar", "Galactic", "Federation of", "Alliance of", "Order of", "Dynasty of", "The", "Republic of" };
            var suffixes = new[] { "Federation", "Empire", "Consortium", "League", "Syndicate", "Front", "Union", "Dynasty", "Collective", "Accord" };
            var cores = new[] { "Velar", "Axiom", "Lyron", "Helios", "Nova", "Astra", "Dominus", "Orion", "Echelon" };

            if (_random.NextDouble() < 0.5)
            {
                // "Federation of Velar"
                return $"{prefixes[_random.Next(prefixes.Length)]} {cores[_random.Next(cores.Length)]} {suffixes[_random.Next(suffixes.Length)]}";
            }
            else
            {
                // "The Orion Accord"
                return $"{prefixes[_random.Next(prefixes.Length)]} {cores[_random.Next(cores.Length)]}";
            }
        }

        private string GenerateAlienFactionName()
        {
            var alienCores = new[] { "Xel", "Kzr", "N'vak", "Thul", "Vrass", "Jha", "Zyn", "Orx", "Ch'rak", "Ghu" };
            var forms = new[] { "Dominion", "Swarm", "Hegemony", "Brood", "Conglomerate", "Continuum", "Clutch", "Cluster", "Pulse" };
            var vowels = new[] { "a", "u", "o", "i", "ae", "ou", "ee", "ia" };
            var consonants = new[] { "kr", "zh", "xt", "dr", "gr", "q", "tz", "ch", "vr", "sk" };

            string part1 = consonants[_random.Next(consonants.Length)];
            string part2 = vowels[_random.Next(vowels.Length)];
            string part3 = consonants[_random.Next(consonants.Length)];
            string creatureName = Capitalize(part1 + part2 + part3);

            return $"{alienCores[_random.Next(alienCores.Length)]}-{creatureName} {forms[_random.Next(forms.Length)]}";
        }

        private string GenerateRobotFactionName()
        {
            var codePrefixes = new[]
            {
                "SOVR", "EXOD", "VNTR", "CORE", "DRM", "SYS", "NEX", "IOM", "ZRA", "MNX", "NSR", "LUXR", "VCTR", "CTRL"
            };

            var cores = new[]
            {
                "Protocol", "Collective", "Directive", "Assembly", "Override", "Core", "Process", "Subroutine", "Node", "Nexus"
            };

            string prefix = codePrefixes[_random.Next(codePrefixes.Length)];
            int num = _random.Next(1, 100);

            return $"{prefix}-{num} {cores[_random.Next(cores.Length)]}";
        }

        private static string Capitalize(string input)
        {
            return char.ToUpper(input[0]) + input[1..];
        }

        public enum FactionNameStyle
        {
            Human,
            Alien,
            Robot,
        }
    }
}
