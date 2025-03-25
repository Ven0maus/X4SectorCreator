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
                if (JobsForm.AllJobs.Count == 0)
                {
                    return;
                }

                XElement addElement = new("add", new XAttribute("sel", "//jobs"));
                IEnumerable<XElement> jobs = CollectJobs(modPrefix);
                foreach (XElement job in jobs)
                {
                    addElement.Add(job);
                }

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
            foreach (KeyValuePair<string, Objects.Job> job in JobsForm.AllJobs)
            {
                string originalId = job.Value.Id;
                string originalBasket = job.Value.Basket?.Basket;

                // Prepend prefix
                job.Value.Id = $"{modPrefix}_{job.Value.Id}";
                if (job.Value.Basket?.Basket != null)
                {
                    job.Value.Basket.Basket = job.Value.Basket.Basket.Replace("PREFIX", modPrefix);
                }

                // Serialize
                string jobElementXml = job.Value.SerializeJob();

                // Reset
                job.Value.Id = originalId;
                if (job.Value.Basket?.Basket != null)
                {
                    job.Value.Basket.Basket = originalBasket;
                }

                XElement jobElement = XElement.Parse(jobElementXml);
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
