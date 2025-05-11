using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class GameStartsGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            // Generate a custom gamestart for custom galaxy
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                XElement customGameStart = GenerateCustomGameStart(modPrefix, clusters);
                if (customGameStart != null)
                {
                    XDocument xmlDocument = new(
                        new XDeclaration("1.0", "utf-8", null),
                        new XElement("diff",
                            new XElement("replace",
                            new XAttribute("sel", "//gamestarts"),
                            customGameStart)
                        )
                    );
                    xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/gamestarts.xml")));
                    return;
                }
            }

            List<XElement> generatedElements = GenerateVanillaChanges(vanillaChanges).ToList();

            if (generatedElements.Count > 0)
            {
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        generatedElements
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/gamestarts.xml")));
            }
        }

        private static XElement GenerateCustomGameStart(string modPrefix, List<Cluster> clusters)
        {
            if (clusters.Count == 0 || !clusters.SelectMany(a => a.Sectors).Any())
            {
                return null;
            }

            // Take the first one alphabetically
            var result = clusters
                .SelectMany(cluster => cluster.Sectors, (cluster, sector) => new { cluster, sector })
                .OrderBy(a => a.cluster.Name)
                .FirstOrDefault();

            Cluster cluster = result?.cluster;
            Sector sector = result?.sector;

            if (cluster == null || sector == null)
            {
                return null;
            }

            string sectorMacro = $"{modPrefix}_SE_c{cluster.Id:D3}_s{sector.Id:D3}_macro";

            XElement gameStartElement = null;
            try
            {
                var xml = File.ReadAllText(Constants.DataPaths.CustomGameStartPath);
                gameStartElement = XElement.Parse(xml);

                // Set galaxy, sector & known space
                XElement locationElement = gameStartElement.Element("location");
                locationElement?.SetAttributeValue("galaxy", $"{GalaxySettingsForm.GalaxyName}_macro");
            }
            catch (Exception e)
            {
                throw new Exception("The custom_galaxy_gamestart.xml mapping is invalid: " + e.Message);
            }

            XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
            return new XElement("gamestarts", new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                new XAttribute(xsi + "noNamespaceSchemaLocation", "gamestarts.xsd"),
                gameStartElement);
        }

        private static IEnumerable<XElement> GenerateVanillaChanges(VanillaChanges vanillaChanges)
        {
            // If argon prime was removed, remove it from "custom_creative" sector + known sectors
            // This will fix the display in the custom creative gamestart
            if (vanillaChanges.RemovedSectors.Any(a =>
                a.VanillaCluster.BaseGameMapping.Equals("Cluster_14", StringComparison.OrdinalIgnoreCase) &&
                a.Sector.BaseGameMapping.Equals("Sector001", StringComparison.OrdinalIgnoreCase)))
            {
                yield return new XElement("remove", new XAttribute("sel", "/gamestarts/gamestart[@id='custom_creative']/location/@sector"));
                yield return new XElement("remove", new XAttribute("sel", "/gamestarts/gamestart[@id='custom_creative']/player/knownspace/space"));
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
