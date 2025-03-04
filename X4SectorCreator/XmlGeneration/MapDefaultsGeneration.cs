using System.Globalization;
using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class MapDefaultsGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            var elements = GenerateVanillaChanges(vanillaChanges)
                .Append(GenerateNewClusterElements(modPrefix, clusters))
                .ToArray();

            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                    new XAttribute(xsi + "noNamespaceSchemaLocation", "libraries.xsd"),
                    elements
                )
            );

            if (elements.Length > 0)
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/mapdefaults.xml")));
        }

        private static XElement GenerateNewClusterElements(string modPrefix, List<Cluster> clusters)
        {
            var addElement = new XElement("add",
                new XAttribute("sel", $"/defaults"));

            foreach (Cluster cluster in clusters.Where(a => !a.IsBaseGame))
            {
                XObject clusterFactionLogicTag = AddFactionLogic(cluster: cluster);
                // Add Cluster XML
                addElement.Add(
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

                    addElement.Add(
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

            return addElement.IsEmpty ? null : addElement;
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

        private static IEnumerable<XElement> GenerateVanillaChanges(VanillaChanges vanillaChanges)
        {
            var elements = new List<XElement>();
            foreach (var (Old, New) in vanillaChanges.ModifiedClusters)
            {
                var macro = Old.BaseGameMapping.CapitalizeFirstLetter();
                elements.Add(CreateReplaceElement(Old.Name, New.Name, macro, "name", New.Name));
                elements.Add(CreateReplaceElement(Old.Description, New.Description, macro, "description", New.Description));
            }
            foreach (var (VanillaCluster, Old, New) in vanillaChanges.ModifiedSectors)
            {
                var cluster = VanillaCluster;
                var macro = $"{cluster.BaseGameMapping.CapitalizeFirstLetter()}_{Old.BaseGameMapping.CapitalizeFirstLetter()}";
                elements.Add(CreateReplaceElement(Old.Name, New.Name, macro, "name", New.Name));
                elements.Add(CreateReplaceElement(Old.Description, New.Description, macro, "description", New.Description));
            }
            return elements.Where(a => a != null);
        }

        private static XElement CreateReplaceElement(string checkOne, string checkTwo, string macro, string property, string value)
        {
            if (Extensions.HasStringChanged(checkOne, checkTwo))
            {
                return new XElement("replace",
                    new XAttribute("sel", $"//dataset[@macro='{macro}_macro']/properties/identification/@{property}"),
                    value);
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
