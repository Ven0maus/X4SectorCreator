using System.IO;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class ClusterGenerationSourceTests
{
    [Fact]
    public void GenerateClusters_PreservesImportedClusterMacroNameWhenPresent()
    {
        string sourcePath = Path.Combine(TestProjectPaths.ProgramRoot, "XmlGeneration", "ClusterGeneration.cs");

        string sourceText = File.ReadAllText(sourcePath);

        Assert.Contains("if (!string.IsNullOrWhiteSpace(cluster.ImportedMacroName))", sourceText);
        Assert.Contains("return cluster.ImportedMacroName.Replace(\"PREFIX\", modPrefix);", sourceText);
    }
}
