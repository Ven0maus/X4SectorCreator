using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class MacrosGeneration
    {
        public static void Generate(string folder, string modName, string modPrefix, List<Cluster> clusters)
        {
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("index",
                    new XElement("entry",
                        new XAttribute("name", $"{modPrefix}_CL_*"),
                        new XAttribute("value", $@"extensions\{modName}\maps\XU_ep2_universe\{modPrefix}_clusters")
                    ),
                    new XElement("entry",
                        new XAttribute("name", $"{modPrefix}_SE_*"),
                        new XAttribute("value", $@"extensions\{modName}\maps\XU_ep2_universe\{modPrefix}_sectors")
                    ),
                    new XElement("entry",
                        new XAttribute("name", $"{modPrefix}_ZO_*"),
                        new XAttribute("value", $@"extensions\{modName}\maps\XU_ep2_universe\{modPrefix}_zones")
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
