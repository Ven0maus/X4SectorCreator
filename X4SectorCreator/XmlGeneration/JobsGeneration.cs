﻿using System.Xml.Linq;
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
                string originalMacro = job.Value.Location?.Macro;

                // Prepend prefix & replace subordinate job prefix
                job.Value.Id = $"{modPrefix}_{job.Value.Id}";
                if (job.Value.Subordinates?.Subordinate != null)
                {
                    foreach (var subordinate in job.Value.Subordinates.Subordinate)
                    {
                        if (JobsForm.AllJobs.ContainsKey(subordinate.Job))
                            subordinate.Job = $"{modPrefix}_{subordinate.Job}";
                    }
                }

                // Replace basket prefix
                if (job.Value.Basket?.Basket != null)
                {
                    job.Value.Basket.Basket = job.Value.Basket.Basket.Replace("PREFIX", modPrefix);
                }

                // Replace location macro prefix
                if (job.Value.Location?.Macro != null && job.Value.Location.Macro.Contains("PREFIX"))
                {
                    job.Value.Location.Macro = job.Value.Location.Macro.Replace("PREFIX", modPrefix);
                }

                // Serialize
                string jobElementXml = job.Value.SerializeJob();

                // Reset
                job.Value.Id = originalId;
                if (job.Value.Basket?.Basket != null)
                {
                    job.Value.Basket.Basket = originalBasket;
                }
                if (job.Value.Location?.Macro != null)
                {
                    job.Value.Location.Macro = originalMacro;
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
