using System.Globalization;
using System.Xml.Linq;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;
using Extensions = X4SectorCreator.Helpers.Extensions;

namespace X4SectorCreator.XmlGeneration
{
    internal static class MapDefaultsGeneration
    {
        public static readonly IReadOnlyDictionary<string, string> SpaceTypeMappings = new Dictionary<string, string>()
        {
            { "Clear Space", "{1042,12011}(Clear Space)" },
            { "Thin Nebula", "{1042,12021}(Thin Nebula)" },
            { "Nebula", "{1042,12031}(Nebula)" },
            { "Asteroids", "{1042,12041}(Asteroids)" },
            { "Protoplanetary Disc", "{1042,12051}(Protoplanetary Disc)" },
            { "Heavy Radiation", "{1042,12061}(Heavy Radiation)" },
            { "Emission Nebula", "{1042,12071}(Emission Nebula)" }
        };

        public static readonly IReadOnlyDictionary<string, string> AtmosphereTypeMappings = new Dictionary<string, string>()
        {
            { "None", "{1042,10011}(None)" },
            { "Methane", "{1042,11011}(Methane)" },
            { "Hydrogen", "{1042,11021}(Hydrogen)" },
            { "Oxygen", "{1042,11031}(Oxygen)" },
            { "Nitrogen", "{1042,11041}(Nitrogen)" },
            { "Helium", "{1042,11051}(Helium)" },
            { "Carbon Dioxide", "{1042,11081}(Carbon Dioxide)" },
            { "Sulphur Dioxide", "{1042,11091}(Sulphur Dioxide)" },
            { "Methane/Hydrogen", "{1042,11061}(Methane/Hydrogen)" },
            { "Nitrogen/Oxygen", "{1042,11071}(Nitrogen/Oxygen)" },
            { "Nitrogen/Hydrogen", "{1042,11101}(Nitrogen/Hydrogen)" },
            { "Hydrogen/Helium", "{1042,11111}(Hydrogen/Helium)" }
        };

        public static readonly IReadOnlyDictionary<string, string> SunTypeMappings = new Dictionary<string, string>()
        {
            { "Red Dwarf", "{1042,13011}(Red Dwarf)" },
            { "Yellow Dwarf", "{1042,13021}(Yellow Dwarf)" },
            { "Orange Dwarf", "{1042,13031}(Orange Dwarf)" },
            { "White Dwarf", "{1042,13041}(White Dwarf)" },
            { "Blue Dwarf", "{1042,13051}(Blue Dwarf)" },
            { "Brown Dwarf", "{1042,13061}(Brown Dwarf)" },
            { "Black Dwarf", "{1042,13071}(Black Dwarf)" },
            { "Red Giant", "{1042,13081}(Red Giant)" },
            { "Yellow Giant", "{1042,13091}(Yellow Giant)" },
            { "Orange Giant", "{1042,13101}(Orange Giant)" },
            { "White Giant", "{1042,13111}(White Giant)" },
            { "Blue Giant", "{1042,13121}(Blue Giant)" },
            { "Red Supergiant", "{1042,13131}(Red Supergiant)" },
            { "Yellow Supergiant", "{1042,13141}(Yellow Supergiant)" },
            { "Orange Supergiant", "{1042,13151}(Orange Supergiant)" },
            { "White Supergiant", "{1042,13161}(White Supergiant)" },
            { "Blue Supergiant", "{1042,13171}(Blue Supergiant)" },
            { "Neutron Star", "{1042,13181}(Neutron Star)" },
            { "Black Hole", "{1042,13191}(Black Hole)" },
        };

        public static readonly IReadOnlyDictionary<string, string> PlanetTypeMappings = new Dictionary<string, string>()
        {
            { "Super Earth", "{1042,14011}(Super Earth)" },
            { "Ringed Super Earth", "{1042,14021}(Ringed Super Earth)" },
            { "Terrestrial", "{1042,14031}(Terrestrial)" },
            { "Ringed Terrestrial", "{1042,14041}(Ringed Terrestrial)" },
            { "Moon", "{1042,14051}(Moon)" },
            { "Planet", "{1042,14061}(Planet)" },
            { "Ringed Planet", "{1042,14071}(Ringed Planet)" },
            { "Gas Giant", "{1042,14081}(Gas Giant)" },
            { "Ringed Gas Giant", "{1042,14091}(Ringed Gas Giant)" },
            { "Earth Analog", "{1042,14101}(Earth Analog)" },
            { "Ringed Earth Analog", "{1042,14111}(Ringed Earth Analog)" },
            { "Dwarf Planet", "{1042,14121}(Dwarf Planet)" },
            { "Ice Giant", "{1042,14131}(Ice Giant)" },
            { "Ringed Ice Giant", "{1042,14141}(Ringed Ice Giant)" },
        };

