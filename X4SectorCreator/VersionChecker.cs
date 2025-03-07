﻿using System.Text.Json;
using X4SectorCreator.Configuration;
using X4SectorCreator.Objects;

namespace X4SectorCreator
{
    internal class VersionChecker
    {
        private readonly string _versionFilePath = Path.Combine(Application.StartupPath, "version.json");
        private const string _versionUrl = "https://raw.githubusercontent.com/Ven0maus/X4SectorCreator/main/X4SectorCreator/version.json";
        private const string _sectorMappingUrl = "https://raw.githubusercontent.com/Ven0maus/X4SectorCreator/main/X4SectorCreator/Mappings/sector_mappings.json";

        public string CurrentVersion { get; }
        public string TargetGameVersion { get; private set; }

        public VersionChecker()
        {
            string versionContent = File.ReadAllText(_versionFilePath);
            VersionInfo versionInfo = JsonSerializer.Deserialize<VersionInfo>(versionContent, ConfigSerializer.SerializerOptions);

            CurrentVersion = versionInfo.AppVersion;
            TargetGameVersion = versionInfo.X4Version;
        }

        public async Task<(bool NewVersionAvailable, VersionInfo VersionInfo)> CheckForUpdatesAsync()
        {
            try
            {
                using HttpClient client = new();
                string response = await client.GetStringAsync(_versionUrl);

                VersionInfo versionInfo = JsonSerializer.Deserialize<VersionInfo>(response, ConfigSerializer.SerializerOptions);
                return versionInfo != null && (IsNewVersionAvailable(versionInfo.AppVersion, CurrentVersion) || IsNewVersionAvailable(versionInfo.X4Version, TargetGameVersion)) ?
                    ((bool NewVersionAvailable, VersionInfo VersionInfo))(true, versionInfo) : ((bool NewVersionAvailable, VersionInfo VersionInfo))(false, versionInfo);
            }
            catch (Exception)
            {
                return (false, null);
            }
        }

        public static async Task<string> GetUpdatedSectorMappingAsync()
        {
            try
            {
                using HttpClient client = new();
                return await client.GetStringAsync(_sectorMappingUrl);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public void UpdateX4Version(VersionInfo versionInfo)
        {
            if (CurrentVersion.Equals(versionInfo.AppVersion))
            {
                if (!TargetGameVersion.Equals(versionInfo.X4Version))
                {
                    // Adjust target version
                    TargetGameVersion = versionInfo.X4Version;

                    // Save it to disk
                    VersionInfo newVersionInfo = new()
                    {
                        AppVersion = CurrentVersion,
                        X4Version = versionInfo.X4Version,
                    };
                    string json = JsonSerializer.Serialize(newVersionInfo, ConfigSerializer.SerializerOptions);
                    File.WriteAllText(_versionFilePath, json);
                }
            }
        }

        public static bool CompareVersion(string source, string target)
        {
            return Version.TryParse(source, out Version current) &&
                 Version.TryParse(target, out Version other) && current.Equals(other);
        }

        private static bool IsNewVersionAvailable(string latestVersion, string version)
        {
            return Version.TryParse(latestVersion, out Version latest) &&
                Version.TryParse(version, out Version current) && latest > current;
        }
    }
}
