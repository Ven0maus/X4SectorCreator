using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ClusterGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters)
        {
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("macros",
                    GenerateClusters(modPrefix, clusters)
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"maps/xu_ep2_universe/{modPrefix}_clusters.xml")));
        }

        private static IEnumerable<XElement> GenerateClusters(string modPrefix, List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters.OrderBy(a => a.Id))
            {
                yield return new XElement("macro",
                    new XAttribute("name", $"{modPrefix}_CL_c{cluster.Id:D3}_macro"),
                    new XAttribute("class", "cluster"),
                    new XElement("component",
                        new XAttribute("ref", "standardcluster")
                    ),
                    new XElement("connections",
                        GenerateConnections(modPrefix, cluster)
                    )
                );
            }
        }

        private static IEnumerable<XElement> GenerateConnections(string modPrefix, Cluster cluster)
        {
            foreach (Sector sector in cluster.Sectors.OrderBy(a => a.Id))
            {
                yield return new XElement("connection",
                    new XAttribute("name", $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_connection"),
                    new XAttribute("ref", "sectors"),
                    new XElement("macro",
                        new XAttribute("ref", $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro"),
                        new XAttribute("connection", "cluster")
                    )
                );
            }

            // Adding content connection
            yield return new XElement("connection",
                new XAttribute("ref", "content"),
                new XElement("macro",
                    new XElement("component",
                        new XAttribute("connection", "space"),
                        new XAttribute("ref", $"Cluster_01") // TODO: Allow picking any base game cluster here
                    )
                )
            );
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
