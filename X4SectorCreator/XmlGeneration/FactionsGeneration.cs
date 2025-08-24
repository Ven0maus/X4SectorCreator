using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class FactionsGeneration
    {
        public static void Generate(string folder)
        {
            var factionsContent = CollectFactionsContent();
            if (factionsContent.Length > 0)
            {
                var createPlayerLicenseElement = CollectPlayerLicenses();
                var relationChanges = CollectRelationChanges().ToArray();
                if (relationChanges.Length == 0)
                    relationChanges = null;

                XElement factionsDiff = new("diff", 
                    new XComment("Custom Factions"),
                    new XElement("add",
                        new XAttribute("sel", "/factions"),
                        factionsContent
                    ),
                    relationChanges != null ? new XComment("Relation Changes") : null,
                    relationChanges,
                    new XComment("Player Licenses"),
                    createPlayerLicenseElement
                );

                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    factionsDiff
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/factions.xml")));
            }
        }

        private static XElement[] CollectFactionsContent()
        {
            return FactionsForm.AllCustomFactions
                .Select(a => AddLocalisations(a.Value.Clone()))
                .Select(a => XElement.Parse(a.Serialize()))
                .ToArray();
        }

        /// <summary>
        /// Add's {local:} to localisable properties if they don't already use localisation tags.
        /// </summary>
        /// <param name="faction"></param>
        /// <returns></returns>
        private static Faction AddLocalisations(Faction faction)
        {
            faction.Name = Localisation.Localize(faction.Name);
            faction.Description = Localisation.Localize(faction.Description);
            faction.Shortname = Localisation.Localize(faction.Shortname);
            faction.Prefixname = Localisation.Localize(faction.Prefixname);

            foreach (var license in faction.Licences?.Licence ?? [])
            {
                license.Name = Localisation.Localize(license.Name);
                license.Description = Localisation.Localize(license.Description);
            }

            return faction;
        }

        private static IEnumerable<XElement> CollectRelationChanges()
        {
            var data = new Dictionary<string, List<Objects.Faction.Relation>>(StringComparer.OrdinalIgnoreCase);
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                if (faction.Relations?.Relation != null)
                {
                    foreach (var relation in faction.Relations.Relation)
                    {
                        if (!data.TryGetValue(relation.Faction, out var changes))
                        {
                            changes = [];
                            data[relation.Faction] = changes;
                        }

                        changes.Add(new Objects.Faction.Relation 
                        { 
                            Faction = faction.Id, 
                            RelationValue = relation.RelationValue 
                        });
                    }
                }
            }

            foreach (var mapping in data)
            {
                var addElement = new XElement("add",
                    new XAttribute("sel", $"//factions/faction[@id='{mapping.Key}']/relations"));
                foreach (var obj in mapping.Value)
                {
                    addElement.Add(new XElement("relation",
                        new XAttribute("faction", obj.Faction),
                        new XAttribute("relation", obj.RelationValue)));
                }
                yield return addElement;
            }
        }

        private static XElement CollectPlayerLicenses()
        {
            var element = new XElement("add", new XAttribute("sel", "/factions/faction[@id='player']/licences"));

            var licenseTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                "generaluseequipment",
                "generaluseship",
                "station_gen_basic"
            };

            var data = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase);

            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                if (faction.Licences?.Licence == null) continue;
                foreach (var license in faction.Licences.Licence)
                {
                    if (licenseTypes.Contains(license.Type))
                    {
                        if (!data.TryGetValue(license.Type, out var factions))
                        {
                            factions = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                            data[license.Type] = factions;
                        }

                        factions.Add(faction.Id);
                    }
                }
            }

            foreach (var mapping in data)
            {
                element.Add(new XElement("licence",
                    new XAttribute("type", mapping.Key),
                    new XAttribute("factions", string.Join(" ", mapping.Value))));
            }

            return element;
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
