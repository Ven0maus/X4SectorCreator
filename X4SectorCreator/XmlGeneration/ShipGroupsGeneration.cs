using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ShipGroupsGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }
            var shipGroupsElement = CollectShipGroups();
            if (shipGroupsElement == null) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    shipGroupsElement
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/shipgroups.xml")));
        }

        private static XElement CollectShipGroups()
        {
            var mainElement = new XElement("add", new XAttribute("sel", "/groups"));
            int count = 0;
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                foreach (var shipGroup in faction.ShipGroups ?? [])
                {
                    var shipGroupElement = XElement.Parse(shipGroup.Serialize());
                    mainElement.Add(shipGroupElement);
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
