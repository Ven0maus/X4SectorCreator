using System.IO;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class MainFormImportSourceTests
{
    [Fact]
    public void MainFormSource_ExportsImportedJsonWhenRequested()
    {
        string sourcePath = Path.Combine(TestProjectPaths.ProgramRoot, "Forms", "General", "MainForm.cs");

        string sourceText = File.ReadAllText(sourcePath);

        Assert.Contains("WriteImportedSnapshotsIfRequested(modPath, importedMod, reversePathFixSummary);", sourceText);
        Assert.Contains("if (string.IsNullOrWhiteSpace(_startupOptions.ExportImportJsonPath))", sourceText);
        Assert.Contains("JsonSerializer.Serialize(payload, ConfigSerializer.JsonSerializerOptions);", sourceText);
        Assert.Contains("File.WriteAllText(exportPath, json);", sourceText);
        Assert.Contains("if (string.IsNullOrWhiteSpace(_startupOptions.ExportImportXmlPath))", sourceText);
        Assert.Contains("BuildImportedXmlDocument(modPath, importedMod, reversePathFixSummary);", sourceText);
        Assert.Contains("document.Save(exportPath);", sourceText);
    }
}
