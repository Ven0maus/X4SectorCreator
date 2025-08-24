using System.Globalization;
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
            var relationChanges = CollectRelationChanges().ToArray();
            var factionsContent = CollectFactionsContent();
            if (relationChanges.Length > 0 || factionsContent.Length > 0)
            {
                var createPlayerLicenseElement = CollectPlayerLicenses();

                if (factionsContent.Length == 0)
                {
                    factionsContent = null;
                    createPlayerLicenseElement = null;
                }
                if (relationChanges.Length == 0)
                    relationChanges = null;

                XElement factionsDiff = new("diff",
                    factionsContent != null ? new XComment("Custom Factions") : null,
                    factionsContent != null ? new XElement("add",
                        new XAttribute("sel", "/factions"),
                        factionsContent
                    ) : null,
                    relationChanges != null ? new XComment("Relation Changes") : null,
                    relationChanges,
                    createPlayerLicenseElement != null ? new XComment("Player Licenses") : null,
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
            return [.. FactionsForm.AllCustomFactions
                .Select(a => AddLocalisations(a.Value.Clone()))
                .Select(a => XElement.Parse(a.Serialize()))];
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
            var data = new Dictionary<string, List<Faction.Relation>>(StringComparer.OrdinalIgnoreCase);

            // Handle faction relations from FactionRelationsForm
            // Only export real "changes" not data that wasn't modified compared to the original
            var defaultRelations = FactionRelationsForm.GetDefaultRelations();
            var factionRelations = FactionRelationsForm.GetModifiedFactionRelations();
            foreach (var kvp in factionRelations)
            {
                if (!data.TryGetValue(kvp.Key, out var changes))
                {
                    changes = [];
                    data[kvp.Key] = changes;
                }

                foreach (var relation in kvp.Value)
                {
                    changes.Add(new Faction.Relation
                    {
                        Faction = relation.Key,
                        RelationValue = relation.Value.ToString(CultureInfo.InvariantCulture)
                    });
                }
            }

            foreach (var mapping in data)
            {
                if (!defaultRelations.TryGetValue(mapping.Key, out var oldRels))
                    oldRels = new Faction.RelationsObj { Relation = [] };

                var addElements = new List<XElement>();
                var replaceElements = new List<XElement>();

                foreach (var obj in mapping.Value)
                {
                    var element = new XElement("relation",
                            new XAttribute("faction", obj.Faction),
                            new XAttribute("relation", obj.RelationValue));

                    if (oldRels.Relation.Any(a => a.Faction.Equals(obj.Faction, StringComparison.OrdinalIgnoreCase)))
                    {
                        replaceElements.Add(element);
                    }
                    else
                    {
                        addElements.Add(element);
                    }
                }

                if (addElements.Count > 0)
                {
                    var addElement = new XElement("add",
                        new XAttribute("sel", $"//factions/faction[@id='{mapping.Key}']/relations"));
                    foreach (var element in addElements)
                        addElement.Add(element);
                    yield return addElement;
                }

                if (replaceElements.Count > 0)
                {
                    foreach (var element in replaceElements)
                    {
                        var replaceElement = new XElement("replace",
                            new XAttribute("sel", $"//factions/faction[@id='{mapping.Key}']/relations[@faction='{element.Attribute("faction").Value}']"));
                        replaceElement.Add(element);
                        yield return replaceElement;
                    }
                }
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
