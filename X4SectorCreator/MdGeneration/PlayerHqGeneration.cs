using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Forms.Galaxy;

namespace X4SectorCreator.MdGeneration
{
    internal static class PlayerHqGeneration
    {
        public static void Generate(string folder, string modPrefix)
        {
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    CreateHQElement(modPrefix)
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"md/XFourSectorCreator_HQ_Setup.xml")));
            }
        }

        private static XElement CreateHQElement(string modPrefix)
        {
            if (string.IsNullOrWhiteSpace(GalaxySettingsForm.HeadQuartersSector))
            {
                // Fall-back
                ProceduralGalaxyForm.SetPlayerHQSector([.. MainForm.Instance.AllClusters.Values]);
            }

            return new XElement("mdscript",
                new XAttribute("name", "XFourSectorCreator_HQ_Setup"),
                new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                new XAttribute(XNamespace.Get("http://www.w3.org/2001/XMLSchema-instance") + "noNamespaceSchemaLocation", "md.xsd"),
                new XElement("cues",
                    new XElement("cue",
                        new XAttribute("name", "Start"),
                        new XAttribute("namespace", "this"),
                        new XAttribute("mapeditor", "false"),
                        new XElement("conditions",
                            new XElement("event_cue_signalled", new XAttribute("cue", "md.Setup.Start")),
                            new XElement("check_value", new XAttribute("value", $"player.galaxy.macro == macro.{GalaxySettingsForm.GalaxyName}_macro"))
                        ),
                        new XElement("actions",
                            new XElement("set_value", new XAttribute("name", "$HQPosition"), new XAttribute("exact", "position.[0km, 0m, 0km]")),
                            new XElement("set_value", new XAttribute("name", "$X"), new XAttribute("min", "$HQPosition.x - 150km"), new XAttribute("max", "$HQPosition.x + 150km")),
                            new XElement("set_value", new XAttribute("name", "$Y"), new XAttribute("min", "$HQPosition.y - 10km"), new XAttribute("max", "$HQPosition.y + 10km")),
                            new XElement("set_value", new XAttribute("name", "$Z"), new XAttribute("min", "$HQPosition.z - 150km"), new XAttribute("max", "$HQPosition.z + 150km")),
                            new XElement("find_sector", new XAttribute("name", "$playersectorstart"), new XAttribute("macro", $"macro.{GalaxySettingsForm.HeadQuartersSector.Replace("PREFIX", modPrefix)}")),
                            new XElement("create_station",
                                new XAttribute("name", "$HQ"),
                                new XAttribute("macro", "macro.station_pla_headquarters_base_01_macro"),
                                new XAttribute("sector", "$playersectorstart"),
                                new XAttribute("constructionplan", "'x4ep1_playerheadquarters_with_dock'"),
                                new XAttribute("owner", "faction.player"),
                                new XAttribute("state", "componentstate.operational"),
                                new XElement("safepos",
                                    new XAttribute("x", "$X"),
                                    new XAttribute("y", "$Y"),
                                    new XAttribute("z", "$Z")
                                )
                            ),
                            new XElement("set_object_name", new XAttribute("object", "$HQ"), new XAttribute("page", "20102"), new XAttribute("line", "2021")),
                            new XElement("remove_value", new XAttribute("name", "$HQPosition")),
                            new XElement("remove_value", new XAttribute("name", "$X")),
                            new XElement("remove_value", new XAttribute("name", "$Y")),
                            new XElement("remove_value", new XAttribute("name", "$Z")),

                            new XElement("do_if", new XAttribute("value", "not $HQ.defencenpc"),
                                new XElement("create_cue_actor", new XAttribute("name", "$computer"), new XAttribute("cue", "this"),
                                    new XElement("select", new XAttribute("tags", "controlpost.defence.tag")),
                                    new XElement("owner", new XAttribute("exact", "faction.player"))
                                ),
                                new XElement("do_if", new XAttribute("value", "$computer"),
                                    new XElement("assign_control_entity",
                                        new XAttribute("object", "$HQ"),
                                        new XAttribute("actor", "$computer"),
                                        new XAttribute("post", "controlpost.defence"),
                                        new XAttribute("transfer", "true")
                                    ),
                                    new XElement("remove_cue_actor", new XAttribute("actor", "$computer"), new XAttribute("cue", "this")),
                                    new XElement("remove_value", new XAttribute("name", "$computer"))
                                )
                            ),

                            new XElement("do_if", new XAttribute("value", "not $HQ.engineer"),
                                new XElement("create_cue_actor", new XAttribute("name", "$computer"), new XAttribute("cue", "this"),
                                    new XElement("select", new XAttribute("tags", "controlpost.engineer.tag")),
                                    new XElement("owner", new XAttribute("exact", "faction.player"))
                                ),
                                new XElement("do_if", new XAttribute("value", "$computer"),
                                    new XElement("assign_control_entity",
                                        new XAttribute("object", "$HQ"),
                                        new XAttribute("actor", "$computer"),
                                        new XAttribute("post", "controlpost.engineer"),
                                        new XAttribute("transfer", "true")
                                    ),
                                    new XElement("remove_cue_actor", new XAttribute("actor", "$computer"), new XAttribute("cue", "this")),
                                    new XElement("remove_value", new XAttribute("name", "$computer"))
                                )
                            ),

                            new XElement("do_if", new XAttribute("value", "not $HQ.paidbuildplot.exists"),
                                new XElement("set_value", new XAttribute("name", "$bp"), new XAttribute("exact", "$HQ.buildplot.max")),
                                new XElement("set_build_plot",
                                    new XAttribute("object", "$HQ"),
                                    new XAttribute("paid", "true"),
                                    new XAttribute("x", "$bp.x"),
                                    new XAttribute("y", "$bp.y"),
                                    new XAttribute("z", "$bp.z")
                                ),
                                new XElement("remove_value", new XAttribute("name", "$bp"))
                            ),

                            new XElement("do_if", new XAttribute("value", "not @$ResearchModule"),
                                new XElement("find_object_component",
                                    new XAttribute("name", "$ResearchModule"),
                                    new XAttribute("macro", "macro.landmarks_player_hq_01_research_macro"),
                                    new XAttribute("object", "$HQ"),
                                    new XAttribute("checkoperational", "false")
                                )
                            ),
                            new XElement("get_room_definition", new XAttribute("macro", "$OfficeRoomMacro"), new XAttribute("tags", "tag.playeroffice")),
                            new XElement("create_dynamic_interior",
                                new XAttribute("roomname", "$OfficeRoom"),
                                new XAttribute("object", "$HQ"),
                                new XAttribute("name", "'{20007,1511}'"),
                                new XAttribute("corridor", "macro.room_arg_corridor_05_macro"),
                                new XAttribute("room", "$OfficeRoomMacro"),
                                new XAttribute("corridorname", "$OfficeCorridor"),
                                new XAttribute("interiorname", "$OfficeInterior"),
                                new XAttribute("persistent", "true"),
                                new XAttribute("module", "$ResearchModule")
                            ),
                            new XElement("get_room_definition",
                                new XAttribute("macro", "$CorridorMacro"),
                                new XAttribute("doors", "$CorridorDoors"),
                                new XAttribute("race", "race.argon"),
                                new XAttribute("tags", "tag.corridor")
                            ),
                            new XElement("create_dynamic_interior",
                                new XAttribute("object", "$HQ"),
                                new XAttribute("corridor", "$CorridorMacro"),
                                new XAttribute("room", "macro.room_gen_war_01_macro"),
                                new XAttribute("door", "$CorridorDoors.{1}"),
                                new XAttribute("name", "'Research Lab'"),
                                new XAttribute("interiorname", "$Interior"),
                                new XAttribute("corridorname", "$Corridor"),
                                new XAttribute("roomname", "$MentorRoom"),
                                new XAttribute("persistent", "true"),
                                new XAttribute("module", "$ResearchModule")
                            ),
                            new XElement("set_dynamic_interior_persistent",
                                new XAttribute("object", "$HQ"),
                                new XAttribute("interior", "$Interior"),
                                new XAttribute("persistent", "true")
                            ),
                            new XElement("do_if", new XAttribute("value", "$MentorRoom and $MentorRoom.dynamicinterior"),
                                new XElement("set_value", new XAttribute("name", "$Interior"), new XAttribute("exact", "$MentorRoom.dynamicinterior")),
                                new XElement("debug_text", new XAttribute("text", "'Setting HQ tank room to persistent'")),
                                new XElement("set_dynamic_interior_persistent",
                                    new XAttribute("object", "$HQ"),
                                    new XAttribute("interior", "$Interior"),
                                    new XAttribute("persistent", "true")
                                )
                            ),

                            new XElement("set_value", new XAttribute("name", "md.Signal_Leaks.Manager.$SuppressSignalLeakGeneration"), new XAttribute("exact", "false")),
                            new XElement("set_value", new XAttribute("name", "player.entity.$x4ep1_hq_research_unlocked")),
                            new XElement("add_encyclopedia_entry", new XAttribute("item", "'research_module_welfare_1_pre'"), new XAttribute("type", "researchables")),
                            new XElement("add_encyclopedia_entry", new XAttribute("item", "'research_module_welfare_1'"), new XAttribute("type", "researchables")),
                            new XElement("remove_value", new XAttribute("name", "$HQ"))
                        )
                    )
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
