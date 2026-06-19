using System.Xml.Linq;
using X4SectorCreator.Helpers;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class ImportSectorMetadataResolverTests
{
    [Fact]
    public void ParseResourceAreas_ParsesGeneratedMapDefaultsShape()
    {
        XElement properties = XElement.Parse(
            """
            <properties>
              <resourceareas>
                <resourcearea amount="3" ref="sphere_medium_ore_high_average" />
                <resourcearea amount="1" ref="sphere_large_hydrogen_low_fast" />
              </resourceareas>
            </properties>
            """);

        var resources = ImportSectorMetadataResolver.ParseResourceAreas(properties);

        Assert.Collection(
            resources,
            first =>
            {
                Assert.Equal("medium", first.Size);
                Assert.Equal("ore", first.Ware);
                Assert.Equal("high", first.Yield);
                Assert.Equal("average", first.Speed);
                Assert.Equal(3, first.Amount);
            },
            second =>
            {
                Assert.Equal("large", second.Size);
                Assert.Equal("hydrogen", second.Ware);
                Assert.Equal("low", second.Yield);
                Assert.Equal("fast", second.Speed);
                Assert.Equal(1, second.Amount);
            });
    }

    [Fact]
    public void ResolveOwner_ReadsOptionalOwnerAttributeFromArea()
    {
        XElement area = XElement.Parse("<area owner=\"argon\" sunlight=\"1.0\" />");

        string owner = ImportSectorMetadataResolver.ResolveOwner(area);

        Assert.Equal("argon", owner);
    }
}
