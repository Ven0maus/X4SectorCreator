using System.Globalization;
using System.Xml.Linq;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Extensions = X4SectorCreator.Helpers.Extensions;

namespace X4SectorCreator.XmlGeneration
{
    internal static class MapDefaultsGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            IGrouping<string, (string dlc, XElement element)>[] groups = GenerateVanillaChanges(vanillaChanges, clusters)
                .Prepend(GenerateNewClusterElements(modPrefix, clusters))
                .GroupBy(a => a.dlc)
                .ToArray();

            if (groups.Length > 0)
            {
                foreach (IGrouping<string, (string dlc, XElement element)> group in groups)
                {
                    (string dlc, XElement element)[] content = group.Where(a => a.element != null).ToArray();
                    if (content.Length == 0)
                    {
                        continue;
                    }

                    string dlcMapping = group.Key == null ? null : $"{MainForm.Instance.DlcMappings[group.Key]}_";
                    XDocument xmlDocument = new(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement("diff",
                            content.Select(a => a.element)
                        )
                    );

                    if (dlcMapping == null)
                    {
                        xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/mapdefaults.xml")));
                    }
                    else
                    {
                        xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"extensions/{group.Key}/libraries/mapdefaults.xml")));
                    }
                }
            }
        }

        private static (string dlc, XElement element) GenerateNewClusterElements(string modPrefix, List<Cluster> clusters)
        {
            XElement addElement = new("add", new XAttribute("sel", $"/defaults"));
            foreach (Cluster cluster in clusters)
            {
                XObject clusterFactionLogicTag = null;
                if (!cluster.IsBaseGame)
                {
                    clusterFactionLogicTag = AddFactionLogic(cluster: cluster);
                    // Add Cluster XML
                    addElement.Add(
                        new XElement("dataset",
                            new XAttribute("macro", $"{modPrefix}_CL_c{cluster.Id:D3}_macro"),
                            new XElement("properties",
                                new XElement("identification",
                                    new XAttribute("name", $"{{local:{cluster.Name}}}"),
                                    new XAttribute("description", $"{{local:{cluster.Description ?? string.Empty}}}"),
                                    new XAttribute("image", "enc_cluster01") // By default point to img of cluster01
                                ),
                                !string.IsNullOrWhiteSpace(cluster.Soundtrack) ? 
                                    new XElement("sounds", new XElement("music", new XAttribute("ref", cluster.Soundtrack))) : null,
                                clusterFactionLogicTag,
                                new XElement("system")
                            )
                        )
                    );
                }

                // Add each Sector inside its Cluster
                foreach (Sector sector in cluster.Sectors)
                {
                    if (sector.IsBaseGame)
                    {
                        continue;
                    }

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

                    string macro = cluster.IsBaseGame ? $"{modPrefix}_SE_{cluster.BaseGameMapping.CapitalizeFirstLetter()}_s{sector.Id:D3}_macro" :
                        $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";

                    addElement.Add(
                        new XElement("dataset",
                            new XAttribute("macro", macro),
                            new XElement("properties",
                                new XElement("identification",
                                    new XAttribute("name", $"{{local:{sector.Name}}}"),
                                    new XAttribute("description", $"{{local:{cluster.Description ?? string.Empty}}}"),
                                    new XAttribute("image", "enc_cluster01") // By default point to img of cluster01
                                ),
                                areaElement,
                                new XElement("system")
                            )
                        )
                    );
                }
            }

            return (null, addElement.IsEmpty ? null : addElement);
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

        private static IEnumerable<(string dlc, XElement element)> GenerateVanillaChanges(VanillaChanges vanillaChanges, List<Cluster> allClusters)
        {
            List<(string dlc, XElement element)> elements = [];
            foreach (Cluster cluster in vanillaChanges.RemovedClusters)
            {
                string macro = cluster.BaseGameMapping;
                elements.Add((cluster.Dlc, new XElement("remove", new XAttribute("sel", $"//dataset[@macro='{macro}_macro']"))));
            }
            foreach (RemovedSector sector in vanillaChanges.RemovedSectors)
            {
                string macro = $"{sector.VanillaCluster.BaseGameMapping}_{sector.Sector.BaseGameMapping.CapitalizeFirstLetter()}";
                elements.Add((sector.VanillaCluster.Dlc, new XElement("remove", new XAttribute("sel", $"//dataset[@macro='{macro}_macro']"))));
            }
            foreach (ModifiedCluster modification in vanillaChanges.ModifiedClusters)
            {
                Cluster Old = modification.Old;
                Cluster New = modification.New;
                string macro = Old.BaseGameMapping;

                // Identification nodes
                elements.Add((Old.Dlc, CreateReplaceElement(Old.Name, New.Name, macro, "identification", "name", $"{{local:{New.Name}}}")));
                elements.Add((Old.Dlc, CreateReplaceElement(Old.Description, New.Description, macro, "identification", "description", $"{{local:{New.Description}}}")));

                // Soundtrack
                var soundtrackElement = HandleElementSoundtrack(Old.Soundtrack, New.Soundtrack, macro);
                if (soundtrackElement != null)
                    elements.Add((Old.Dlc, soundtrackElement));
            }
            foreach (ModifiedSector modification in vanillaChanges.ModifiedSectors)
            {
                Cluster VanillaCluster = modification.VanillaCluster;
                Sector Old = modification.Old;
                Sector New = modification.New;
                string macro = $"{VanillaCluster.BaseGameMapping}_{Old.BaseGameMapping.CapitalizeFirstLetter()}";

                // Identification nodes
                elements.Add((VanillaCluster.Dlc, CreateReplaceElement(Old.Name, New.Name, macro, "identification", "name", $"{{local:{New.Name}}}")));
                elements.Add((VanillaCluster.Dlc, CreateReplaceElement(Old.Description, New.Description, macro, "identification", "description", $"{{local:{New.Description}}}")));

                // Area nodes
                elements.Add((VanillaCluster.Dlc, CreateReplaceElement(Old.Sunlight.ToString("0.##"), New.Sunlight.ToString("0.##"), macro, "area", "sunlight", New.Sunlight.ToString("0.##"))));
                elements.Add((VanillaCluster.Dlc, CreateReplaceElement(Old.Economy.ToString("0.##"), New.Economy.ToString("0.##"), macro, "area", "economy", New.Economy.ToString("0.##"))));
                elements.Add((VanillaCluster.Dlc, CreateReplaceElement(Old.Security.ToString("0.##"), New.Security.ToString("0.##"), macro, "area", "security", New.Security.ToString("0.##"))));

                // Adjust tags for random anomalies
                if (Old.AllowRandomAnomalies != New.AllowRandomAnomalies)
                {
                    if (New.AllowRandomAnomalies)
                    {
                        if (string.IsNullOrWhiteSpace(New.Tags))
                        {
                            if (New.AllowRandomAnomalies)
                            {
                                New.Tags = "allowrandomanomaly";
                            }
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

                elements.Add((VanillaCluster.Dlc, CreateRemoveOrReplaceElement(Old.Tags, New.Tags, macro, "area", "tags", New.Tags)));

                // Faction logic element
                if (Old.DisableFactionLogic != New.DisableFactionLogic)
                {
                    Cluster newCluster = allClusters.First(a => a.BaseGameMapping.Equals(VanillaCluster.BaseGameMapping, StringComparison.OrdinalIgnoreCase));
                    if (newCluster.Sectors.All(a => a.DisableFactionLogic) ||
                        newCluster.Sectors.All(a => !a.DisableFactionLogic))
                    {
                        // Set on cluster
                        // If the vanilla cluster had its factionlogic disabled, we need to replace instead of add!
                        if (Old.DisableFactionLogic)
                        {
                            // Set on the cluster with replace
                            elements.Add((VanillaCluster.Dlc, CreateReplaceElement(Old.DisableFactionLogic.ToString(), New.DisableFactionLogic.ToString(),
                                VanillaCluster.BaseGameMapping, "area", "factionlogic", New.DisableFactionLogic.ToString().ToLower())));
                        }
                        else
                        {
                            // Set on the cluster with add
                            elements.Add((VanillaCluster.Dlc, CreateAddElement(Old.DisableFactionLogic.ToString(), New.DisableFactionLogic.ToString(),
                                VanillaCluster.BaseGameMapping, "area", "factionlogic", New.DisableFactionLogic.ToString().ToLower())));
                        }
                    }
                    else
                    {
                        // Set on sector
                        // If the vanilla sector had its factionlogic disabled, we need to replace instead of add!
                        if (Old.DisableFactionLogic)
                        {
                            // Set on the sector with replace
                            elements.Add((VanillaCluster.Dlc, CreateReplaceElement(Old.DisableFactionLogic.ToString(), New.DisableFactionLogic.ToString(),
                                macro, "area", "factionlogic", New.DisableFactionLogic.ToString().ToLower())));
                        }
                        else
                        {
                            // Set on the sector with add
                            elements.Add((VanillaCluster.Dlc, CreateAddElement(Old.DisableFactionLogic.ToString(), New.DisableFactionLogic.ToString(),
                                macro, "area", "factionlogic", New.DisableFactionLogic.ToString().ToLower())));
                        }
                    }
                }
            }
            return elements.Where(a => a.element != null);
        }

        private static XElement HandleElementSoundtrack(string old, string @new, string macro)
        {
            // Nothing changed, skip
            if (!Extensions.HasStringChanged(old, @new)) return null;

            // If old was null and new is not, then add new sounds node
            if (string.IsNullOrWhiteSpace(old) && !string.IsNullOrWhiteSpace(@new))
            {
                // Add
                return new XElement("add", new XAttribute("sel", $"//dataset[@macro='{macro}_macro']/properties"), 
                    new XElement("sounds", new XElement("music", new XAttribute("ref", @new))));
            }

            // If old was not null and new is null, then remove sounds node
            if (!string.IsNullOrWhiteSpace(old) && string.IsNullOrWhiteSpace(@new))
            {
                // Remove
                return new XElement("remove", new XAttribute("sel", $"//dataset[@macro='{macro}_macro']/properties/sounds"));
            }

            // If old was not null and new is not null then add replace sounds node
            if (!string.IsNullOrWhiteSpace(old) && !string.IsNullOrWhiteSpace(@new))
            {
                // Replace
                return new XElement("replace", new XAttribute("sel", $"//dataset[@macro='{macro}_macro']/properties/sounds/music/@ref"), @new);
            }

            return null;
        }

        private static XElement CreateReplaceElement(string checkOne, string checkTwo, string macro, string property, string field, string value)
        {
            return Extensions.HasStringChanged(checkOne, checkTwo)
                ? new XElement("replace",
                    new XAttribute("sel", $"//dataset[@macro='{macro}_macro']/properties/{property}/@{field}"),
                    value)
                : null;
        }

        private static XElement CreateRemoveOrReplaceElement(string checkOne, string checkTwo, string macro, string property, string field, string value)
        {
            return Extensions.HasStringChanged(checkOne, checkTwo)
                ? !string.IsNullOrWhiteSpace(value) ? 
                    new XElement("replace",
                        new XAttribute("sel", $"//dataset[@macro='{macro}_macro']/properties/{property}/@{field}"), value) 
                : 
                    new XElement("remove",
                        new XAttribute("sel", $"//dataset[@macro='{macro}_macro']/properties/{property}/@{field}"))
                : null;
        }

        private static XElement CreateAddElement(string checkOne, string checkTwo, string macro, string property, string field, string value)
        {
            return Extensions.HasStringChanged(checkOne, checkTwo)
                ? new XElement("add",
                    new XAttribute("sel", $"//dataset[@macro='{macro}_macro']/properties/{property}/@{field}"),
                    value)
                : null;
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
