using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class BasketsGeneration
    {
        public static void Generate(string folder, string modPrefix)
        {
            if (JobsForm.AllBaskets.Count == 0)
            {
                return;
            }

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    CollectBaskets(modPrefix)
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/baskets.xml")));
        }

        private static XElement CollectBaskets(string modPrefix)
        {
            XElement addElement = new("add", new XAttribute("sel", "//baskets"));

            foreach (KeyValuePair<string, Objects.Basket> basket in JobsForm.AllBaskets)
            {
                string originalId = basket.Value.Id;

                // Replace prefix
                basket.Value.Id = basket.Value.Id.Replace("PREFIX", modPrefix);

                // Serialize
                string basketElementXml = basket.Value.SerializeBasket();

                // Reset
                basket.Value.Id = originalId;

                XElement basketElement = XElement.Parse(basketElementXml);
                addElement.Add(basketElement);
            }

            return addElement;
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
