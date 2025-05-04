namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.NameAlgorithms
{
    internal class ScifiNameGen(ProceduralSettings settings)
    {
        private readonly Random _random = new(settings.Seed);
        private readonly HashSet<string> _usedNames = [];

        private static readonly string[] Prefixes = { "Zar", "Vel", "Xen", "Ark", "Qua", "Nyx", "Tor", "Ely", "Drak", "Oph", "Lyr", "Axi" };
        private static readonly string[] Middles = { "an", "or", "ul", "ex", "ith", "ar", "yn", "os", "ae", "ix", "ur" };
        private static readonly string[] Suffixes = { "ion", "ara", "is", "on", "eus", "um", "os", "ae", "ar", "ax", "eth" };

        private static readonly string[] AlienConsonants = { "kr", "zh", "xt", "dr", "gr", "q", "tz", "ch", "vr", "sk" };
        private static readonly string[] AlienVowels = { "a", "u", "o", "i", "ae", "ou", "ee", "ia" };

        private static readonly string[] AncientRoots = { "Astra", "Nex", "Cael", "Domus", "Ther", "Lux", "Obli", "Seren", "Umbra", "Myth" };
        private static readonly string[] AncientEndings = { "aris", "ium", "ora", "aeon", "thys", "eth", "em", "ios" };

        public string Generate(NameStyle style = NameStyle.Default, bool includeDesignator = true)
        {
            string name;

            do
            {
                switch (style)
                {
                    case NameStyle.Alien:
                        name = GenerateAlienName();
                        break;
                    case NameStyle.Ancient:
                        name = GenerateAncientName();
                        break;
                    case NameStyle.Military:
                        name = GenerateMilitaryName();
                        break;
                    default:
                        name = GenerateDefaultName();
                        break;
                }

                if (includeDesignator && style != NameStyle.Military)
                    name += " " + GenerateDesignator();

            } while (_usedNames.Contains(name));

            _usedNames.Add(name);
            return name;
        }

        private string GenerateDefaultName()
        {
            return Prefixes[_random.Next(Prefixes.Length)] +
                   Middles[_random.Next(Middles.Length)] +
                   Suffixes[_random.Next(Suffixes.Length)];
        }

        private string GenerateAlienName()
        {
            string part1 = AlienConsonants[_random.Next(AlienConsonants.Length)];
            string part2 = AlienVowels[_random.Next(AlienVowels.Length)];
            string part3 = AlienConsonants[_random.Next(AlienConsonants.Length)];
            return Capitalize(part1 + part2 + part3);
        }

        private string GenerateAncientName()
        {
            return AncientRoots[_random.Next(AncientRoots.Length)] +
                   AncientEndings[_random.Next(AncientEndings.Length)];
        }

        private string GenerateMilitaryName()
        {
            string prefix = $"X-{_random.Next(10, 100)}";
            string suffix = $"{(char)_random.Next(65, 91)}{_random.Next(1000, 10000)}";
            return $"{prefix} {suffix}";
        }

        private string GenerateDesignator()
        {
            return $"{(char)_random.Next(65, 91)}{_random.Next(1000, 10000)}";
        }

        private static string Capitalize(string input)
        {
            return char.ToUpper(input[0]) + input[1..];
        }

        public enum NameStyle
        {
            Default,
            Alien,
            Ancient,
            Military
        }
    }
}
