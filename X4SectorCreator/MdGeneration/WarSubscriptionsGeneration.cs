using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.MdGeneration
{
    internal static class WarSubscriptionsGeneration
    {
        public static void Generate(string folder)
        {
            if (!GalaxySettingsForm.IsCustomGalaxy)
            {
                return;
            }

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    new XElement("replace", 
                        new XAttribute("sel", "/mdscript/cues/cue[@name='Start']/conditions/check_value/@value"),
                        $"player.galaxy.macro == macro.{GalaxySettingsForm.GalaxyName}_macro"
                    )
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/x4ep1_war_subscriptions.xml")));
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
