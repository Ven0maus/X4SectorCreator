using System.Xml.Linq;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class X4ProjectRuntimeConfigTests
{
    [Fact]
    public void X4SectorCreatorProject_EnablesSystemDrawingUnixSupport()
    {
        string? root = FindRepositoryRoot();
        Assert.NotNull(root);

        string projectPath = Path.Combine(root!, "X4SectorCreator", "X4SectorCreator.csproj");

        XDocument document = XDocument.Load(projectPath);

        bool hasUnixSupportOption = document
            .Descendants()
            .Any(element =>
                element.Name.LocalName == "RuntimeHostConfigurationOption" &&
                string.Equals((string?)element.Attribute("Include"), "System.Drawing.EnableUnixSupport", StringComparison.Ordinal) &&
                string.Equals((string?)element.Attribute("Value"), "true", StringComparison.OrdinalIgnoreCase));

        Assert.True(hasUnixSupportOption,
            "X4SectorCreator must enable System.Drawing Unix support so the Linux/X11Forms build can initialize GDI+ at startup.");
    }

    private static string? FindRepositoryRoot()
    {
        DirectoryInfo? current = new(AppContext.BaseDirectory);

        while (current != null)
        {
            if (File.Exists(Path.Combine(current.FullName, "X4SectorCreator.sln")))
                return current.FullName;

            current = current.Parent;
        }

        return null;
    }
}
