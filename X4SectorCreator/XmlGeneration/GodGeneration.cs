using System.Xml.Linq;

namespace X4SectorCreator.XmlGeneration
{
    internal static class GodGeneration
    {
        public static void Generate(string folder)
        {
            // Replace entire god file
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        new XElement("replace", new XAttribute("sel", "//god"),
                        new XElement("god", new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                            new XAttribute(xsi + "noNamespaceSchemaLocation", "libraries.xsd"))
                        )
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/god.xml")));
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
