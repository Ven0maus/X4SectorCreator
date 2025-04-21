using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class WaresGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            var wares = CollectWares().ToArray();
            if (wares == null || wares.Length == 0) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    wares
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/wares.xml")));
        }

        private static IEnumerable<XElement> CollectWares()
        {
            var xml = File.ReadAllText(Constants.DataPaths.WaresMappingPath);
            var allWares = Wares.Deserialize(xml).Ware;

            // Add respective owners to wares
            foreach (var ware in allWares)
            {
                var wareElement = new XElement("add", new XAttribute("sel", $"/wares/ware[@id='{ware.Id}']"));
                int count = 0;
                foreach (var faction in FactionsForm.AllCustomFactions)
                {
                    if (ware.OwnerObj != null && ware.OwnerObj
                        .Any(a => a.Faction != null && a.Faction
                            .Equals(faction.Value.Primaryrace, StringComparison.OrdinalIgnoreCase)))
                    {
                        wareElement.Add(new XElement("owner", new XAttribute("faction", faction.Value.Id)));
                        count++;
                    }
                }
                if (count > 0)
                    yield return wareElement;
            }

            // Add paintmod wares
            var paintModsElement = new XElement("add", new XAttribute("sel", "//wares"));
            foreach (var faction in FactionsForm.AllCustomFactions)
            {
                paintModsElement.Add(new XElement("ware", new XAttribute("id", $"paintmod_{faction.Value.Id}"),
                    new XAttribute("name", $"{faction.Value.Name} Paint Mod"),
                    new XAttribute("description", ""),
                    new XAttribute("transport", "inventory"),
                    new XAttribute("volume", "1"),
                    new XAttribute("tags", "inventory paintmod"),
                    new XElement("price",
                        new XAttribute("min", "2500"),
                        new XAttribute("average", "3000"),
                        new XAttribute("max", "3500")
                        )
                    )
                );
            }
            yield return paintModsElement;
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
