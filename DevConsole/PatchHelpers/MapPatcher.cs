using X4SectorCreator;
using X4SectorCreator.Helpers;

namespace DevConsole.PatchHelpers
{
    /// <summary>
    /// Helper to patch all the vanilla map files and dlcs into one file (since the filename differences mess x4 customizer up)
    /// </summary>
    internal static class MapPatcher
    {
        public static void Patch()
        {
            Console.WriteLine("Enter x4 game directory path: ");
            var path = Console.ReadLine();

            (string prefix, string path)[] directories =
            [
                (null, GetDirectory(path, null)),
                ("dlc4_", GetDirectory(path, SectorMapForm.DlcMapping["Split Vendetta"])),
                ("dlc_pirate_", GetDirectory(path, SectorMapForm.DlcMapping["Tides Of Avarice"])),
                ("dlc_terran_", GetDirectory(path, SectorMapForm.DlcMapping["Cradle Of Humanity"])),
                ("dlc_boron_", GetDirectory(path, SectorMapForm.DlcMapping["Kingdom End"])),
                ("dlc7_", GetDirectory(path, SectorMapForm.DlcMapping["Timelines"])),
                ("dlc_mini_01_", GetDirectory(path, SectorMapForm.DlcMapping["Hyperion Pack"])),
            ];

            var vanillaFilesPath = directories.FirstOrDefault(a => a.prefix == null);
            var vanillaFiles = Directory.GetFiles(vanillaFilesPath.path, "*.xml", SearchOption.AllDirectories)
                .ToDictionary(a => a, a => new XmlPatcher(a));

            // Dlc files
            foreach (var directory in directories.Where(a => a.prefix != null))
            {
                var files = Directory.GetFiles(directory.path, "*.xml", SearchOption.AllDirectories);

                foreach (var vanillaFile in vanillaFiles)
                {
                    foreach (var file in files)
                    {
                        if (Path.GetFileName(file).EndsWith(Path.GetFileName(vanillaFile.Key), StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Patching " + Path.GetFileName(file) + " onto " + Path.GetFileName(vanillaFile.Key));
                            vanillaFile.Value.ApplyPatch(file);
                            break;
                        }
                    }
                }
            }

            if (!Directory.Exists("vanillamapfiles"))
                Directory.CreateDirectory("vanillamapfiles");

            foreach (var vanillaFile in vanillaFiles)
            {
                vanillaFile.Value.Save(Path.Combine("vanillamapfiles", Path.GetFileName(vanillaFile.Key)));
            }
        }

        private static string GetDirectory(string path, string dlc)
        {
            return dlc == null ? $"{path}/maps/xu_ep2_universe/" : $"{path}/extensions/{dlc}/maps/xu_ep2_universe/";
        }
    }
}
