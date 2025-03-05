using System.Xml.Linq;
using X4SectorCreator.Objects;
using Region = X4SectorCreator.Objects.Region;

namespace X4SectorCreator.XmlGeneration
{
    internal static class RegionDefinitionGeneration
    {
        public static void Generate(string folder, string modPrefix, List<Cluster> clusters, VanillaChanges vanillaChanges)
        {
            // Create XML structure
            var regions = GetRegions(modPrefix, clusters).ToArray();
            if (regions.Length > 0)
            {
                XDocument xmlDocument = new(
                    new XDeclaration("1.0", "utf-8", null),
                    new XElement("regions",
                        new XAttribute(XNamespace.Xmlns + "xsi", "http://www.w3.org/2001/XMLSchema-instance"),
                        new XAttribute(XName.Get("noNamespaceSchemaLocation", "http://www.w3.org/2001/XMLSchema-instance"), "region_definitions.xsd"),
                        regions
                    )
                );

                // Save to an XML file
                xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/region_definitions.xml")));
            }
        }

        private static IEnumerable<XElement> GetRegions(string modPrefix, List<Cluster> clusters)
        {
            foreach (Cluster cluster in clusters)
            {
                foreach (Sector sector in cluster.Sectors)
                {
                    foreach (Region region in sector.Regions)
                    {
                        // Region definition name needs to be fully lowercase else it will NOT work!!!!!!!!
                        yield return new XElement("region",
                            new XAttribute("name", $"{modPrefix}_re_c{cluster.Id:D3}_s{sector.Id:D3}_r{region.Id:D3}"),
                            new XAttribute("density", region.Definition.Density),
                            new XAttribute("rotation", region.Definition.Rotation),
                            new XAttribute("noisescale", region.Definition.NoiseScale),
                            new XAttribute("seed", region.Definition.Seed),
                            new XAttribute("minnoisevalue", region.Definition.MinNoiseValue),
                            new XAttribute("maxnoisevalue", region.Definition.MaxNoiseValue),
                            new XElement("boundary",
                                new XAttribute("class", $"{region.Definition.BoundaryType}"),
                                new XElement("size",
                                    new XAttribute("r", region.BoundaryRadius),
                                    new XAttribute("linear", region.BoundaryLinear)
                                )
                            ),
                            new XElement("falloff",
                                GenerateLateralRadialSteps(region)
                            ),
                            new XElement("fields",
                                GenerateFields(region)
                            ),
                            new XElement("resources",
                                GenerateResources(region)
                            )
                        );
                    }
                }
            }
        }

        private static IEnumerable<XElement> GenerateLateralRadialSteps(Region region)
        {
            IEnumerable<IGrouping<string, StepObj>> groups = region.Definition.Falloff.GroupBy(a => a.Type);
            foreach (IGrouping<string, StepObj> group in groups)
            {
                IEnumerable<XElement> steps = group.Select(a => new XElement("step",
                                        new XAttribute("position", a.Position),
                                        new XAttribute("value", a.Value)
                                    ));
                yield return new XElement(group.Key.ToLower(), steps);
            }
        }

        private static IEnumerable<XElement> GenerateFields(Region region)
        {
            foreach (FieldObj field in region.Definition.Fields)
            {
                yield return new XElement(field.Type.ToLower(),
                    // Core attributes
                    field.GroupRef != null ? new XAttribute("groupref", field.GroupRef) : null,
                    field.DensityFactor.HasValue ? new XAttribute("densityfactor", field.DensityFactor) : null,
                    field.Rotation.HasValue ? new XAttribute("rotation", field.Rotation) : null,
                    field.RotationVariation.HasValue ? new XAttribute("rotationvariation", field.RotationVariation) : null,
                    field.NoiseScale.HasValue ? new XAttribute("noisescale", field.NoiseScale) : null,
                    field.Seed.HasValue ? new XAttribute("seed", field.Seed) : null,
                    field.MinNoiseValue.HasValue ? new XAttribute("minnoisevalue", field.MinNoiseValue) : null,
                    field.MaxNoiseValue.HasValue ? new XAttribute("maxnoisevalue", field.MaxNoiseValue) : null,

                    // Volumetric Fog
                    field.Multiplier.HasValue ? new XAttribute("multiplier", field.Multiplier) : null,
                    field.Medium != null ? new XAttribute("medium", field.Medium) : null,
                    field.Texture != null ? new XAttribute("texture", field.Texture) : null,
                    field.LodRule != null ? new XAttribute("lodrule", field.LodRule) : null,
                    field.Size.HasValue ? new XAttribute("size", field.Size) : null,
                    field.SizeVariation.HasValue ? new XAttribute("sizevariation", field.SizeVariation) : null,
                    field.DistanceFactor.HasValue ? new XAttribute("distancefactor", field.DistanceFactor) : null,
                    field.Ref != null ? new XAttribute("ref", field.Ref) : null,
                    field.Factor.HasValue ? new XAttribute("factor", field.Factor) : null,

                    // Local RGB
                    field.LocalRed.HasValue ? new XAttribute("localred", field.LocalRed) : null,
                    field.LocalGreen.HasValue ? new XAttribute("localgreen", field.LocalGreen) : null,
                    field.LocalBlue.HasValue ? new XAttribute("localblue", field.LocalBlue) : null,
                    field.LocalDensity.HasValue ? new XAttribute("localdensity", field.LocalDensity) : null,

                    // Uniform RGB
                    field.UniformRed.HasValue ? new XAttribute("uniformred", field.UniformRed) : null,
                    field.UniformGreen.HasValue ? new XAttribute("uniformgreen", field.UniformGreen) : null,
                    field.UniformBlue.HasValue ? new XAttribute("uniformblue", field.UniformBlue) : null,
                    field.UniformDensity.HasValue ? new XAttribute("uniformdensity", field.UniformDensity) : null,

                    // Boolean Attribute (BackgroundFog as "true"/"false")
                    field.BackgroundFog.HasValue ? new XAttribute("backgroundfog", field.BackgroundFog.Value ? "true" : "false") : null,

                    // Resources
                    field.Resources != null ? new XAttribute("resources", field.Resources) : null,
                    field.SoundId != null ? new XAttribute("soundid", field.SoundId) : null,
                    field.Playtime != null ? new XAttribute("playtime", field.Playtime) : null
                );
            }
        }

        private static IEnumerable<XElement> GenerateResources(Region region)
        {
            foreach (Resource resource in region.Definition.Resources)
            {
                yield return new XElement("resource",
                                    new XAttribute("ware", $"{resource.Ware}"),
                                    new XAttribute("yield", $"{resource.Yield}")
                                );
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
