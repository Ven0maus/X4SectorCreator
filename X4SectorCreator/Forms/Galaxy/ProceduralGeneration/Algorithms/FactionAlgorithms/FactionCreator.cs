using System.Text;
using System.Text.RegularExpressions;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.NameAlgorithms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms
{
    internal partial class FactionCreator(int seed)
    {
        private readonly Random _random = new(seed);
        private readonly FactionNameGen _nameGen = new(seed);
        private readonly FactionDescriptionGen _descGen = new(seed);
        private readonly FactionNameGen.FactionNameStyle[] _factionTypes = Enum.GetValues<FactionNameGen.FactionNameStyle>();

        public Faction Generate(bool isPirateFaction)
        {
            var factionType = _factionTypes[_random.Next(_factionTypes.Length)];

            var faction = new Faction
            {
                Name = _nameGen.Generate(factionType),
                Description = _descGen.Generate(factionType)
            };
            faction.Id = SanitizeNameForId(faction.Name);
            faction.Shortname = GetShortName(faction.Name);
            faction.Prefixname = faction.Shortname;
            faction.PoliceFaction = isPirateFaction ? null : "self";

            // Set licenses
            // Set race
            // Set policefaction = self
            // Set aggression, avarice, lawfulness
            // Set tags
            // Set color
            // Set HQ
            // Set some kind of auto generated icon?

            DefineFactionShips(faction);
            DefineFactionStations(faction);

            return faction;
        }

        private void DefineFactionShips(Faction faction)
        {
            // Ship groups
            // Ships
        }

        private void DefineFactionStations(Faction faction)
        {
            // Station types
            // Desired wharfs, shipyards, equipment docks, tradestations
            // faction rep station types
        }

        private static string GetShortName(string fullName)
        {
            // Remove any digits from input
            var cleanName = new string(fullName.Where(c => !char.IsDigit(c)).ToArray());

            // Split by common delimiters
            var parts = cleanName
                .Split([' ', '-', '_'], StringSplitOptions.RemoveEmptyEntries)
                .Where(p => p.Length > 0)
                .ToList();

            if (parts.Count == 0)
                return "UNK"; // fallback for empty/invalid input

            if (parts.Count == 1)
            {
                // One word: Take first 3 alphabetic chars
                return new string(parts[0]
                    .Where(char.IsLetter)
                    .Take(3)
                    .Select(char.ToUpperInvariant)
                    .ToArray());
            }

            // Multi-word: Take first letter of each up to 3, skip numbers
            var shortName = new StringBuilder();
            foreach (var part in parts)
            {
                var firstLetter = part.FirstOrDefault(char.IsLetter);
                if (firstLetter != default && shortName.Length < 3)
                    shortName.Append(char.ToUpperInvariant(firstLetter));
            }

            // Pad with more letters from the first part if under 3
            var firstWordLetters = parts[0].Where(char.IsLetter).Skip(1);
            foreach (var c in firstWordLetters)
            {
                if (shortName.Length >= 3) break;
                shortName.Append(char.ToUpperInvariant(c));
            }

            return shortName.ToString().PadRight(3, 'X'); // fallback pad if needed
        }

        private static string SanitizeNameForId(string name)
        {
            // Replace whitespace with underscores
            string sanitized = SanitizeFactionName().Replace(name, "_");

            // Remove all non-alphanumeric and non-underscore characters
            sanitized = SanitizeFactionNameFurther().Replace(sanitized, "");

            // Optionally make lowercase for consistency
            return sanitized.ToLowerInvariant();
        }

        [GeneratedRegex(@"\s+")]
        private static partial Regex SanitizeFactionName();
        [GeneratedRegex(@"[^\w\d_]")]
        private static partial Regex SanitizeFactionNameFurther();
    }
}
