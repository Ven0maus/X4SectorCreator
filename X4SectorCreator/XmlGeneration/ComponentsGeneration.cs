using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ComponentsGeneration
    {
        public static void Generate(string folder, string modPrefix, string modName)
        {
            var components = CollectComponents(modPrefix, folder, modName);
            if (components == null) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    components
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/components.xml")));
        }

        private static XElement CollectComponents(string modPrefix, string modFolder, string modName)
        {
            var addElement = new XElement("add", new XAttribute("sel", "/index"));

            int count = 0;
            foreach (var cluster in MainForm.Instance.AllClusters.Values)
            {
                // If cluster has custom xml export it
                if (string.IsNullOrWhiteSpace(cluster.CustomClusterXml)) continue;

                var index = GenerateClusterAssetFile(cluster, modPrefix, modFolder);
                if (index == null) continue;

                // Create the _macro asset file too
                GenerateClusterAssetMacroFile(cluster, modPrefix, modFolder);

                addElement.Add(new XElement("entry", 
                    new XAttribute("name", index), 
                    new XAttribute("value", $"extensions\\{modName}\\assets\\environments\\cluster\\{index}")));

                count++;
            }

            return count > 0 ? addElement : null;
        }

        private static string GenerateClusterAssetFile(Cluster cluster, string modPrefix, string modFolder)
        {
            var clusterCode = GetClusterCode(cluster).Replace("PREFIX", modPrefix);

            try
            {
                var xDoc = XDocument.Parse(cluster.CustomClusterXml);

                // Verify if the name still matches the cluster code
                var componentName = (xDoc.Element("components")?.Elements("component")?.FirstOrDefault()?.Attribute("name")) ?? 
                    throw new Exception("Invalid XML content, no component found?");

                // Automatically correct the name if mismatched
                if (string.IsNullOrWhiteSpace(componentName.Value) || !componentName.Value.Equals(clusterCode))
                {
                    componentName.Value = clusterCode;
                }

                xDoc.Save(EnsureDirectoryExists(Path.Combine(modFolder, $"assets/environments/cluster/{clusterCode}.xml")));
            }
            catch(Exception ex)
            {
                _ = MessageBox.Show($"Unable to generate cluster asset file for cluster \"{cluster.Name}\": {ex.Message}");
                return null;
            }

            return clusterCode;
        }

        private static void GenerateClusterAssetMacroFile(Cluster cluster, string modPrefix, string modFolder)
        {
            var clusterCode = GetClusterCode(cluster).Replace("PREFIX", modPrefix);

            try
            {
                XDocument macro = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("macros",
                        new XElement("macro",
                            new XAttribute("name", $"{clusterCode}_macro"),
                            new XAttribute("class", "celestialbody"),
                        new XElement("component", new XAttribute("ref", clusterCode)),
                        new XElement("properties",
                            new XElement("identification", new XAttribute("unique", "0")))
                        )
                    )
                );

                macro.Save(EnsureDirectoryExists(Path.Combine(modFolder, $"assets/environments/cluster/macros/{clusterCode}_macro.xml")));
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show($"Unable to generate cluster asset macro file for cluster \"{cluster.Name}\": {ex.Message}");
            }
        }

        private static string GetClusterCode(Cluster cluster)
        {
            return $"PREFIX_CL_c{cluster.Id:D3}";
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
