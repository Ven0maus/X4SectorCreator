using System.Xml.Linq;

namespace X4SectorCreator.XmlGeneration
{
    internal static class MacrosGeneration
    {
        public static void Generate(string folder, string modName, string modPrefix)
        {
            string galaxyName = GalaxySettingsForm.IsCustomGalaxy ?
                $"{modPrefix}_{GalaxySettingsForm.GalaxyName}" : GalaxySettingsForm.GalaxyName;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("index",
                    GalaxySettingsForm.IsCustomGalaxy ? new XElement("entry",
                        new XAttribute("name", $"{galaxyName}_macro"),
                        new XAttribute("value", $@"extensions\{modName}\maps\{galaxyName}\galaxy")
                    ) : null,
                    new XElement("entry",
                        new XAttribute("name", $"{modPrefix}_CL_*"),
                        new XAttribute("value", $@"extensions\{modName}\maps\{galaxyName}\{modPrefix}_clusters")
                    ),
                    new XElement("entry",
                        new XAttribute("name", $"{modPrefix}_SE_*"),
                        new XAttribute("value", $@"extensions\{modName}\maps\{galaxyName}\{modPrefix}_sectors")
                    ),
                    new XElement("entry",
                        new XAttribute("name", $"{modPrefix}_ZO_*"),
                        new XAttribute("value", $@"extensions\{modName}\maps\{galaxyName}\{modPrefix}_zones")
                    )
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"index/macros.xml")));
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
