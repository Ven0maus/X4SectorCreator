using System.Xml.Linq;

namespace X4SectorCreator.MdGeneration
{
    internal static class FactionGoalInvadeSpaceGeneration
    {
        public static void Generate(string folder)
        {
            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    CreateInvadeFixElement()
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/factiongoal_invade_space.xml")));
        }

        private static XElement CreateInvadeFixElement()
        {
            return new XElement("add", new XAttribute("sel", "/mdscript/cues/cue[@name='Start']/actions/do_all[@counter='$i']/do_if[@value='$AdjacentSectors.{$i}.cluster == $TargetCluster']"),
                new XComment("Fallback to search for gates instead of highways."),
                new XElement("do_if",
                    new XAttribute("value", "not $LocalEntryPoints.count"),
                new XElement("find_gate",
                    new XAttribute("name", "$LocalEntryPoints"),
                    new XAttribute("destination", "$Target"),
                    new XAttribute("space", "$AdjacentSectors.{$i}"),
                    new XAttribute("multiple", "true"))));
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
