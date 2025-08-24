using System.Text.Json;
using System.Xml.Linq;
using X4SectorCreator.Configuration;
using X4SectorCreator.Objects;

namespace DevConsole.Extractors
{
    internal static class RelationsExtractor
    {
        internal static void ExtractRelations(string factionsPath)
        {
            var relations = CollectRelations(factionsPath);

            Console.WriteLine($"Exported \"{relations.Count}\" faction relations.");

            var xml = JsonSerializer.Serialize(relations, ConfigSerializer.JsonSerializerOptions);
            if (!Directory.Exists(Path.GetDirectoryName(Path.Combine("Extractions", "ExtractedRelations.xml"))))
                Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine("Extractions", "ExtractedRelations.xml")));
            File.WriteAllText(Path.Combine("Extractions", "ExtractedRelations.xml"), xml);
        }

        private static Dictionary<string, List<Faction.Relation>> CollectRelations(string factionsPath)
        {
            var xdoc = XDocument.Load(factionsPath);
            var factions = xdoc.Element("factions").Elements("faction");

            var factionRelationObjects = new Dictionary<string, List<Faction.Relation>>();
            foreach (var faction in factions)
            {
                if (faction.Attribute("id").Value.Equals("Ownerless", StringComparison.OrdinalIgnoreCase)) continue;
                var relations = faction.Element("relations").Elements("relation")
                    .Where(a => !a.Attribute("faction").Value.Equals("Ownerless", StringComparison.OrdinalIgnoreCase))
                    .Select(a => new Faction.Relation
                    {
                        Faction = a.Attribute("faction").Value,
                        RelationValue = a.Attribute("relation").Value
                    })
                    .ToList();
                factionRelationObjects[faction.Attribute("id").Value] = relations;
            }

            return factionRelationObjects;
        }
    }
}
