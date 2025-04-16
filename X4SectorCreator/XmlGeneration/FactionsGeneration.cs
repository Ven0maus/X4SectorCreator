using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class FactionsGeneration
    {
        public static void Generate(string folder)
        {
            var factionsContent = CollectFactionsContent();
            if (factionsContent.Length > 0)
            {
                XElement factionsDiff = new("diff", 
                    new XElement("add",
                        new XAttribute("sel", "/factions"),
                        factionsContent
                    )
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
                .Select(a => XElement.Parse(a.Value.Serialize()))
                .ToArray();
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
