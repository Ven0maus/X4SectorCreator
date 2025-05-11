namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.NameAlgorithms
{
    internal class FactionNameGen(int seed)
    {
        private readonly Random _random = new(seed);
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
            var prefixes = new[]
            {
                // Political / Structural
                "United", "New", "Old", "People's", "Imperial", "Sovereign", "Free", "Great", "Greater", "Outer", "Inner", "Autonomous", "Neo", "Pan", "Interstellar",

                // Types of government
                "Republic of", "Confederacy of", "Empire of", "Federation of", "Kingdom of", "Theocracy of", "Technocracy of", "Dominion of", "Order of", "Collective of", "Alliance of", "Consortium of",

                // Abstract starters
                "The", "Legacy of", "Children of", "Heralds of", "Keepers of", "Descendants of", "Council of", "Lords of", "Guardians of"
            };

            var cores = new[]
            {
                // Celestial / Mythical
                "Helios", "Nova", "Astra", "Orion", "Lyra", "Solari", "Prometheus", "Valkar", "Drakon", "Zereth", "Arcadia", "Chronos", "Oblivion", "Seraphis", "Ignis", "Eclipse", "Horizon", "Nyx", "Aether", "Lumina", "Erebus",

                // Ideological / Conceptual
                "Unity", "Dominion", "Ascension", "Balance", "Truth", "Justice", "Progress", "Harmony", "Genesis", "Legacy", "Concord", "Exodus", "Continuum", "Silence", "Redemption", "Echo",

                // Sci-fi / Industrial
                "Nexus", "Core", "Grid", "Axis", "Protocol", "System", "Prime", "Node", "Sector", "Axiom", "Vector", "Forge", "Citadel", "Vault", "Beacon", "Module", "Control", "Drive"
            };

            var suffixes = new[]
            {
                // Government / Org types
                "Federation", "Empire", "League", "Union", "Syndicate", "Dynasty", "Conglomerate", "Accord", "Coalition", "Front", "Compact", "Command", "Council", "Regime", "Circle", "Assembly", "Network",

                // Military / Religious
                "Crusade", "Legion", "Armada", "Faith", "Cult", "Order", "Knighthood", "Ascendancy", "Dominion", "Host", "Covenant", "Flame", "Oath",

                // Corporate / Sci-fi
                "Consortium", "Division", "Collective", "Core", "Protocol", "Interface", "Group", "Fabric", "Matrix", "Directive", "Enterprise", "Synthesis", "Combine"
            };

            string[] templates =
            [
                "{prefix} {core}",
                "{core} {suffix}",
                "{prefix} {core} {suffix}",
                "{prefix} of the {core} {suffix}",
                "{prefix} of {core}",
                "The {core} {suffix}",
                "{core}-{core2} {suffix}",                    // e.g. "Nova-Astra Combine"
                "{core} of the {suffix}",                   // e.g. "Exodus of the Flame"
                "{prefix} {core} of {core2}",                // e.g. "Federation of Helios Ascension"
                "{prefix} {core}, {suffix}"                 // e.g. "Sovereign Unity, Empire"
            ];

            string Get(string[] arr) => arr[_random.Next(arr.Length)];

            string prefix = Get(prefixes);
            string core1 = Get(cores);
            string core2 = Get(cores);
            while (core2 == core1) core2 = Get(cores);
            string suffix = Get(suffixes);

            string template = templates[_random.Next(templates.Length)];

            return template
                .Replace("{prefix}", prefix)
                .Replace("{core}", core1)
                .Replace("{core2}", core2)
                .Replace("{suffix}", suffix);
        }

        private string GenerateAlienFactionName()
        {
            var alienCores = new[]
            {
                "Xel", "Kzr", "N'vak", "Thul", "Vrass", "Jha", "Zyn", "Orx", "Ch'rak", "Ghu",
                "Srr", "Vroth", "G'zil", "Threx", "Ux", "Ka'nar", "Ylth", "Qrith", "Mrrg", "Dhul"
            };

            var forms = new[]
            {
                "Dominion", "Swarm", "Hegemony", "Brood", "Clutch", "Continuum", "Pulse", "Cluster", "Collective", "Scourge", "Covenant",
                "Chorus", "Synod", "Confluence", "Echo", "Hive", "Mycelium", "Fractal", "Overmind", "Plenum", "Propagule"
            };

            var vowels = new[] { "a", "u", "o", "i", "ae", "ou", "ee", "ia", "uu", "ei" };

            var consonants = new[] { "kr", "zh", "xt", "dr", "gr", "q", "tz", "ch", "vr", "sk", "gl", "sh", "rk", "bh", "mn" };

            var prefixes = new[]
            {
                "The", "Ancient", "Eternal", "Voracious", "Primordial", "Nameless", "Voidborn", "Assimilated", "Shattered", "Broken", "Awakened", "Transcendent"
            };

            var suffixes = new[]
            {
                "of the Void", "of Hunger", "of Silence", "from Beyond", "of Unbeing", "of the Deep", "of Stars", "of Ascension", "from the Rift", "of the Endless", "of Flesh", "of the Maw"
            };

            string[] alienTemplates =
            [
                "{prefix} {core} {form}",             // "Ancient Xel Brood"
                "{core}-{core2} {form}",              // "N'vak-Thul Hegemony"
                "{form} of {core}",                  // "Swarm of Vrass"
                "{core} {form} {suffix}",           // "Ghu Cluster of the Void"
                "{prefix} {form} of {core}",         // "The Scourge of Jha"
                "{form} {suffix}",                   // "Brood of Flesh"
                "{form} of the {core} {form}",       // "Clutch of the Orx Continuum"
                "{prefix} {core}-{core2}",            // "Nameless Ghu-Zyn"
                "{core}-{syllable}-{core2} {form}",   // "Ch'rak-ia-Vrass Covenant"
                "{form} of {core}-{core2}"            // "Pulse of Ghu-Kzr"
            ];


            string Get(string[] arr) => arr[_random.Next(arr.Length)];

            string prefix = Get(prefixes);
            string core1 = Get(alienCores);
            string core2 = Get(alienCores);
            while (core2 == core1) core2 = Get(alienCores);

            string vowel = Get(vowels);
            string consonant = Get(consonants);
            string syllable = consonant + vowel;

            string form = Get(forms);
            string suffix = Get(suffixes);

            string template = alienTemplates[_random.Next(alienTemplates.Length)];

            return template
                .Replace("{prefix}", prefix)
                .Replace("{core}", core1)
                .Replace("{core2}", core2)
                .Replace("{form}", form)
                .Replace("{syllable}", syllable)
                .Replace("{suffix}", suffix);
        }

        private string GenerateRobotFactionName()
        {
            var codePrefixes = new[]
            {
                "SOVR", "EXOD", "VNTR", "CORE", "DRM", "SYS", "NEX", "IOM", "ZRA", "MNX", "NSR", "LUXR", "VCTR", "CTRL",
                "AXIOM", "SIGMA", "V3X", "PRTCL", "ALPHA", "BETA", "QNTM", "OBLX", "MNDX", "ARCA", "SUBL", "OMNIX", "CYBR"
            };

            var cores = new[]
            {
                "Protocol", "Collective", "Directive", "Assembly", "Override", "Core", "Process", "Subroutine", "Node", "Nexus",
                "Mainframe", "Circuit", "Daemon", "Shell", "Loop", "Archive", "Cluster", "Matrix", "Fabric", "Routine"
            };

            var suffixes = new[]
            {
                "Prime", "Unit", "Array", "Zero", "Seed", "Link", "Echo", "Hub", "Core", "Bridge", "Arc", "Forge", "Vault", "Beacon"
            };

            var titles = new[]
            {
                "The Singularity", "System Prime", "Machine Ascendancy", "Logic Path", "Artificial Accord", "Signal Unity",
                "The Calculation", "Prime Directive", "The Infinite Process", "Neural Convergence", "The Sync", "Core Awakening"
            };

            string[] robotTemplates =
            [
                "{prefix}-{core}",                    // SYS-Protocol
                "{prefix}-{core} {suffix}",          // NEX-Node Vault
                "{core} of {prefix}",                // Directive of LUXR
                "{prefix}-{core}-{suffix}",          // VCTR-Protocol-Prime
                "{prefix}-{prefix2} {core}",          // AXIOM-EXOD Assembly
                "The {core} {suffix}",               // The Subroutine Arc
                "{core} {suffix} of {prefix}",       // Loop Unit of MNX
                "{title}",                           // The Singularity
                "{core} of the {title}",             // Node of the Sync
                "{prefix} {core} Assembly"           // CTRL Process Assembly
            ];

            string Get(string[] arr) => arr[_random.Next(arr.Length)];

            string prefix1 = Get(codePrefixes);
            string prefix2 = Get(codePrefixes);
            while (prefix2 == prefix1) prefix2 = Get(codePrefixes);

            string core = Get(cores);
            string suffix = Get(suffixes);
            string title = Get(titles);

            string template = robotTemplates[_random.Next(robotTemplates.Length)];

            return template
                .Replace("{prefix}", prefix1)
                .Replace("{prefixé}", prefix2)
                .Replace("{core}", core)
                .Replace("{suffix}", suffix)
                .Replace("{title}", title);
        }

        public enum FactionNameStyle
        {
            Human,
            Alien,
            Robot,
        }
    }
}
