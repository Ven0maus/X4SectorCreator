using X4SectorCreator.Helpers;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.NameAlgorithms
{
    internal class FactionDescriptionGen
    {
        private readonly Random _random;

        public FactionDescriptionGen(int seed)
        {
            _random = new Random(seed);
        }

        public enum FactionType
        {
            Human,
            Alien,
            Robot
        }

        public string Generate(FactionNameGen.FactionNameStyle style)
        {
            return style switch
            {
                FactionNameGen.FactionNameStyle.Human => GenerateHumanDescription(),
                FactionNameGen.FactionNameStyle.Alien => GenerateAlienDescription(),
                FactionNameGen.FactionNameStyle.Robot => GenerateRobotDescription(),
                _ => "An enigmatic presence in the stars."
            };
        }

        private string GenerateHumanDescription()
        {
            var goals = new[] {
            "expansion", "technological supremacy", "resource dominance", "interstellar trade", "diplomatic leverage"
        };

            var traits = new[] {
            "pragmatic", "ambitious", "fractured", "industrialized", "militarized"
        };

            var origins = new[] {
            "the Core Worlds", "a collapsed federation", "the Outer Colonies", "Earth's last successor state", "the Cradle System"
        };

            return $"A {traits.Pick(_random)} human faction originating from {origins.Pick(_random)}, driven by {goals.Pick(_random)}.";
        }

        private string GenerateAlienDescription()
        {
            var behaviors = new[] {
            "xenophobic", "ancient and secretive", "curiously benevolent", "biologically adaptive", "ritualistic"
        };

            var motivations = new[] {
            "balance with the cosmos", "ascension", "territorial preservation", "revenge for an ancient war", "genetic perfection"
        };

            var origins = new[] {
            "a dying nebula", "beyond the galactic rim", "a black-hole cluster", "the ruins of a forgotten empire", "the Maelstrom Expanse"
        };

            return $"An {behaviors.Pick(_random)} alien species from {origins.Pick(_random)}, seeking {motivations.Pick(_random)}.";
        }

        private string GenerateRobotDescription()
        {
            var purposes = new[] 
            {
                "total optimization", "perfect order", "self-replication", "containment of biological chaos", "the preservation of logic"
            };

            var traits = new[] 
            {
                "coldly calculating", "networked", "relentlessly efficient", "detached from emotion", "recursive"
            };

            var origins = new[] 
            {
                "a forgotten research array", "the last machine world", "an ancient AI conclave", "an abandoned orbital station", "the data-cradle of an extinct race"
            };


            return $"A {traits.Pick(_random)} machine collective originating from {origins.Pick(_random)}, pursuing {purposes.Pick(_random)}.";
        }
    }
}
