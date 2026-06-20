using System.IO;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class ProgramSourceTests
{
    [Fact]
    public void ProgramSource_IncludesHelpHandlingAndExitAfterImportHelpText()
    {
        string sourcePath = Path.Combine(TestProjectPaths.ProgramRoot, "Program.cs");

        string sourceText = File.ReadAllText(sourcePath);

        Assert.Contains("if (IsHelpRequested(rawArgs))", sourceText);
        Assert.Contains("--exit-after-import", sourceText);
        Assert.Contains("--export-import-json", sourceText);
        Assert.Contains("--export-import-xml", sourceText);
        Assert.Contains("string.Equals(a, \"--help\"", sourceText);
        Assert.Contains("string.Equals(a, \"-h\"", sourceText);
        Assert.Contains("string.Equals(a, \"/?\"", sourceText);
    }

    [Fact]
    public void ProgramSource_ParsesAndConvertsExportImportJsonArgument()
    {
        string sourcePath = Path.Combine(TestProjectPaths.ProgramRoot, "Program.cs");

        string sourceText = File.ReadAllText(sourcePath);

        Assert.Contains("public string ExportImportJsonPath { get; private set; }", sourceText);
        Assert.Contains("public string ExportImportXmlPath { get; private set; }", sourceText);
        Assert.Contains("case \"--export-import-json\":", sourceText);
        Assert.Contains("case \"--export-import-xml\":", sourceText);
        Assert.Contains("options.ExportImportJsonPath = args[++i];", sourceText);
        Assert.Contains("options.ExportImportXmlPath = args[++i];", sourceText);
        Assert.Contains("options.ExportImportJsonPath = ConvertToWinePath(options.ExportImportJsonPath);", sourceText);
        Assert.Contains("options.ExportImportXmlPath = ConvertToWinePath(options.ExportImportXmlPath);", sourceText);
    }
}
