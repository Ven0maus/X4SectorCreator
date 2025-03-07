using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class GameStartsGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            // Generate a custom gamestart for custom galaxy
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                XElement customGameStart = GenerateCustomGameStart(modPrefix, clusters);
                if (customGameStart != null)
                {
                    XDocument xmlDocument = new(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement("diff",
                            new XElement("replace",
                            new XAttribute("sel", "//gamestarts"),
                            customGameStart)
                        )
                    );
                    xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/gamestarts.xml")));
                    return;
                }
            }

            List<XElement> generatedElements = GenerateVanillaChanges(vanillaChanges).ToList();

            if (generatedElements.Count > 0)
            {
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        generatedElements
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/gamestarts.xml")));
            }
        }

        private static XElement GenerateCustomGameStart(string modPrefix, List<Cluster> clusters)
        {
            Cluster firstCluster = clusters.OrderBy(a => a.Name).FirstOrDefault(a => !a.IsBaseGame);
            if (firstCluster == null)
            {
                return null;
            }

            Sector sector = firstCluster.Sectors.FirstOrDefault();
            if (sector == null)
            {
                return null;
            }

            string sectorMacro = $"{modPrefix}_SE_c{firstCluster.Id:D3}_s{sector.Id:D3}_macro";
            XElement gameStartElement = new("gamestart",
                new XAttribute("id", $"{modPrefix}_sectorcreator_customgalaxy"),
                new XAttribute("name", $"{GalaxySettingsForm.GalaxyName}"),
                new XAttribute("description", $"{GalaxySettingsForm.GalaxyName} entrypoint."),
                new XAttribute("image", "gamestart_1"),
                new XElement("location",
                    new XAttribute("galaxy", $"{modPrefix}_{GalaxySettingsForm.GalaxyName}_macro"),
                    new XAttribute("sector", $"{sectorMacro.ToLower()}"),
                    new XElement("position",
                        new XAttribute("x", "0"),
                        new XAttribute("y", "0"),
                        new XAttribute("z", "0")
                    ),
                    new XElement("rotation",
                        new XAttribute("yaw", "0"),
                        new XAttribute("pitch", "0"),
                        new XAttribute("roll", "0")
                    )
                ),
                new XElement("player",
                    new XAttribute("macro", "character_player_custom_f_asi_macro"),
                    new XAttribute("female", "true"),
                    new XAttribute("money", "75000"),
                    new XAttribute("name", "Jade Miras"),
                    new XElement("ship",
                        new XAttribute("macro", "ship_par_s_fighter_01_a_macro"),
                        new XElement("loadout",
                            new XElement("macros",
                                new XElement("engine", new XAttribute("macro", "engine_par_s_combat_01_mk1_macro"), new XAttribute("path", "../con_engine_01")),
                                new XElement("engine", new XAttribute("macro", "engine_par_s_combat_01_mk1_macro"), new XAttribute("path", "../con_engine_02")),
                                new XElement("engine", new XAttribute("macro", "engine_par_s_combat_01_mk1_macro"), new XAttribute("path", "../con_engine_03")),
                                new XElement("weapon", new XAttribute("macro", "weapon_gen_s_laser_01_mk1_macro"), new XAttribute("path", "../con_weapon_01"), new XAttribute("optional", "true")),
                                new XElement("weapon", new XAttribute("macro", "weapon_gen_s_guided_01_mk1_macro"), new XAttribute("path", "../con_weapon_02"), new XAttribute("optional", "true")),
                                new XElement("shield", new XAttribute("macro", "shield_par_s_standard_01_mk1_macro"), new XAttribute("path", "../con_shield_01"), new XAttribute("optional", "true"))
                            ),
                            new XElement("ammunition",
                                new XElement("ammunition", new XAttribute("macro", "missile_guided_light_mk1_macro"), new XAttribute("exact", "10"), new XAttribute("optional", "true")),
                                new XElement("ammunition", new XAttribute("macro", "eq_arg_satellite_01_macro"), new XAttribute("exact", "5"), new XAttribute("optional", "true")),
                                new XElement("ammunition", new XAttribute("macro", "env_deco_nav_beacon_t1_macro"), new XAttribute("exact", "5"), new XAttribute("optional", "true")),
                                new XElement("ammunition", new XAttribute("macro", "eq_arg_resourceprobe_01_macro"), new XAttribute("exact", "5"), new XAttribute("optional", "true"))
                            ),
                            new XElement("software",
                                new XElement("software", new XAttribute("ware", "software_targetmk1"))
                            ),
                            new XElement("virtualmacros",
                                new XElement("thruster", new XAttribute("macro", "thruster_gen_s_allround_01_mk1_macro"))
                            )
                        )
                    ),
                    new XElement("inventory",
                        new XElement("ware", new XAttribute("ware", "weapon_gen_spacesuit_repairlaser_01_mk1"), new XAttribute("amount", "1")),
                        new XElement("ware", new XAttribute("ware", "software_scannerobjectmk3"), new XAttribute("amount", "1"))
                    ),
                    new XElement("blueprints",
                        new XElement("ware", new XAttribute("ware", "module_arg_dock_m_01_lowtech")),
                        new XElement("ware", new XAttribute("ware", "module_arg_stor_container_s_01")),
                        new XElement("ware", new XAttribute("ware", "module_arg_conn_base_01")),
                        new XElement("ware", new XAttribute("ware", "module_arg_conn_cross_01")),
                        new XElement("ware", new XAttribute("ware", "module_arg_conn_vertical_01")),
                        new XElement("ware", new XAttribute("ware", "module_gen_prod_energycells_01"))
                    ),
                    new XElement("research",
                        new XElement("ware", new XAttribute("ware", "research_radioreceiver")),
                        new XElement("ware", new XAttribute("ware", "research_sensorbooster")),
                        new XElement("ware", new XAttribute("ware", "research_tradeinterface"))
                    ),
                    new XElement("theme", new XAttribute("paint", "painttheme_player_01"))
                )
            );

            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            return new XElement("gamestarts", new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(xsi + "noNamespaceSchemaLocation", "gamestarts.xsd"),
                gameStartElement);
        }

        private static IEnumerable<XElement> GenerateVanillaChanges(VanillaChanges vanillaChanges)
        {
            // If argon prime was removed, remove it from "custom_creative" sector + known sectors
            // This will fix the display in the custom creative gamestart
            if (vanillaChanges.RemovedSectors.Any(a =>
                a.VanillaCluster.BaseGameMapping.Equals("Cluster_14", StringComparison.OrdinalIgnoreCase) &&
                a.Sector.BaseGameMapping.Equals("Sector001", StringComparison.OrdinalIgnoreCase)))
            {
                yield return new XElement("remove", new XAttribute("sel", "/gamestarts/gamestart[@id='custom_creative']/location/@sector"));
                yield return new XElement("remove", new XAttribute("sel", "/gamestarts/gamestart[@id='custom_creative']/player/knownspace/space"));
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
