using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.MdGeneration
{
    internal static class FactionLogicEconomyGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    CreateElements()
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/factionlogic_economy.xml")));
        }

        private static IEnumerable<XElement> CreateElements()
        {
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                yield return new XElement("add",
                    new XAttribute("sel", "/mdscript/cues/library[@name='Econ_Manager']/cues/cue[@name='Request_Production_Module']/actions/set_value[@name='$DebugText']"),
                    new XAttribute("pos", "before"),
                        new XElement("do_if",
                            new XAttribute("value", $"$Faction == faction.{faction.Id}"),
                        new XElement("set_value",
                            new XAttribute("name", "$CheckProductionCompatibility"),
                            new XAttribute("exact", $"true"))));
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
