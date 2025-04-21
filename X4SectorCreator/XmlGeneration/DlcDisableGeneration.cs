using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class DlcDisableGeneration
    {
        /// <summary>
        /// This method is used to generate MD replacement scripts to disable a lot of the MD scripts that spam logfile with errors on custom galaxy
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="modPrefix"></param>
        public static void Generate(string folder)
        {
            if (GalaxySettingsForm.IsCustomGalaxy || GalaxySettingsForm.DisableAllStorylines)
            {
                // Timelines
                CreateReplacementMdForDlc(folder, SectorMapForm.DlcMapping["Timelines"], "setup_dlc_timelines", "story_research_abandoned_ships", "story_timelines_epilogue");

                // Kingdom End
                CreateReplacementMdForDlc(folder, SectorMapForm.DlcMapping["Kingdom End"], "setup_dlc_boron", "story_boron", "story_boron_prelude");

                // Tides of Avarice
                CreateReplacementMdForDlc(folder, SectorMapForm.DlcMapping["Tides Of Avarice"], "story_pirate_prelude", "story_criminal", "story_research_welfare_2", "story_thefan", "setup_dlc_pirate", "story_research_erlking");

                // Cradle of Humanity
                CreateReplacementMdForDlc(folder, SectorMapForm.DlcMapping["Cradle Of Humanity"], "setup_dlc_terran", "story_covert_operations", "story_hq_discovery", "story_terraforming", "story_terran_core", "story_terran_prelude", "story_yaki", "yaki_supply", "x4ep1_war_terran");

                // Split Vendetta
                CreateReplacementMdForDlc(folder, SectorMapForm.DlcMapping["Split Vendetta"], "setup_dlc_split", "story_split", "x4ep1_war_split");

                // Hyperion Pack
                CreateReplacementMdForDlc(folder, SectorMapForm.DlcMapping["Hyperion Pack"], "setup_dlc_mini_01", "story_hyperion", "gs_hyperion");

                // Base Game
                List<string> baseGameMds = ["story_buccaneers", "story_diplomacy_intro", "story_paranid", "story_research_welfare_1", "story_ventures", "terraforming"];

                // Exceptional cases where macro checks happen
                if (GalaxySettingsForm.IsCustomGalaxy)
                {
                    baseGameMds.Add("khaak_activity");
                }
                CreateReplacementMdForBaseGame(folder, baseGameMds);
            }
        }

        private static void CreateReplacementMdForDlc(string folder, string dlc, params string[] filenames)
        {
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    new XElement("replace", new XAttribute("sel", "//mdscript/cues"),
                        new XElement("cues") // This creates an empty <cues> element
                    )
                )
            );

            foreach (string filename in filenames)
            {
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"extensions/{dlc}/md/{filename}.xml")));
            }
        }

        private static void CreateReplacementMdForBaseGame(string folder, IEnumerable<string> filenames)
        {
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    new XElement("replace", new XAttribute("sel", "//mdscript/cues"),
                        new XElement("cues") // This creates an empty <cues> element
                    )
                )
            );

            foreach (string filename in filenames)
            {
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/{filename}.xml")));
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
