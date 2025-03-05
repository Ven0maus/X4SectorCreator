﻿using System.Globalization;
using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class MapDefaultsGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            var elements = GenerateVanillaChanges(vanillaChanges, clusters)
                .Append(GenerateNewClusterElements(modPrefix, clusters))
                .ToArray();

            if (elements.Length > 0)
            {
                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                        new XAttribute(xsi + "noNamespaceSchemaLocation", "libraries.xsd"),
                        elements
                    )
                );

                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/mapdefaults.xml")));
            }
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

        private static IEnumerable<XElement> GenerateVanillaChanges(VanillaChanges vanillaChanges, List<Cluster> allClusters)
        {
            var elements = new List<XElement>();
            foreach (var cluster in vanillaChanges.RemovedClusters)
            {
                var macro = cluster.BaseGameMapping.CapitalizeFirstLetter();
                elements.Add(new XElement("remove", new XAttribute("sel", $"//dataset[@macro='{macro}_macro']")));
            }
            foreach (var sector in vanillaChanges.RemovedSectors)
            {
                var macro = $"{sector.VanillaCluster.BaseGameMapping.CapitalizeFirstLetter()}_{sector.Sector.BaseGameMapping.CapitalizeFirstLetter()}";
                elements.Add(new XElement("remove", new XAttribute("sel", $"//dataset[@macro='{macro}_macro']")));
            }
            foreach (var (Old, New) in vanillaChanges.ModifiedClusters)
            {
                var macro = Old.BaseGameMapping.CapitalizeFirstLetter();

                // Identification nodes
                elements.Add(CreateReplaceElement(Old.Name, New.Name, macro, "identification", "name", New.Name));
                elements.Add(CreateReplaceElement(Old.Description, New.Description, macro, "identification", "description", New.Description));
            }
            foreach (var (VanillaCluster, Old, New) in vanillaChanges.ModifiedSectors)
            {
                var macro = $"{VanillaCluster.BaseGameMapping.CapitalizeFirstLetter()}_{Old.BaseGameMapping.CapitalizeFirstLetter()}";

                // Identification nodes
                elements.Add(CreateReplaceElement(Old.Name, New.Name, macro, "identification", "name", New.Name));
                elements.Add(CreateReplaceElement(Old.Description, New.Description, macro, "identification", "description", New.Description));

                // Area nodes
                elements.Add(CreateReplaceElement(Old.Sunlight.ToString("0.##"), New.Sunlight.ToString("0.##"), macro, "area", "sunlight", New.Sunlight.ToString("0.##")));
                elements.Add(CreateReplaceElement(Old.Economy.ToString("0.##"), New.Economy.ToString("0.##"), macro, "area", "economy", New.Economy.ToString("0.##")));
                elements.Add(CreateReplaceElement(Old.Security.ToString("0.##"), New.Security.ToString("0.##"), macro, "area", "security", New.Security.ToString("0.##")));

                // Adjust tags for random anomalies
                if (Old.AllowRandomAnomalies != New.AllowRandomAnomalies)
                {
                    if (New.AllowRandomAnomalies)
                    {
                        if (string.IsNullOrWhiteSpace(New.Tags))
                        {
                            if (New.AllowRandomAnomalies)
                                New.Tags = "allowrandomanomaly";
                        }
                        else if (!New.Tags.Contains("allowrandomanomaly"))
                        {
                            New.Tags = New.Tags.TrimEnd() + " allowrandomanomaly";
                        }
                    }
                    else
                    {
                        Old.Tags ??= "allowrandomanomaly";
                        if (!string.IsNullOrWhiteSpace(New.Tags))
                        {
                            if (New.Tags.Contains("allowrandomanomaly"))
                            {
                                New.Tags = New.Tags.Replace("allowrandomanomaly", string.Empty).TrimEnd();
                            }
                        }
                    }
                }

                elements.Add(CreateReplaceElement(Old.Tags, New.Tags, macro, "area", "tags", New.Tags));

                // Faction logic element
                if (Old.DisableFactionLogic != New.DisableFactionLogic)
                {
                    var newCluster = allClusters.First(a => a.BaseGameMapping.Equals(VanillaCluster.BaseGameMapping, StringComparison.OrdinalIgnoreCase));
                    if (newCluster.Sectors.All(a => a.DisableFactionLogic) ||
                        newCluster.Sectors.All(a => !a.DisableFactionLogic))
                    {
                        // Set on cluster
                        // If the vanilla cluster had its factionlogic disabled, we need to replace instead of add!
                        if (Old.DisableFactionLogic)
                        {
                            // Set on the cluster with replace
                            elements.Add(CreateReplaceElement(Old.DisableFactionLogic.ToString(), New.DisableFactionLogic.ToString(),
                                VanillaCluster.BaseGameMapping.CapitalizeFirstLetter(), "area", "factionlogic", New.DisableFactionLogic.ToString().ToLower()));
                        }
                        else
                        {
                            // Set on the cluster with add
                            elements.Add(CreateAddElement(Old.DisableFactionLogic.ToString(), New.DisableFactionLogic.ToString(),
                                VanillaCluster.BaseGameMapping.CapitalizeFirstLetter(), "area", "factionlogic", New.DisableFactionLogic.ToString().ToLower()));
                        }
                    }
                    else
                    {
                        // Set on sector
                        // If the vanilla sector had its factionlogic disabled, we need to replace instead of add!
                        if (Old.DisableFactionLogic)
                        {
                            // Set on the sector with replace
                            elements.Add(CreateReplaceElement(Old.DisableFactionLogic.ToString(), New.DisableFactionLogic.ToString(),
                                macro, "area", "factionlogic", New.DisableFactionLogic.ToString().ToLower()));
                        }
                        else
                        {
                            // Set on the sector with add
                            elements.Add(CreateAddElement(Old.DisableFactionLogic.ToString(), New.DisableFactionLogic.ToString(),
                                macro, "area", "factionlogic", New.DisableFactionLogic.ToString().ToLower()));
                        }
                    }
                }
            }
            return elements.Where(a => a != null);
        }

        private static XElement CreateReplaceElement(string checkOne, string checkTwo, string macro, string property, string field, string value)
        {
            if (Extensions.HasStringChanged(checkOne, checkTwo))
            {
                return new XElement("replace",
                    new XAttribute("sel", $"//dataset[@macro='{macro}_macro']/properties/{property}/@{field}"),
                    value);
            }
            return null;
        }

        private static XElement CreateAddElement(string checkOne, string checkTwo, string macro, string property, string field, string value)
        {
            if (Extensions.HasStringChanged(checkOne, checkTwo))
            {
                return new XElement("add",
                    new XAttribute("sel", $"//dataset[@macro='{macro}_macro']/properties/{property}/@{field}"),
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
