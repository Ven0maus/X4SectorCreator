using X4SectorCreator;
using X4SectorCreator.Helpers;

namespace DevConsole.PatchHelpers
{
    /// <summary>
    /// Helper to patch all the vanilla map files and dlcs into one file (since the filename differences mess x4 customizer up)
    /// </summary>
    internal static class MapPatcher
    {
        public static void Patch(string path)
        {
            if (!Directory.Exists("vanillafiles"))
                Directory.CreateDirectory("vanillafiles");

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
            var sourceMapFiles = Directory.GetFiles(vanillaFilesPath.path, "*.xml", SearchOption.AllDirectories)
                .ToDictionary(a => a, a => new XmlPatcher(a));

            // Dlc files
            foreach (var directory in directories.Where(a => a.prefix != null))
            {
                var patchFiles = Directory.GetFiles(directory.path, "*.xml", SearchOption.AllDirectories);

                foreach (var mapFile in sourceMapFiles)
                {
                    foreach (var patchFile in patchFiles)
                    {
                        if (Path.GetFileName(patchFile).EndsWith(Path.GetFileName(mapFile.Key), StringComparison.OrdinalIgnoreCase))
                        {
                            Console.WriteLine("Patching " + Path.GetFileName(patchFile) + " onto " + Path.GetFileName(mapFile.Key));
                            mapFile.Value.ApplyPatch(patchFile);
                            break;
                        }
                    }
                }
            }

            foreach (var vanillaFile in sourceMapFiles)
            {
                vanillaFile.Value.Save(Path.Combine("vanillafiles", Path.GetFileName(vanillaFile.Key)));
            }

            // Also include others
            PatchFile(directories, vanillaFilesPath, "libraries", "region_definitions.xml");
            PatchFile(directories, vanillaFilesPath, "libraries", "god.xml");
        }

        private static void PatchFile(
            (string prefix, string path)[] directories,
            (string prefix, string path) vanillaFilesPath,
            params string[] filePathParts)
        {
            // Build source file path
            var sourceFilePath = Path.Combine(
                Path.GetFullPath(Path.Combine(vanillaFilesPath.path, "..", "..")),
                Path.Combine(filePathParts) // joins the rest of the parts
            );

            var patcher = new XmlPatcher(sourceFilePath);

            foreach (var (prefix, path) in directories.Where(a => a.prefix != null))
            {
                var patchFilePath = Path.Combine(
                    Path.GetFullPath(Path.Combine(path, "..", "..")),
                    Path.Combine(filePathParts)
                );

                if (!File.Exists(patchFilePath))
                    continue;

                patcher.ApplyPatch(patchFilePath);

                Console.WriteLine($"Patching {Path.GetFileName(patchFilePath)} onto {Path.GetFileName(sourceFilePath)}");
            }

            patcher.Save(Path.Combine("vanillafiles", Path.GetFileName(filePathParts.Last())));
        }

        private static string GetDirectory(string path, string dlc)
        {
            return dlc == null ? $"{path}/maps/xu_ep2_universe/" : $"{path}/extensions/{dlc}/maps/xu_ep2_universe/";
        }
    }
}
