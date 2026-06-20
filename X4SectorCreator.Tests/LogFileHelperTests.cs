using X4SectorCreator.Helpers;
using Xunit;

namespace X4SectorCreator.Tests;

public sealed class LogFileHelperTests
{
    [Fact]
    public void ResolveRequestedLogFilePath_KeepsRequestedDirectory()
    {
        string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        string requestedPath = Path.Combine(tempRoot, "nested", "command.log");

        string resolvedPath = LogFileHelper.ResolveRequestedLogFilePath(requestedPath, "fallback.log");

        Assert.Equal(Path.GetFullPath(requestedPath), resolvedPath);
        Assert.True(Directory.Exists(Path.GetDirectoryName(resolvedPath)));
    }

    [Fact]
    public void ResolveRequestedLogFilePath_UsesDefaultFileNameForDirectory()
    {
        string tempRoot = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
        string requestedDirectory = Path.Combine(tempRoot, "logs") + Path.DirectorySeparatorChar;

        string resolvedPath = LogFileHelper.ResolveRequestedLogFilePath(requestedDirectory, "fallback.log");

        Assert.Equal(Path.Combine(Path.GetFullPath(tempRoot), "logs", "fallback.log"), resolvedPath);
    }
}
