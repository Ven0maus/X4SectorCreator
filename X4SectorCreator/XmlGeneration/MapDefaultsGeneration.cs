using System.Globalization;
using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class MapDefaultsGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters)
        {
            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("defaults",
                    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                    new XAttribute(xsi + "noNamespaceSchemaLocation", "libraries.xsd"),
                    GenerateClusterElements(modPrefix, clusters)
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/mapdefaults.xml")));
        }

        private static List<XElement> GenerateClusterElements(string modPrefix, List<Cluster> clusters)
        {
            List<XElement> elements = [];

            foreach (Cluster cluster in clusters.Where(a => !a.IsBaseGame))
            {
                XObject clusterFactionLogicTag = AddFactionLogic(cluster: cluster);
                // Add Cluster XML
                elements.Add(
                    new XElement("dataset",
                        new XAttribute("macro", $"{modPrefix}_CL_c{cluster.Id:D3}_macro"),
                        new XElement("properties",
                            new XElement("identification",
                                new XAttribute("name", cluster.Name),
                                new XAttribute("description", cluster.Description ?? string.Empty),
                                new XAttribute("image", "enc_cluster01") // By default point to img of cluster01
                            ),
                            clusterFactionLogicTag,
                            new XElement("system")
                        )
                    )
                );

                // Add each Sector inside its Cluster
                foreach (Sector sector in cluster.Sectors)
                {
                    if (sector.AllowRandomAnomalies)
                    {
                        if (string.IsNullOrWhiteSpace(sector.Tags))
                        {
                            sector.Tags = "allowrandomanomaly";
                        }
                        else if (!sector.Tags.Contains("allowrandomanomaly"))
                        {
                            sector.Tags = sector.Tags.TrimEnd() + " allowrandomanomaly";
                        }
                    }

                    XElement areaElement = new("area",
                        new XAttribute("sunlight", sector.Sunlight.ToString("0.0", CultureInfo.InvariantCulture)),
                        new XAttribute("economy", sector.Economy.ToString("0.0", CultureInfo.InvariantCulture)),
                        new XAttribute("security", sector.Security.ToString("0.0", CultureInfo.InvariantCulture)),
                        clusterFactionLogicTag == null ? AddFactionLogic(sector: sector) : null,
                        string.IsNullOrWhiteSpace(sector.Tags) ? null : new XAttribute("tags", sector.Tags)
                    );

                    elements.Add(
                        new XElement("dataset",
                            new XAttribute("macro", $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro"),
                            new XElement("properties",
                                new XElement("identification",
                                    new XAttribute("name", sector.Name),
                                    new XAttribute("description", sector.Description ?? string.Empty),
                                    new XAttribute("image", "enc_cluster01") // By default point to img of cluster01
                                ),
                                areaElement,
                                new XElement("system")
                            )
                        )
                    );
                }
            }

            return elements;
        }

        private static XObject AddFactionLogic(Cluster cluster = null, Sector sector = null)
        {
            if (cluster != null)
            {
                if (cluster.Sectors.All(a => a.DisableFactionLogic) ||
                    cluster.Sectors.All(a => !a.DisableFactionLogic))
                {
                    bool disableFactionLogic = cluster.Sectors[0].DisableFactionLogic;
                    return new XElement("area",
                        new XAttribute("factionlogic", (!disableFactionLogic).ToString().ToLower())
                    );
                }
            }
            else if (sector != null)
            {
                return new XAttribute("factionlogic", (!sector.DisableFactionLogic).ToString().ToLower());
            }
            return null;
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
