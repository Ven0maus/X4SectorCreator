using System.Text;

namespace X4SectorCreator.Helpers
{
    internal static class LogFileHelper
    {
        private static readonly object SessionLogLock = new();
        private static string _sessionLogPath;

        public static string SessionLogPath
        {
            get
            {
                lock (SessionLogLock)
                    return _sessionLogPath;
            }
        }

        public static string ResolveApplicationRoot()
        {
            string processPath = Environment.ProcessPath;
            if (!string.IsNullOrWhiteSpace(processPath))
            {
                string processDirectory = Path.GetDirectoryName(processPath);
                if (!string.IsNullOrWhiteSpace(processDirectory))
                    return Path.GetFullPath(processDirectory);
            }

            if (!string.IsNullOrWhiteSpace(AppContext.BaseDirectory))
                return Path.GetFullPath(AppContext.BaseDirectory);

            if (!string.IsNullOrWhiteSpace(AppDomain.CurrentDomain.BaseDirectory))
                return Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory);

            return Path.GetFullPath(Directory.GetCurrentDirectory());
        }

        public static string ResolveLogsDirectory()
        {
            string logsDirectory = Path.Combine(ResolveApplicationRoot(), "logs");
            _ = Directory.CreateDirectory(logsDirectory);
            return logsDirectory;
        }

        public static string ResolveRequestedLogFilePath(string requestedPath, string defaultFileName)
        {
            string resolvedPath = string.IsNullOrWhiteSpace(requestedPath)
                ? defaultFileName
                : Environment.ExpandEnvironmentVariables(requestedPath.Trim().Trim('"'));

            if (string.IsNullOrWhiteSpace(Path.GetFileName(resolvedPath)))
                resolvedPath = Path.Combine(resolvedPath, defaultFileName);

            string fullPath = Path.IsPathRooted(resolvedPath)
                ? Path.GetFullPath(resolvedPath)
                : Path.GetFullPath(resolvedPath, Directory.GetCurrentDirectory());

            string directory = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrWhiteSpace(directory))
                _ = Directory.CreateDirectory(directory);

            return fullPath;
        }

        public static string InitializeSessionLog(string requestedPath, string defaultFileName)
        {
            string fullPath = ResolveRequestedLogFilePath(requestedPath, defaultFileName);
            string contents = BuildDiagnosticLogContents("Session log initialized", string.Empty);

            lock (SessionLogLock)
            {
                File.WriteAllText(fullPath, contents);
                _sessionLogPath = fullPath;
            }

            return fullPath;
        }

        public static void AppendToSessionLog(string caption, string body)
        {
            string sessionLogPath;
            lock (SessionLogLock)
                sessionLogPath = _sessionLogPath;

            if (string.IsNullOrWhiteSpace(sessionLogPath))
                return;

            try
            {
                string contents = BuildDiagnosticLogContents(caption, body);
                lock (SessionLogLock)
                {
                    File.AppendAllText(sessionLogPath,
                        Environment.NewLine +
                        "============================================================" + Environment.NewLine +
                        contents);
                }
            }
            catch
            {
            }
        }

        public static string TryWriteDiagnosticLog(string prefix, string caption, string body)
        {
            if (string.IsNullOrWhiteSpace(body))
                return null;

            try
            {
                string logsDirectory = ResolveLogsDirectory();
                string safePrefix = SanitizeFileNamePart(prefix, "x4sectorcreator");
                string timestamp = DateTime.Now.ToString("yyyyMMdd-HHmmss-fff");

                string fullPath = Path.Combine(logsDirectory, $"{safePrefix}-{timestamp}.log");
                string latestPath = Path.Combine(logsDirectory, $"{safePrefix}-latest.log");
                string contents = BuildDiagnosticLogContents(caption, body);

                File.WriteAllText(fullPath, contents);
                File.WriteAllText(latestPath, contents);
                AppendToSessionLog(caption, body);
                return fullPath;
            }
            catch
            {
                return null;
            }
        }

        private static string BuildDiagnosticLogContents(string caption, string body)
        {
            StringBuilder builder = new();
            builder.AppendLine(caption);
            builder.AppendLine($"Timestamp: {DateTime.Now:O}");
            builder.AppendLine($"Process path: {Environment.ProcessPath ?? "<unknown>"}");
            builder.AppendLine($"Application root: {ResolveApplicationRoot()}");
            builder.AppendLine($"Current directory: {Directory.GetCurrentDirectory()}");
            builder.AppendLine($"Command line: {Environment.CommandLine}");
            builder.AppendLine();
            if (!string.IsNullOrWhiteSpace(body))
                builder.Append(body);
            return builder.ToString();
        }

        private static string SanitizeFileNamePart(string value, string fallback)
        {
            if (string.IsNullOrWhiteSpace(value))
                return fallback;

            char[] invalidCharacters = Path.GetInvalidFileNameChars();
            StringBuilder builder = new(value.Length);
            foreach (char character in value)
                builder.Append(invalidCharacters.Contains(character) ? '-' : character);

            string sanitized = builder
                .ToString()
                .Replace(' ', '-')
                .Trim('-')
                .ToLowerInvariant();

            return string.IsNullOrWhiteSpace(sanitized)
                ? fallback
                : sanitized;
        }
    }
}
