using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ColorsGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    CollectColors(),
                    CollectMappings()
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/colors.xml")));
        }

        private static XElement CollectColors()
        {
            var mainElement = new XElement("add", new XAttribute("sel", "/colormap/colors"));
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                mainElement.Add(new XElement("color", 
                    new XAttribute("id", $"color_{faction.Id}"),
                    new XAttribute("r", faction.Color.R.ToString()), 
                    new XAttribute("g", faction.Color.G.ToString()), 
                    new XAttribute("b", faction.Color.B.ToString()), 
                    new XAttribute("a", "255")));
            }
            return mainElement;
        }

        private static XElement CollectMappings()
        {
            var mainElement = new XElement("add", new XAttribute("sel", "/colormap/mappings"));
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                mainElement.Add(new XElement("mapping", 
                    new XAttribute("id", faction.ColorData.Ref),
                    new XAttribute("ref", $"color_{faction.Id}")));
            }
            return mainElement;
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
