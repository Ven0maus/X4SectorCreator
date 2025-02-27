using System.Text.Json;
using X4SectorCreator.Objects;

namespace X4SectorCreator
{
    internal class VersionChecker
    {
        private const string _versionUrl = "https://raw.githubusercontent.com/Ven0maus/X4SectorCreator/main/X4SectorCreator/version.json";

        public string CurrentVersion { get; }
        public string TargetGameVersion { get; }

        public VersionChecker()
        {
            string versionFilePath = Path.Combine(Application.StartupPath, "version.json");
            string versionContent = File.ReadAllText(versionFilePath);
            VersionInfo versionInfo = JsonSerializer.Deserialize<VersionInfo>(versionContent);

            CurrentVersion = versionInfo.AppVersion;
            TargetGameVersion = versionInfo.X4Version;
        }

        public async Task<(bool NewVersionAvailable, VersionInfo VersionInfo)> CheckForUpdatesAsync()
        {
            try
            {
                using HttpClient client = new();
                string response = await client.GetStringAsync(_versionUrl);

                VersionInfo versionInfo = JsonSerializer.Deserialize<VersionInfo>(response);
                return versionInfo != null && IsNewVersionAvailable(versionInfo.AppVersion) ? ((bool NewVersionAvailable, VersionInfo VersionInfo))(true, versionInfo) : ((bool NewVersionAvailable, VersionInfo VersionInfo))(false, versionInfo);
            }
            catch (Exception)
            {
                return (false, null);
            }
        }

        private bool IsNewVersionAvailable(string latestVersion)
        {
            return Version.TryParse(latestVersion, out Version latest) &&
                Version.TryParse(CurrentVersion, out Version current)
&& latest > current;
        }
    }
}
