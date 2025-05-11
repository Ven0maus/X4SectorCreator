using System.Drawing.Imaging;
using System.Text;
using System.Text.RegularExpressions;
using X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.NameAlgorithms;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Forms.Galaxy.ProceduralGeneration.Algorithms.FactionAlgorithms
{
    internal partial class FactionCreator(int seed)
    {
        private readonly Random _random = new(seed);
        private readonly FactionNameGen _nameGen = new(seed);
        private readonly FactionDescriptionGen _descGen = new(seed);
        private readonly FactionColorGen _factionColorGen = new(seed);
        private readonly FactionNameGen.FactionNameStyle[] _factionTypes = Enum.GetValues<FactionNameGen.FactionNameStyle>();

        private readonly string[] _pirateRaces = ["argon", "teladi", "paranid"];
        private readonly string[] _races = ["argon", "terran", "teladi", "paranid", "boron", "split"];
        private readonly string[] _levels = ["verylow", "low", "normal", "high", "veryhigh"];

        public Faction Generate(bool isPirateFaction)
        {
            var factionType = _factionTypes[_random.Next(_factionTypes.Length)];
            var faction = new Faction
            {
                Name = _nameGen.Generate(factionType),
                Description = _descGen.Generate(factionType)
            };

            var race = isPirateFaction ? _pirateRaces.RandomOrDefault(_random) : _races.RandomOrDefault(_random);

            faction.Id = SanitizeNameForId(faction.Name);
            faction.Shortname = GetShortName(faction.Name);
            faction.Prefixname = faction.Shortname;
            faction.PoliceFaction = isPirateFaction ? null : faction.Id;
            faction.Primaryrace = race;
            faction.AggressionLevel = _levels[_random.Next(_levels.Length)];
            faction.AvariceLevel = _levels[_random.Next(_levels.Length)];
            faction.Lawfulness = _random.NextDouble().ToString("0.##");
            faction.Behaviourset = "default";
            faction.Color = _factionColorGen.GenerateDistinctColor();

            // Set color and icon data refs
            var dataEntryName = $"faction_{faction.Id}";
            faction.ColorData = new Faction.ColorDataObj { Ref = dataEntryName };
            faction.IconData = new Faction.IconObj { Active = dataEntryName, Inactive = dataEntryName };

            if (!isPirateFaction)
            {
                faction.DesiredEquipmentDocks = "1";
                faction.DesiredWharfs = "1";
                faction.DesiredShipyards = "1";
                faction.DesiredTradeStations = "1";
                faction.Tags = "claimspace economic police privateloadout privateship protective publicloadout publicship standard watchdoguser custom";
                faction.PrefferedHqStationTypes = ["shipbuilding"];
                faction.StationTypes = ["wharf", "shipyard", "tradestation", "defence", "equipmentdock"];
            }
            else
            {
                faction.Tags = "economic pirate plunder privateloadout privateship protective watchdoguser custom";
                faction.PrefferedHqStationTypes = ["any"];
                faction.StationTypes = ["piratedock", "piratebase", "freeport"];
            }

            DefineLicenses(faction);
            DefineIcon(faction, factionType);
            DefineFactionShips(faction, isPirateFaction);

            return faction;
        }

        private static void DefineFactionShips(Faction faction, bool isPirateFaction)
        {
            faction.ShipGroups = [];
            faction.Ships = [];

            var race = isPirateFaction ? "scaleplate" : faction.Primaryrace;
            var shipGroups = FactionShipsForm.ShipGroupPresets[race];
            foreach (var shipGroup in shipGroups.Group)
            {
                var newGroup = shipGroup.Clone();
                newGroup.Name = $"{faction.Id}_{string.Join("_", shipGroup.Name.Split('_').Skip(1))}";

                faction.ShipGroups.Add(newGroup);
            }

            var ships = FactionShipsForm.ShipPresets[race];
            foreach (var ship in ships.Ship)
            {
                var newShip = ship.Clone();
                newShip.Id = $"{faction.Id}_{string.Join("_", ship.Id.Split('_').Skip(1))}";
                if (ship.Group != null)
                    newShip.Group = $"{faction.Id}_{string.Join("_", ship.Group.Split('_').Skip(1))}";

                if (newShip.CategoryObj != null)
                    newShip.CategoryObj.Faction = faction.Id;
                if (newShip.PilotObj != null && newShip.PilotObj.Select != null)
                    newShip.PilotObj.Select.Faction = faction.Id;

                faction.Ships.Add(newShip);
            }
        }

        private void DefineIcon(Faction faction, FactionNameGen.FactionNameStyle style)
        {
            var factionIcon = FactionIconGen.GenerateFactionIcon(_random.Next(0, 1000000), style);
            faction.Icon = ImageHelper.ImageToBase64(factionIcon, ImageFormat.Png);
        }

        private static void DefineLicenses(Faction faction)
        {
            faction.Licences = new Faction.LicencesObj { Licence = [] };
            var defaultLicenses = FactionForm.GetPlaceholderLicenses();
            foreach (var license in defaultLicenses)
                faction.Licences.Licence.Add(license);
            FactionForm.UpdateLicenseNames(faction, false);
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