        public static readonly IReadOnlyDictionary<string, string> GeologyTypeMappings = new Dictionary<string, string>()
        {
            { "None", "{1042,10011}(None)" },
            { "Tundra", "{1042,15011}(Tundra)" },
            { "Mountains", "{1042,15021}(Mountains)" },
            { "Volcanic", "{1042,15031}(Volcanic)" },
            { "Xenon Structures", "{1042,15041}(Xenon Structures)" },
            { "Rocks&Ice", "{1042,15051}(Rocks/Ice)" },
            { "Canyons", "{1042,15061}(Canyons)" },
            { "Rocks", "{1042,15071}(Rocks)" },
            { "Ice", "{1042,15081}(Ice)" },
            { "Craters", "{1042,15091}(Craters)" },
            { "Barren", "{1042,15101}(Barren)" },
            { "Desert", "{1042,15111}(Desert)" },
            { "Swamp", "{1042,15121}(Swamp)" },
            { "Forest", "{1042,15131}(Forest)" },
            { "Ocean", "{1042,15141}(Ocean)" },
            { "Polar", "{1042,15151}(Polar)" },
        };

        public static readonly IReadOnlyDictionary<string, string> SettlementTypeMappings = new Dictionary<string, string>()
        {
            { "None", "{1042,10011}(None)" },
            { "Uninhabited", "{1042,16011}(Uninhabited)" },
            { "Mining Colonies", "{1042,16021}(Mining Colonies)" },
            { "Research Outpost", "{1042,16031}(Research Outpost)" },
            { "Multiple Cities", "{1042,16041}(Multiple Cities)" },
            { "Densely Populated", "{1042,16051}(Densely Populated)" },
            { "Xenon Structures", "{1042,16061}(Xenon Structures)" },
            { "Megalopolis", "{1042,16071}(Megalopolis)" },
            { "Sky City", "{1042,16081}(Sky City)" },
            { "Agora Ouranos", "{1042,16091}(Agora Ouranos)" },
            { "Small Settlements", "{1042,16101}(Small Settlements)" },
        };

        public static readonly IReadOnlyDictionary<string, IReadOnlyDictionary<string, string>> ReverseLookupStarSystemInfos =
            new Dictionary<string, IReadOnlyDictionary<string, string>>
            {
                { nameof(SpaceTypeMappings), SpaceTypeMappings.ToDictionary(a => a.Value, a => a.Key) },
                { nameof(AtmosphereTypeMappings), AtmosphereTypeMappings.ToDictionary(a => a.Value, a => a.Key) },
                { nameof(SunTypeMappings), SunTypeMappings.ToDictionary(a => a.Value, a => a.Key) },
                { nameof(PlanetTypeMappings), PlanetTypeMappings.ToDictionary(a => a.Value, a => a.Key) },
                { nameof(GeologyTypeMappings), GeologyTypeMappings.ToDictionary(a => a.Value, a => a.Key) },
                { nameof(SettlementTypeMappings), SettlementTypeMappings.ToDictionary(a => a.Value, a => a.Key) },
            };

        public static string GetReverseLookup(string dict, string value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (ReverseLookupStarSystemInfos.TryGetValue(dict, out var dictionary) &&
                dictionary.TryGetValue(value, out var key))
                return key;
            throw new Exception($"Unable to find key \"{value}\" in dictionary \"{dict}\".");
        }

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

                    XElement resourceAreasElement = null;
                    if (sector.ResourceAreas.Count > 0)
                    {
                        resourceAreasElement = new("resourceareas");
                        foreach (var ra in sector.ResourceAreas)
                        {
                            resourceAreasElement.Add(new XElement("resourcearea",
                                new XAttribute("amount", ra.Amount),
                                new XAttribute("ref", $"sphere_{ra.Size}_{ra.Ware}_{ra.Yield}_{ra.Speed}")));
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
                                resourceAreasElement,
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

                // Resource areas
                HandleResourceAreas(Old, New, VanillaCluster, elements, macro);

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

        private static void HandleResourceAreas(
            Sector old,
            Sector @new,
            Cluster cluster,
            List<(string dlc, XElement element)> elements,
            string macro)
        {
            static string Key(Resource r)
                => $"{r.Ware}|{r.Yield}|{r.Size}|{r.Speed}|{r.Amount}";

            var oldCounts = old.ResourceAreas
                .GroupBy(Key)
                .ToDictionary(g => g.Key, g => g.Count());

            var newCounts = @new.ResourceAreas
                .GroupBy(Key)
                .ToDictionary(g => g.Key, g => g.Count());

            // ADDED
            foreach (var ra in @new.ResourceAreas)
            {
                var key = Key(ra);

                if (!oldCounts.TryGetValue(key, out var count) || count == 0)
                {
                    elements.Add((
                        cluster.Dlc,
                        new XElement("add",
                            new XAttribute("sel",
                                $"//dataset[@macro='{macro}_macro']/properties/resourceareas"),
                            new XElement("resourcearea",
                                new XAttribute("ref",
                                    $"sphere_{ra.Size}_{ra.Ware}_{ra.Yield}_{ra.Speed}"),
                                new XAttribute("amount", ra.Amount)
                            )
                        )
                    ));
                }
                else
                {
                    oldCounts[key]--;
                }
            }

            // REMOVED
            foreach (var ra in old.ResourceAreas)
            {
                var key = Key(ra);

                if (!newCounts.TryGetValue(key, out var count) || count == 0)
                {
                    elements.Add((
                        cluster.Dlc,
                        new XElement("remove",
                            new XAttribute("sel",
                                $"//dataset[@macro='{macro}_macro']/properties/resourceareas/resourcearea[@ref='sphere_{ra.Size}_{ra.Ware}_{ra.Yield}_{ra.Speed}' and @amount='{ra.Amount}']")
                        )
                    ));
                }
                else
                {
                    newCounts[key]--;
                }
            }
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
