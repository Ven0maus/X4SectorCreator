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
            var versionFilePath = Path.Combine(Application.StartupPath, "version.json");
            var versionContent = File.ReadAllText(versionFilePath);
            var versionInfo = JsonSerializer.Deserialize<VersionInfo>(versionContent);

            CurrentVersion = versionInfo.AppVersion;
            TargetGameVersion = versionInfo.X4Version;
        }

        public async Task<(bool NewVersionAvailable, VersionInfo VersionInfo)> CheckForUpdatesAsync()
        {
            try
            {
                using HttpClient client = new();
                string response = await client.GetStringAsync(_versionUrl);

                var versionInfo = JsonSerializer.Deserialize<VersionInfo>(response);
                if (versionInfo != null && IsNewVersionAvailable(versionInfo.AppVersion))
                {
                    return (true, versionInfo);
                }
                else
                {
                    return (false, versionInfo);
                }
            }
            catch (Exception)
            {
                return (false, null);
            }
        }

        private bool IsNewVersionAvailable(string latestVersion)
        {
            if (Version.TryParse(latestVersion, out Version latest) &&
                Version.TryParse(CurrentVersion, out Version current))
            {
                return latest > current;
            }
            return false;
        }
    }
}
