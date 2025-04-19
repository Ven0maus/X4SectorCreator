using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ShipsGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }
            var shipsElement = CollectShips();
            if (shipsElement == null) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    shipsElement
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/ships.xml")));
        }

        private static XElement CollectShips()
        {
            var mainElement = new XElement("add", new XAttribute("sel", "/ships"));
            int count = 0;
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                foreach (var ship in faction.Ships ?? [])
                {
                    var shipElement = XElement.Parse(ship.Serialize());
                    mainElement.Add(shipElement);
                    count++;
                }
            }
            return count == 0 ? null : mainElement;
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
