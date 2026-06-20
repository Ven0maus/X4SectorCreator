using System.IO;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class X4SectorCreatorCsprojSourceTests
{
    [Fact]
    public void ProjectFile_OnlyTargetsWindowsFrameworkOnWindowsHosts()
    {
        string sourcePath = Path.Combine(TestProjectPaths.ProgramRoot, "X4SectorCreator.csproj");

        string sourceText = File.ReadAllText(sourcePath);

        Assert.Contains("<TargetFrameworks Condition=\"$([MSBuild]::IsOSPlatform('windows'))\">net9.0;net9.0-windows</TargetFrameworks>", sourceText);
        Assert.Contains("<TargetFrameworks Condition=\"!$([MSBuild]::IsOSPlatform('windows'))\">net9.0</TargetFrameworks>", sourceText);
    }
}
