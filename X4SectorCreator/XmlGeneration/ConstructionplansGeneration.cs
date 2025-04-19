﻿using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ConstructionplansGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }
            var xElement = CollectConstructionplans();
            if (xElement == null) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    xElement
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/constructionplans.xml")));
        }

        private static XElement CollectConstructionplans()
        {
            var constructionplanCache = new Dictionary<string, List<Constructionplan>>(StringComparer.OrdinalIgnoreCase);
            var mainElement = new XElement("add", new XAttribute("sel", "/plans"));
            int count = 0;
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                var stationTypes = faction.StationTypes ?? [];
                if (stationTypes.Count == 0) continue;

                if (!constructionplanCache.TryGetValue(faction.Primaryrace, out var constructionplans))
                {
                    constructionplans = CollectConstructionPlans(faction.Primaryrace);
                    constructionplanCache[faction.Primaryrace] = constructionplans;
                }

                if (!constructionplanCache.TryGetValue("generic", out var genericConstructionplans))
                {
                    genericConstructionplans = CollectConstructionPlans("generic");
                    constructionplanCache["generic"] = genericConstructionplans;
                }

                foreach (var stationType in stationTypes)
                {
                    var plan = constructionplans
                        .FirstOrDefault(a => a.Id.Split('_')[1].Equals(stationType, StringComparison.OrdinalIgnoreCase)) 
                        ?? genericConstructionplans
                        .FirstOrDefault(a => a.Id.Split('_')[1].Equals(stationType, StringComparison.OrdinalIgnoreCase));
                    if (plan == null) continue; // Plan not found, skip

                    var clone = plan.Clone();
                    clone.Id = $"{clone.Id.Split('_')[1]}_{faction.Id}";
                    clone.Name = $"{faction.Name} {GetConstructionplanName(stationType)}";
                    var xElement = XElement.Parse(clone.Serialize());
                    mainElement.Add(xElement);
                    count++;
                }
            }
            return count == 0 ? null : mainElement;
        }

        private static string GetConstructionplanName(string stationType)
        {
            return stationType.ToLower() switch
            {
                "defence" => "Defence Platform",
                "tradestation" => "Trading Station",
                "equipmentdock" => "Equipment Dock",
                "wharf" => "Wharf",
                "shipyard" => "Shipyard",
                "piratebase" => "Pirate Base",
                "piratedock" => "Pirate Dock",
                "freeport" => "Free Port",
                _ => stationType,
            };
        }

        private static readonly Lazy<string[]> _constructionPlanFilePaths = new(() =>
        {
            return Directory.GetFiles(Path.Combine(Application.StartupPath, $"Data/Presets/Constructionplans"), "*.xml");
        });

        private static List<Constructionplan> CollectConstructionPlans(string race)
        {
            var preset = _constructionPlanFilePaths.Value
                .FirstOrDefault(a => Path.GetFileName(a)
                    .StartsWith(race, StringComparison.OrdinalIgnoreCase));

            if (preset == null) 
                return [];

            var plansXml = File.ReadAllText(preset);
            return Constructionplans.Deserialize(plansXml)?.Plan ?? [];
        }

        private static string EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            return filePath;
        }
    }
}
