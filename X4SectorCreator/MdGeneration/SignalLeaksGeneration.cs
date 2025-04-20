using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.MdGeneration
{
    internal static class SignalLeaksGeneration
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
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/signal_leaks.xml")));
        }

        private static IEnumerable<XElement> CreateElements()
        {
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                yield return new XElement("add",
                    new XAttribute("sel", "/mdscript/cues/cue[@name='Manager'][@version='4']/cues/cue[@name='GM_Transport_Passengers__Trigger'][@version='2']/actions/do_if[@value='$Station.hasrelation.dock.{faction.player}']/do_if[@value='$Page == 30101 and ($TextOffset == 100 or $TextOffset == 200)']/do_all[@exact='1']"),
                        new XElement("do_elseif",
                            new XAttribute("value", $"$Station.owner == faction.{faction.Id}"),
                        new XElement("set_value",
                            new XAttribute("name", "$ClientOwner"),
                            new XAttribute("exact", $"faction.{faction.Id}"))));
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
