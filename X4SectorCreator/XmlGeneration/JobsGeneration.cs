using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class JobsGeneration
    {
        public static void Generate(string folder, string modPrefix)
        {
            // Replace entire job file
            if (GalaxySettingsForm.IsCustomGalaxy)
            {
                XNamespace xsi = "http://www.w3.org/2001/XMLSchema-instance";
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        new XElement("replace", new XAttribute("sel", "//jobs"),
                        new XElement("jobs", new XAttribute(XNamespace.Xmlns + "xsi", xsi),
                            new XAttribute(xsi + "noNamespaceSchemaLocation", "libraries.xsd"),
                            CollectJobs(modPrefix))
                        )
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/jobs.xml")));
            }
            else
            {
                if (JobsForm.AllJobs.Count == 0) return;

                var addElement = new XElement("add", new XAttribute("sel", "//jobs"));
                var jobs = CollectJobs(modPrefix);
                foreach (var job in jobs)
                    addElement.Add(job);

                // Replace entire job file
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("diff",
                        addElement
                    )
                );
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/jobs.xml")));
            }
        }

        private static IEnumerable<XElement> CollectJobs(string modPrefix)
        {
            foreach (var job in JobsForm.AllJobs)
            {
                var originalId = job.Value.Id;
                var originalBasket = job.Value.Basket?.Basket;

                // Prepend prefix
                job.Value.Id = $"{modPrefix}_{job.Value.Id}";
                if (job.Value.Basket?.Basket != null)
                    job.Value.Basket.Basket = job.Value.Basket.Basket.Replace("PREFIX", modPrefix);

                // Serialize
                var jobElementXml = job.Value.SerializeJob();

                // Reset
                job.Value.Id = originalId;
                if (job.Value.Basket?.Basket != null)
                    job.Value.Basket.Basket = originalBasket;

                var jobElement = XElement.Parse(jobElementXml);
                yield return jobElement;
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
