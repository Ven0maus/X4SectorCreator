using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.MdGeneration
{
    internal static class FactionLogicGeneration
    {
        public static void Generate(string modPrefix, string folder)
        {
            var factionLogicElement = FactionsForm.AllCustomFactions.Count == 0 ? null : AddFactionsToFactionLogic(modPrefix);

            if (factionLogicElement != null || GalaxySettingsForm.IsCustomGalaxy)
            {
                var customGalaxyElements = CreateCustomGalaxyElements();
                if (customGalaxyElements != null && !customGalaxyElements.Any())
                    customGalaxyElements = null;

                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        customGalaxyElements,
                        factionLogicElement
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/factionlogic.xml")));
            }
        }

        private static IEnumerable<XElement> CreateCustomGalaxyElements()
        {
            if (!GalaxySettingsForm.IsCustomGalaxy) yield break;

            // Galaxy macro
            yield return new XElement("replace", 
                new XAttribute("sel", "//cue[@name='FactionLogicManagers']/conditions/check_value/@value"),
                $"player.galaxy.macro.ismacro.{{macro.{GalaxySettingsForm.GalaxyName}_macro}}");

            // Disable Crisis
            yield return new XElement("remove", new XAttribute("sel", "//cue[@name='XenonFactionLogic_KhaakCrisis']"));

            // Remove kaori trigger
            yield return new XElement("remove", new XAttribute("sel", "/mdscript/cues/cue[@name='FactionLogicManagers']/cues/cue[@name='KaoriFactionLogic_GamestatTrigger']"));
        }

        private static XElement AddFactionsToFactionLogic(string modPrefix)
        {
            return new XElement("add", 
                new XAttribute("sel", "//cue[@name='FactionLogicManagers']/cues"), 
                CreateFactionCues(modPrefix));
        }

        private static IEnumerable<XElement> CreateFactionCues(string modPrefix)
        {
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                yield return new XElement("cue",
                    new XAttribute("name", $"{faction.Id}FactionLogic"),
                    new XElement("cues",
                        new XElement("cue",
                            new XAttribute("name", $"{faction.Id}FactionLogic_Manager"),
                            new XAttribute("ref", "md.FactionLogic.Manager"),
                            new XElement("param",
                                new XAttribute("name", "Faction"),
                                new XAttribute("value", $"faction.{faction.Id}")
                            ),
                            new XElement("param",
                                new XAttribute("name", "BaseAggressionLevel"),
                                new XAttribute("value", $"moodlevel.{faction.AggressionLevel}")
                            ),
                            new XElement("param",
                                new XAttribute("name", "BaseAvariceLevel"),
                                new XAttribute("value", $"moodlevel.{faction.AvariceLevel}")
                            ),
                            new XElement("param",
                                new XAttribute("name", "BaseLawfulness"),
                                new XAttribute("value", faction.Lawfulness)
                            ),
                            new XElement("param",
                                new XAttribute("name", "PreferredHQSpaceMacro"),
                                new XAttribute("value", $"macro.{faction.PrefferedHqSpace.Replace("PREFIX", modPrefix)}")
                            ),
                            new XElement("param",
                                new XAttribute("name", "PreferredHQTypes"),
                                new XAttribute("value", $"[{string.Join(", ", faction.PrefferedHqStationTypes.Select(a => $"'{a}'"))}]")
                            ),
                            new XElement("param",
                                new XAttribute("name", "SatelliteNetworkGoal"),
                                new XAttribute("value", "20")
                            ),
                            new XElement("param",
                                new XAttribute("name", "LasertowerNetworkGoal"),
                                new XAttribute("value", "5")
                            ),
                            new XElement("param",
                                new XAttribute("name", "MinefieldGoalPerSector"),
                                new XAttribute("value", "1"),
                                new XAttribute("comment", "[MGPS * Sectors, 12].min is the maximum amount of Minefields for this faction")
                            ),
                            new XElement("param",
                                new XAttribute("name", "DebugChance"),
                                new XAttribute("value", "0")
                            ),
                            new XElement("param",
                                new XAttribute("name", "DebugChance2"),
                                new XAttribute("value", "0")
                            )
                        )
                    )
                );
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
