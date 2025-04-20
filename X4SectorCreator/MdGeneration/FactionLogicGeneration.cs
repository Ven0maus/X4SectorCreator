using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.MdGeneration
{
    internal static class FactionLogicGeneration
    {
        public static void Generate(string modPrefix, string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            var mainCue = GetOrCreateMainCue(modPrefix);

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    mainCue
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/factionlogic.xml")));
        }

        private static XElement GetOrCreateMainCue(string modPrefix)
        {
            XElement mainCue;
            XElement cuesCue;
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                var managerCue = CreateManagerCue(modPrefix, out cuesCue);
                mainCue = new XElement("add", new XAttribute("sel", "/mdscript/cues"));
                mainCue.Add(managerCue);
            }
            else
            {
                mainCue = cuesCue = new XElement("add", new XAttribute("sel", "//cue[@name='FactionLogicManagers']/cues"));
            }

            var setupCue = CreateSetupCue(modPrefix);
            cuesCue.Add(setupCue);
            cuesCue.Add(CreateFactionCues());

            return mainCue;
        }

        private static XElement CreateManagerCue(string modPrefix, out XElement cuesCue)
        {
            cuesCue = new XElement("cues");
            var managerCue = new XElement("cue",
                new XAttribute("name", $"FactionLogicManagersX4SectorCreator{modPrefix}"),
                new XAttribute("mapeditor", "false"),
                new XAttribute("version", "3"),
                new XElement("conditions",
                    new XElement("event_cue_signalled",
                    new XAttribute("cue", "md.Setup.Start")),
                    new XElement("check_value", new XAttribute("value", $"player.galaxy.macro.ismacro.{{macro.{GalaxySettingsForm.GalaxyName}}}"))),
                new XElement("actions",
                    new XElement("set_value",
                    new XAttribute("name", "md.$DefaultShipStrengthTable"),
                    new XAttribute("exact", @"table[
                                {class.ship_xl} = 23,
                                {class.ship_l} = 11,
                                {class.ship_m} = 3,
                                {class.ship_s} = 1]")
                ),
                new XElement("set_value",
                    new XAttribute("name", "md.$DefaultSubordinateStrengthTable"),
                    new XAttribute("exact", @"table[
                                {class.ship_xl} = 21,
                                {class.ship_l} = 9,
                                {class.ship_m} = 3,
                                {class.ship_s} = 1]")
                )),
                cuesCue);
            return managerCue;
        }

        private static XElement CreateSetupCue(string modPrefix)
        {
            var setupCue = new XElement("cue",
                new XAttribute("name", $"SetupX4SectorCreator{modPrefix}CustomFactions"));

            var actionsCue = new XElement("actions");
            setupCue.Add(actionsCue);

            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                var setValueCue = new XElement("set_value",
                    new XAttribute("name", $"md.$FactionData.{{faction.{faction.Id}}}"),
                    new XAttribute("exact", "table[]")
                    );
                actionsCue.Add(setValueCue);
            }
            
            return setupCue;
        }

        private static IEnumerable<XElement> CreateFactionCues()
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
                                new XAttribute("value", "moodlevel.normal")
                            ),
                            new XElement("param",
                                new XAttribute("name", "BaseAvariceLevel"),
                                new XAttribute("value", "moodlevel.high")
                            ),
                            new XElement("param",
                                new XAttribute("name", "BaseLawfulness"),
                                new XAttribute("value", "0.8")
                            ),
                            new XElement("param",
                                new XAttribute("name", "PreferredHQSpaceMacro"),
                                new XAttribute("value", $"macro.{faction.PrefferedHqSpace}")
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
