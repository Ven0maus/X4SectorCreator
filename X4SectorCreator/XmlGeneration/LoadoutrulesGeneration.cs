using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class LoadoutrulesGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            var loadoutElement = CollectPoliceShips();
            if (loadoutElement == null) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    loadoutElement
                )
            );

            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/loadoutrules.xml")));
        }

        private static XElement CollectPoliceShips()
        {
            var mainElement = new XElement("add", new XAttribute("sel", "//rules/unit/macros"));
            int count = 0;
            foreach (var faction in FactionsForm.AllCustomFactions)
            {
                var policeMacro = GetPoliceMacroByRace(faction.Value.Primaryrace);
                if (policeMacro == null) continue; // xenon & boron don't have one so skip these races

                var element = new XElement("macro",
                    new XAttribute("category", "police"),
                    new XAttribute("mk", "1"),
                    new XAttribute("macro", policeMacro),
                    new XAttribute("factions", faction.Value.Id)    
                );
                mainElement.Add(element);
                count++;
            }
            return count == 0 ? null : mainElement;
        }

        private static string GetPoliceMacroByRace(string race)
        {
            return race.ToLower() switch
            {
                "argon" => "ship_arg_xs_police_01_a_macro",
                "paranid" => "ship_par_xs_police_01_a_macro",
                "teladi" => "ship_tel_xs_police_01_a_macro",
                "split" => "ship_spl_xs_police_01_a_macro",
                "terran" => "ship_ter_xs_police_01_a_macro",
                _ => null,
            };
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
