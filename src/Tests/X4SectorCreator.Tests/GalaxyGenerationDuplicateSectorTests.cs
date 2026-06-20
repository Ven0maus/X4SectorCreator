using System.Drawing;
using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class GalaxyGenerationDuplicateSectorTests
{
    [Fact]
    public void Generate_DoesNotAssumeSectorNamesAreUniqueWhenResolvingGatePairs()
    {
        string outputDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(outputDir);

        try
        {
            GalaxySettingsForm.IsCustomGalaxy = false;
            GalaxySettingsForm.GalaxyName = "xu_ep2_universe";

            var firstDuplicate = new Sector
            {
                Id = 1,
                Name = "Shared Name",
                Zones = [new Zone { Id = 1, Position = new Point(0, 0) }],
            };

            var sourceGate = new Gate
            {
                Id = 1,
                ParentSectorName = "Origin",
                DestinationSectorName = "Shared Name",
                Source = "c001_s001_z001",
                Destination = "c002_s001_z001",
                SourcePath = "PREFIX_CL_c001_connection/PREFIX_SE_c001_s001_connection/PREFIX_ZO_c001_s001_z001_connection/PREFIX_GA_g001_c001_s001_z001_c002_s001_z001_connection",
                DestinationPath = "PREFIX_CL_c003_connection/PREFIX_SE_c003_s001_connection/PREFIX_ZO_c003_s001_z001_connection/PREFIX_GA_g001_c003_s001_z001_c001_s001_z001_connection",
            };

            var originSector = new Sector
            {
                Id = 2,
                Name = "Origin",
                Zones = [new Zone { Id = 1, Position = new Point(0, 0), Gates = [sourceGate] }],
            };

            var targetGate = new Gate
            {
                Id = 1,
                ParentSectorName = "Shared Name",
                DestinationSectorName = "Origin",
                Source = "c003_s001_z001",
                Destination = "c001_s001_z001",
                SourcePath = sourceGate.DestinationPath,
                DestinationPath = sourceGate.SourcePath,
            };

            var secondDuplicate = new Sector
            {
                Id = 3,
                Name = "Shared Name",
                Zones = [new Zone { Id = 1, Position = new Point(0, 0), Gates = [targetGate] }],
            };

            var clusters = new List<Cluster>
            {
                new() { Id = 1, Name = "C1", Sectors = [originSector] },
                new() { Id = 2, Name = "C2", Sectors = [firstDuplicate] },
                new() { Id = 3, Name = "C3", Sectors = [secondDuplicate] },
            };

            var ex = Record.Exception(() =>
                GalaxyGeneration.Generate(
                    outputDir,
                    "test",
                    clusters,
                    new VanillaChanges(),
                    new ClusterCollection()));

            Assert.Null(ex);
        }
        finally
        {
            Directory.Delete(outputDir, recursive: true);
        }
    }

    [Fact]
    public void Generate_PreservesImportedCustomClusterMacroAndConnectionNames()
    {
        string outputDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(outputDir);

        try
        {
            GalaxySettingsForm.IsCustomGalaxy = false;
            GalaxySettingsForm.GalaxyName = "xu_ep2_universe";

            var cluster = new Cluster
            {
                Id = 7,
                Name = "Imported Cluster",
                ImportedMacroName = "my_imported_cluster_macro",
                ImportedConnectionName = "my_imported_cluster_connection",
                Sectors = [new Sector { Id = 1, Name = "Imported Sector", Zones = [new Zone { Id = 1, Position = new Point(0, 0) }] }],
            };

            GalaxyGeneration.Generate(
                outputDir,
                "test",
                [cluster],
                new VanillaChanges(),
                new ClusterCollection());

            XDocument document = XDocument.Load(Path.Combine(outputDir, "maps", "xu_ep2_universe", "galaxy.xml"));
            XElement connection = Assert.Single(document.Root!.Elements("add").Single().Elements("connection"));

            Assert.Equal("my_imported_cluster_connection", (string?)connection.Attribute("name"));
            Assert.Equal("my_imported_cluster_macro", (string?)connection.Element("macro")?.Attribute("ref"));
        }
        finally
        {
            Directory.Delete(outputDir, recursive: true);
        }
    }
}
