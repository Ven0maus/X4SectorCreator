using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ComponentsGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters)
        {
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    new XElement("add",
                        new XAttribute("sel", "/index"),
                        GenerateClusterComponentXml(modPrefix, clusters)
                    )
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"index/components.xml")));
        }

        private static IEnumerable<XElement> GenerateClusterComponentXml(string modPrefix, List<Cluster> clusters)
        {
            foreach (var cluster in clusters)
            {
                // By default I'm pointing all new sectors to take over the look of Cluster 01
                // It's possible to define your own cluster look by copying another and modifying it and updating the path here
                yield return new XElement("entry",
                    new XAttribute("name", $"{modPrefix}_CL_c{cluster.Id:D3}"),
                    new XAttribute("value", $@"assets\environments\cluster\Cluster_01")
                );
            }
        }

        private static string EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
                Directory.CreateDirectory(directoryPath);
            return filePath;
        }
    }
}
