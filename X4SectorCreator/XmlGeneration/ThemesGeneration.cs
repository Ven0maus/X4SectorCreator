using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ThemesGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            var themes = CollectThemes().ToArray();
            if (themes == null || themes.Length == 0) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    themes
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/themes.xml")));
        }

        private static IEnumerable<XElement> CollectThemes()
        {
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                // Default theme for faction
                yield return new XElement("add",
                    new XAttribute("sel", "/themes/theme[@id='painttheme_race_default']"),
                    new XElement("group", 
                        new XAttribute("faction", faction.Id),
                        new XAttribute("paintmod", $"paintmod_{faction.Id}")
                    )
                );

                // Ship variation theme for all factions
                yield return new XElement("add",
                    new XAttribute("sel", "/themes/theme[@id='painttheme_race_ship']"),
                    new XElement("group",
                        new XAttribute("faction", faction.Id),
                        new XAttribute("selection", "random"),
                        new XAttribute("paintmod", ""),
                        new XElement("select",
                            new XAttribute("paintmod", $"paintmod_{faction.Id}"),
                            new XAttribute("weight", "100")
                        )
                    )
                );
            }
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
