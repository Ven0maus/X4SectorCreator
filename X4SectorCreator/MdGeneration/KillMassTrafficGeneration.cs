using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.MdGeneration
{
    internal static class KillMassTrafficGeneration
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
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/gm_killmasstraffic.xml")));
        }

        private static IEnumerable<XElement> CreateElements()
        {
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                yield return new XElement("add",
                    new XAttribute("sel", "/mdscript/cues/library[@name='Start'][@version='4']/cues/cue[@name='Do_Start_Mission']/cues/cue[@name='MissionAccepted']/cues/cue[@name='ActivateMission'][@version='2']/actions/set_value[@name='$CurKillAmount'][@exact='0']"),
                    new XAttribute("pos", "before"),
                        new XElement("do_if",
                            new XAttribute("value", $"$Location.owner == faction.{faction.Id}"),
                        new XElement("set_value",
                            new XAttribute("name", "$CriminalMasstraffic"),
                            new XAttribute("exact", $"'masstraffic_{faction.Id}_criminal'"))));
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
