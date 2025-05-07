using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.MdGeneration
{
    internal static class FinaliseStationsGeneration
    {
        public static void Generate(string folder)
        {
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        CreateGalaxyReplaceElement()
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/finalisestations.xml")));
            }
        }

        private static XElement CreateGalaxyReplaceElement()
        {
            return new XElement("replace", new XAttribute("sel", "/mdscript/cues/cue[@name='Update_Neighbor_Defense_Station_Loadout']/conditions/check_all/check_value[@value='player.galaxy.macro.ismacro.{macro.xu_ep2_universe_macro}']/@value"),
                $"player.galaxy.macro.ismacro.{{macro.{GalaxySettingsForm.GalaxyName}_macro}}");
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
