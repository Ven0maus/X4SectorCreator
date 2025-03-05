﻿using System.Xml.Linq;

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
                CreateReplacementMdForDlc(folder, "ego_dlc_timelines", "setup_dlc_timelines", "story_research_abandoned_ships", "story_timelines_epilogue");

                // Kingdom End
                CreateReplacementMdForDlc(folder, "ego_dlc_boron", "setup_dlc_boron", "story_boron", "story_boron_prelude");

                // Tides of Avarice
                CreateReplacementMdForDlc(folder, "ego_dlc_pirate", "story_pirate_prelude", "story_criminal", "story_research_welfare_2", "story_thefan", "setup_dlc_pirate", "story_research_erlking");

                // Cradle of Humanity
                CreateReplacementMdForDlc(folder, "ego_dlc_terran", "setup_dlc_terran", "story_covert_operations", "story_hq_discovery", "story_terraforming", "story_terran_core", "story_terran_prelude", "story_yaki", "yaki_supply", "x4ep1_war_terran");

                // Split Vendetta
                CreateReplacementMdForDlc(folder, "ego_dlc_split", "setup_dlc_split", "story_split", "x4ep1_war_split");

                // Base Game
                var baseGameMds = new List<string> { "story_buccaneers", "story_diplomacy_intro", "story_paranid", "story_research_welfare_1", "story_ventures", "terraforming", "x4ep1_war_subscriptions" };

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
            var xmlDocument = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    new XElement("replace", new XAttribute("sel", "//mdscript/cues"),
                        new XElement("cues") // This creates an empty <cues> element
                    )
                )
            );

            foreach (var filename in filenames)
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"extensions/{dlc}/md/{filename}.xml")));
        }

        private static void CreateReplacementMdForBaseGame(string folder, IEnumerable<string> filenames)
        {
            var xmlDocument = new XDocument(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    new XElement("replace", new XAttribute("sel", "//mdscript/cues"),
                        new XElement("cues") // This creates an empty <cues> element
                    )
                )
            );

            foreach (var filename in filenames)
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/{filename}.xml")));
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
