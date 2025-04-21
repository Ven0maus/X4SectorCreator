using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.MdGeneration
{
    internal static class FactionSetupGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            foreach (var faction in FactionsForm.AllCustomFactions.Values) 
            {
                var cuesElement = new XElement("cues");
                InitializeCueForFaction(cuesElement, faction);

                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("mdscript",
                        new XAttribute("name", $"Setup_Faction_{faction.Id}"),
                        new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                        new XAttribute(xsi + "noNamespaceSchemaLocation", "md.xsd"),
                        cuesElement
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/setup_faction_{faction.Id}.xml")));
            }
        }

        private static void InitializeCueForFaction(XElement parent, Faction faction)
        {
            var setupCue = new XElement("cue",
                    new XAttribute("name", "Setup"),
                    new XAttribute("namespace", "this"),
                    new XElement("conditions",
                        new XElement("event_cue_signalled", new XAttribute("cue", "md.Setup.Start"))
                    ),
                    new XElement("actions",
                        new XElement("do_if", new XAttribute("value", "global.$smugglercoverfactions?"),
                            new XElement("do_if", new XAttribute("value", $"not global.$smugglercoverfactions.indexof.{{faction.{faction.Id}}}"),
                                new XElement("append_to_list",
                                    new XAttribute("name", "global.$smugglercoverfactions"),
                                    new XAttribute("exact", $"faction.{faction.Id}")
                                )
                            )
                        )
                    )
                );
            parent.Add(setupCue);

            var cueOne = new XElement("cue",
                new XAttribute("name", "Start"),
                new XAttribute("namespace", "this"),
                new XElement("actions",
                    new XElement("include_actions", new XAttribute("ref", "Start_Actions")),
                    new XElement("cancel_cue", new XAttribute("cue", "Game_Loaded"))
                )
            );
            parent.Add(cueOne);

            var cueTwo = new XElement("cue",
                new XAttribute("name", "Game_Loaded"),
                new XAttribute("namespace", "this"),
                new XElement("conditions",
                    new XElement("event_game_loaded")
                ),
                new XElement("actions",
                    new XElement("include_actions", new XAttribute("ref", "Start_Actions")),
                    new XElement("cancel_cue", new XAttribute("cue", "Start"))
                )
            );
            parent.Add(cueTwo);

            var libCue = new XElement("library",
                new XAttribute("name", "Start_Actions"),
                new XElement("actions",
                    new XElement("do_if", new XAttribute("value", "not md.$EquipmentTable?"),
                        new XElement("set_value",
                            new XAttribute("name", "md.$EquipmentTable"),
                            new XAttribute("exact", "table[]")
                        )
                    ),
                    new XElement("get_ware_definition",
                        new XAttribute("result", $"md.$EquipmentTable.{{faction.{faction.Id}}}"),
                        new XAttribute("faction", $"faction.{faction.Id}"),
                        new XAttribute("flags", "equipment")
                    ),
                    new XElement("do_if", new XAttribute("value", "not md.$FactionData?"),
                        new XElement("set_value",
                            new XAttribute("name", "md.$FactionData"),
                            new XAttribute("exact", "table[]")
                        )
                    ),
                    new XElement("do_if", new XAttribute("value", $"not md.$FactionData.{{faction.{faction.Id}}}?"),
                        new XElement("set_value",
                            new XAttribute("name", $"md.$FactionData.{{faction.{faction.Id}}}"),
                            new XAttribute("exact", "table[]")
                        )
                    )
                )
            );
            parent.Add(libCue);

            var cueThree = new XElement("cue",
                new XAttribute("name", "Update_HQ_Plot_Data"),
                new XElement("conditions",
                    new XElement("event_cue_completed", new XAttribute("cue", "md.X4Ep1_Mentor_Subscriptions.Start"))
                ),
                new XElement("actions",
                    new XElement("append_list_elements",
                        new XAttribute("name", "md.X4Ep1_Mentor_Subscriptions.Start.$SignalLeakStationFactions"),
                        new XAttribute("other", $"[faction.{faction.Id}]")
                    )
                )
            );
            parent.Add(cueThree);
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
