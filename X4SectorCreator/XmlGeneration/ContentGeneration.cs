﻿using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.XmlGeneration
{
    internal static class ContentGeneration
    {
        public static void Generate(string folder, string modName, string depVersion, List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            XElement content = new("content",
                new XAttribute("id", $"{modName.Replace(" ", "_")}"),
                new XAttribute("name", $"{modName}"),
                new XAttribute("description", "Generated by Venomaus's X4 Sector Creator."),
                new XAttribute("author", "X4SectorCreator"),
                new XAttribute("version", "1"),
                new XAttribute("date", DateTime.Today.ToString("d")),
                new XAttribute("save", "0"),
                new XAttribute("enabled", "1"),
                GenerateDependencies(CollectDependencies(clusters, vanillaChanges)),
                new XElement("dependency", new XAttribute("version", depVersion)),
                GenerateTranslations(modName)
            );

            XDocument doc = new(new XDeclaration("1.0", "utf-8", null), content);
            doc.Save(EnsureDirectoryExists(Path.Combine(folder, $"content.xml")));
        }

        private static IEnumerable<(string code, string name)> CollectDependencies(List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            HashSet<string> dlcDependencies = new(StringComparer.OrdinalIgnoreCase);

            foreach (Cluster cluster in clusters)
            {
                if (!cluster.IsBaseGame)
                {
                    continue;
                }

                foreach (Sector sector in cluster.Sectors)
                {
                    if (sector.Zones.Count == 0)
                    {
                        continue;
                    }

                    bool breaked = false;
                    foreach (Zone zone in sector.Zones)
                    {
                        if (!zone.IsBaseGame)
                        {
                            if (!string.IsNullOrWhiteSpace(cluster.Dlc))
                            {
                                if (dlcDependencies.Contains(cluster.Dlc))
                                {
                                    breaked = true;
                                    break;
                                }

                                breaked = true;
                                _ = dlcDependencies.Add(cluster.Dlc);
                                break;
                            }
                        }
                    }
                    if (breaked)
                    {
                        break;
                    }
                }
            }

            // Check if vanilla changes touched any dlc content
            HashSet<string> allChangedContent = vanillaChanges.GetModifiedDlcContent()
                .Where(a => a != null)
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            // Add to current dlcDependencies
            foreach (string changedContent in allChangedContent)
            {
                _ = dlcDependencies.Add(changedContent);
            }

            // Convert to a pair
            return dlcDependencies.Select(a =>
            {
                KeyValuePair<string, string> pair = SectorMapForm.DlcMapping.First(b => b.Value.Equals(a, StringComparison.OrdinalIgnoreCase));
                return (pair.Value, pair.Key);
            });
        }

        private static IEnumerable<XElement> GenerateDependencies(IEnumerable<(string code, string name)> dependencies)
        {
            foreach ((string code, string name) in dependencies ?? [])
            {
                yield return new XElement("dependency", new XAttribute("id", code), new XAttribute("optional", "false"), new XAttribute("name", name));
            }
        }

        private static IEnumerable<XElement> GenerateTranslations(string modName)
        {
            HashSet<int> languages =
            [
                7, 33, 34, 39, 42, 44, 48, 49, 55, 81, 82, 86, 88
            ];
            foreach (int language in languages)
            {
                yield return new XElement("text", new XAttribute("language", $"{language}"), new XAttribute("name", modName), new XAttribute("description", "Generated by Venomaus's X4 Sector Creator."), new XAttribute("author", "X4SectorCreator"));
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
