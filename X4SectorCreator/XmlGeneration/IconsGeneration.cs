using System.IO.Compression;
using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Helpers;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class IconsGeneration
    {
        public static void Generate(string folder, string modName)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    CollectIcons(folder, modName)
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/icons.xml")));
        }

        private static XElement CollectIcons(string folder, string modName)
        {
            var mainElement = new XElement("add", new XAttribute("sel", "/icons"));
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                // Store tga img at location and compress it as GZ type
                StoreTgaImage(folder, faction);

                // Create icon element
                mainElement.Add(new XElement("icon",
                    new XAttribute("name", faction.IconData.Active),
                    new XAttribute("texture", $"extensions\\{modName}\\assets\\fx\\gui\\textures\\factions\\{faction.IconData.Active}.tga"),
                    new XAttribute("height", "256"),
                    new XAttribute("width", "256")));
            }
            return mainElement;
        }

        private static void StoreTgaImage(string folder, Faction faction)
        {
            var path = EnsureDirectoryExists(Path.Combine(folder, $"assets/fx/gui/textures/factions/{faction.IconData.Active}.tga"));

            // Create tga file at location
            using var img = ImageHelper.Base64ToImage(faction.Icon);
            // Create tga at location as regular file type
            ImageHelper.SaveAsTga(img, path.Replace(".tga", ""));

            // Compress to a gzip
            CompressTgaToGz(path.Replace(".tga", ""), path.Replace(".tga", ".gz"));

            // Delete original tga
            File.Delete(path.Replace(".tga", ""));
        }

        private static void CompressTgaToGz(string tgaPath, string gzPath)
        {
            using (FileStream originalFileStream = File.OpenRead(tgaPath))
            using (FileStream compressedFileStream = File.Create(gzPath))
            using (GZipStream compressionStream = new(compressedFileStream, CompressionLevel.Optimal))
            {
                originalFileStream.CopyTo(compressionStream);
            }

            Console.WriteLine($"Compressed {tgaPath} to {gzPath}");
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
