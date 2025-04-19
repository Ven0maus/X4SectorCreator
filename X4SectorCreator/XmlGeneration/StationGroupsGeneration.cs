using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class StationGroupsGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }
            var xElement = CollectStationGroups();
            if (xElement == null) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    xElement
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/stationgroups.xml")));
        }

        private static XElement CollectStationGroups()
        {
            var mainElement = new XElement("add", new XAttribute("sel", "/groups"));
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                foreach (var stationType in faction.StationTypes ?? [])
                {
                    var xElement = new XElement("group", 
                        new XAttribute("name", $"{stationType}_{faction.Id}"),
                        new XElement("select", 
                            new XAttribute("constructionplan", $"{stationType}_{faction.Id}")
                        )
                    );
                    mainElement.Add(xElement);
                }
            }
            return mainElement.IsEmpty ? null : mainElement;
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
