using System.Drawing;
using System.Xml.Linq;
using X4SectorCreator.Forms;
using X4SectorCreator.Objects;
using X4SectorCreator.XmlGeneration;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class SectorGenerationSequenceTests
{
    [Fact]
    public void Generate_UsesSequenceSafeSelectorsForNewZonesInVanillaSectors()
    {
        string outputDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        Directory.CreateDirectory(outputDir);

        try
        {
            GalaxySettingsForm.GalaxyName = "xu_ep2_universe";
            MainForm.Instance = new MainForm();

            var zone1 = new Zone
            {
                Id = 1,
                Name = "Zone001_Cluster_43_Sector001",
                Position = new Point(1000, 2000),
            };
            var zone2 = new Zone
            {
                Id = 2,
                Position = new Point(3000, 4000),
            };

            var sector = new Sector
            {
                Id = 1,
                BaseGameMapping = "Sector001",
                Zones = [zone1, zone2],
            };
            var cluster = new Cluster
            {
                Id = 43,
                BaseGameMapping = "Cluster_43",
                Sectors = [sector],
            };

            var vanillaSector = new Sector
            {
                Id = 1,
                BaseGameMapping = "Sector001",
                Zones = [new Zone { Id = 1, Name = zone1.Name, Position = zone1.Position }],
            };
            var vanillaCluster = new Cluster
            {
                Id = 43,
                BaseGameMapping = "Cluster_43",
                Sectors = [vanillaSector],
            };

            SectorGeneration.Generate(
                outputDir,
                "test",
                [cluster],
                new ClusterCollection { Clusters = [vanillaCluster] },
                new VanillaChanges());

            XDocument document = XDocument.Load(Path.Combine(outputDir, "maps", "xu_ep2_universe", "sectors.xml"));
            List<XElement> addElements = document.Root!.Elements("add").ToList();

            Assert.Contains(
                addElements,
                element =>
                    (string?)element.Attribute("sel") == "/macros/macro[@name='Cluster_43_Sector001_macro']/connections/connection[not(@ref='zones')][1]" &&
                    (string?)element.Attribute("pos") == "before");

            Assert.Contains(
                addElements,
                element =>
                    (string?)element.Attribute("sel") == "/macros/macro[@name='Cluster_43_Sector001_macro']/connections[not(connection[not(@ref='zones')])]" &&
                    element.Attribute("pos") is null);

            Assert.DoesNotContain(
                addElements,
                element => (string?)element.Attribute("sel") == "//macros/macro[@name='Cluster_43_Sector001_macro']/connections");
        }
        finally
        {
            Directory.Delete(outputDir, recursive: true);
        }
    }
}
