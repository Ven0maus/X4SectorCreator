using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.MdGeneration
{
    internal static class SetupGeneration
    {
        public static void Generate(string folder)
        {
            // Will replace smuggler cover factions with our "custom" factions and remove the pirate factions from it
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        CreateSmugglerCoverFix()
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/setup.xml")));
            }
        }

        private static IEnumerable<XElement> CreateSmugglerCoverFix()
        {
            yield return new XElement("replace",
                new XAttribute("sel", "/mdscript/cues/cue[@name='Start']/actions/get_factions_by_tag[@result='global.$smugglercoverfactions']/@tag"),
                "tag.custom"
            );
            yield return new XElement("replace",
                new XAttribute("sel", "/mdscript/cues/cue[@name='Start']/actions/remove_from_list[@name='global.$smugglercoverfactions']"),
                new XElement("get_factions_by_tag",
                    new XAttribute("result", "$piratefactionsxse"),
                    new XAttribute("tag", "tag.pirate")
                )
            );
            yield return new XElement("add",
                new XAttribute("sel", "/mdscript/cues/cue[@name='Start']/actions/create_group[@groupname='global.$IgnoredAbandonedShips']"),
                new XAttribute("pos", "before"),
                new XElement("remove_from_list",
                    new XAttribute("name", "global.$smugglercoverfactions"),
                    new XAttribute("list", "$piratefactionsxse")
                )
            );
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
