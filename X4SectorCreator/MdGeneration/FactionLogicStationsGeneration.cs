using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.MdGeneration
{
    internal static class FactionLogicStationsGeneration
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
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/factionlogic_stations.xml")));
        }

        private static IEnumerable<XElement> CreateElements()
        {
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                foreach (var stationType in faction.StationTypes ?? [])
                {
                    switch (stationType.ToLower())
                    {
                        case "shipyard":
                            yield return GetShipyard(faction);
                            break;
                        case "wharf":
                            yield return GetWharf(faction);
                            break;
                        case "equipmentdock":
                            yield return GetEquipmentDock(faction);
                            break;
                        case "tradestation":
                            yield return GetTradeStation(faction);
                            break;
                    }
                }
            }
        }

        private static XElement GetShipyard(Faction faction)
        {
             return new XElement("add",
                new XAttribute("sel", "/mdscript/cues/library[@name='Manage_Stations']/cues/cue[@name='Process']/cues/cue[@name='Analyse_Stations']/actions/do_elseif[@value='@$DesiredShipyardPatchMarker']"),
                new XAttribute("pos", "after"),
                    new XElement("do_elseif",
                        new XAttribute("value", $"$Faction == faction.{faction.Id}"),
                    new XElement("set_value",
                        new XAttribute("name", "$DesiredShipyards"),
                        new XAttribute("exact", faction.DesiredShipyards))));
        }

        private static XElement GetWharf(Faction faction)
        {
            return new XElement("add",
               new XAttribute("sel", "/mdscript/cues/library[@name='Manage_Stations']/cues/cue[@name='Process']/cues/cue[@name='Analyse_Stations']/actions/do_elseif[@value='@$DesiredWharfPatchMarker']"),
               new XAttribute("pos", "after"),
                   new XElement("do_elseif",
                       new XAttribute("value", $"$Faction == faction.{faction.Id}"),
                   new XElement("set_value",
                       new XAttribute("name", "$DesiredWharfs"),
                       new XAttribute("exact", faction.DesiredWharfs))));
        }

        private static XElement GetEquipmentDock(Faction faction)
        {
            return new XElement("add",
               new XAttribute("sel", "/mdscript/cues/library[@name='Manage_Stations']/cues/cue[@name='Process']/cues/cue[@name='Analyse_Stations']/actions/do_elseif[@value='@$DesiredEquipmentDockPatchMarker']"),
               new XAttribute("pos", "after"),
                   new XElement("do_elseif",
                       new XAttribute("value", $"$Faction == faction.{faction.Id}"),
                   new XElement("set_value",
                       new XAttribute("name", "$DesiredEquipmentDocks"),
                       new XAttribute("exact", faction.DesiredEquipmentDocks))));
        }

        private static XElement GetTradeStation(Faction faction)
        {
            return new XElement("add",
               new XAttribute("sel", "/mdscript/cues/library[@name='Manage_Stations']/cues/cue[@name='Process']/cues/cue[@name='Analyse_Stations']/actions/do_elseif[@value='[faction.argon, faction.antigone, faction.hatikvah, faction.paranid, faction.holyorder].indexof.{$Faction}']"),
               new XAttribute("pos", "after"),
                   new XElement("do_elseif",
                       new XAttribute("value", $"$Faction == faction.{faction.Id}"),
                   new XElement("set_value",
                       new XAttribute("name", "$DesiredTradeStation"),
                       new XAttribute("exact", faction.DesiredTradeStations))));
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
