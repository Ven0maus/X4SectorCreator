using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class MacrosGeneration
    {
        public static void Generate(string folder, string modName, string modPrefix, List<Cluster> clusters)
        {
            Cluster[] customClusters = clusters
                .Where(a => !a.IsBaseGame || a.Sectors.Any(a => !a.IsBaseGame))
                .ToArray();

            List<XElement> elements =
            [
                GalaxySettingsForm.IsCustomGalaxy ? new XElement("entry",
                        new XAttribute("name", $"{GalaxySettingsForm.GalaxyName}_macro"),
                        new XAttribute("value", $@"extensions\{modName}\maps\{GalaxySettingsForm.GalaxyName}\galaxy")
                    ) : null,
                    customClusters.Length > 0 ? new XElement("entry",
                        new XAttribute("name", $"{modPrefix}_CL_*"),
                        new XAttribute("value", $@"extensions\{modName}\maps\{GalaxySettingsForm.GalaxyName}\{modPrefix}_clusters")
                    ) : null,
                    customClusters.Length > 0 ? new XElement("entry",
                        new XAttribute("name", $"{modPrefix}_SE_*"),
                        new XAttribute("value", $@"extensions\{modName}\maps\{GalaxySettingsForm.GalaxyName}\{modPrefix}_sectors")
                    ) : null,
                    customClusters.Length > 0 ? new XElement("entry",
                        new XAttribute("name", $"{modPrefix}_ZO_*"),
                        new XAttribute("value", $@"extensions\{modName}\maps\{GalaxySettingsForm.GalaxyName}\{modPrefix}_zones")
                    ) : null
            ];
            _ = elements.RemoveAll(a => a == null);

            if (elements.Count > 0)
            {
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("index",
                        elements
                    )
                );

                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"index/macros.xml")));
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
