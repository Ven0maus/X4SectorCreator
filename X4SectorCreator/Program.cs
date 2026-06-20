using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using X4SectorCreator.Configuration;
using X4SectorCreator.Helpers;

namespace X4SectorCreator
{
    internal static class Program
    {
        private const uint AttachParentProcess = 0xFFFFFFFF;
        private static bool _commandLoggingInitialized;
        private static string _commandLogFilePath;
        private static StreamWriter _commandLogWriter;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = invariantCulture;
            Thread.CurrentThread.CurrentUICulture = invariantCulture;

            string[] rawArgs = Environment.GetCommandLineArgs().Skip(1).ToArray();
            TryInitializeCommandLoggingFromRawArgs(rawArgs);

            StartupOptions startupOptions;
            try
            {
                startupOptions = StartupOptions.Parse(rawArgs);
            }
            catch (Exception ex)
            {
                LogFileHelper.AppendToSessionLog("Invalid command line arguments", ex.ToString());
                if (Environment.UserInteractive)
                {
                    _ = MessageBox.Show(ex.Message, "Invalid command line arguments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                else
                {
                    Console.Error.WriteLine(ex.Message);
                }
                return;
            }

            SetupCommandLogging(startupOptions.LoggerFilePath);

            if (!string.IsNullOrWhiteSpace(startupOptions.CheckSectorIslandsPath))
            {
                AttachToConsole();
                Environment.ExitCode = SectorIslandCheckService.Run(startupOptions.CheckSectorIslandsPath, startupOptions.ImportModPath);
                return;
            }

            if (!string.IsNullOrWhiteSpace(startupOptions.CheckImportPath))
            {
                AttachToConsole();
                Environment.ExitCode = ImportAuditService.Run(startupOptions.CheckImportPath, startupOptions.ImportModPath);
                return;
            }

            if (!string.IsNullOrWhiteSpace(startupOptions.CheckNameResolutionPath))
            {
                AttachToConsole();
                Environment.ExitCode = ImportAuditService.RunNameResolution(startupOptions.CheckNameResolutionPath, startupOptions.ImportModPath);
                return;
            }

            if (!string.IsNullOrWhiteSpace(startupOptions.CheckIncludedFilesPath))
            {
                AttachToConsole();
                Environment.ExitCode = ImportAuditService.RunIncludedFiles(startupOptions.CheckIncludedFilesPath, startupOptions.ImportModPath);
                return;
            }

            if (!string.IsNullOrWhiteSpace(startupOptions.ListSectorNamesPath))
            {
                AttachToConsole();
                Environment.ExitCode = ImportAuditService.RunSectorNameList(startupOptions.ListSectorNamesPath, startupOptions.ImportModPath);
                return;
            }

            if (!string.IsNullOrWhiteSpace(startupOptions.FixUnifiedSectorTranslationPath))
            {
                AttachToConsole();
                string attachedBase = !string.IsNullOrWhiteSpace(startupOptions.ImportModMergePath)
                    ? startupOptions.ImportModPath
                    : null;
                Environment.ExitCode = SectorTranslationFixService.Run(startupOptions.FixUnifiedSectorTranslationPath, attachedBase);
                return;
            }

            try
            {
                ApplyGalaxySettingStartupOptions(startupOptions);

                Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);
                Application.ThreadException += (_, exArgs) => ShowFatalStartupError(exArgs.Exception, "Unhandled UI exception");
                AppDomain.CurrentDomain.UnhandledException += (_, exArgs) =>
                {
                    if (exArgs.ExceptionObject is Exception exception)
                    {
                        ShowFatalStartupError(exception, "Unhandled application exception");
                    }
                };

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm(startupOptions));
            }
            catch (Exception ex)
            {
                ShowFatalStartupError(ex, "Startup failure");
            }
        }

        private static void ShowFatalStartupError(Exception ex, string caption)
        {
            string logPath = TryWriteStartupErrorLog(ex, caption);
            string message = $"{caption}:{Environment.NewLine}{ex.GetType().Name}: {ex.Message}";

            if (!string.IsNullOrWhiteSpace(logPath))
            {
                message += Environment.NewLine + Environment.NewLine + $"A log file was written to:{Environment.NewLine}{logPath}";
            }

            if (Environment.UserInteractive)
            {
                _ = MessageBox.Show(message, caption, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            else
            {
                Console.Error.WriteLine(message);
                Console.Error.WriteLine(ex);
            }
        }

        private static string TryWriteStartupErrorLog(Exception ex, string caption)
        {
            try
            {
                string contents =
                    $"{caption}{Environment.NewLine}" +
                    ex;

                return LogFileHelper.TryWriteDiagnosticLog("startup-error", caption, contents);
            }
            catch
            {
                return null;
            }
        }

        private static void AttachToConsole()
        {
            if (!OperatingSystem.IsWindows())
                return;

            if (!AttachConsole(AttachParentProcess))
            {
                _ = AllocConsole();
            }

            var standardOut = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(standardOut);

            var standardError = new StreamWriter(Console.OpenStandardError()) { AutoFlush = true };
            Console.SetError(standardError);

            if (_commandLogWriter != null)
            {
                Console.SetOut(new MultiTextWriter(Console.Out, _commandLogWriter));
                Console.SetError(new MultiTextWriter(Console.Error, _commandLogWriter));
            }
        }

        private static void SetupCommandLogging(string loggerFilePath)
        {
            if (_commandLoggingInitialized || string.IsNullOrWhiteSpace(loggerFilePath))
                return;

            try
            {
                _commandLoggingInitialized = true;

                string fullPath = LogFileHelper.InitializeSessionLog(loggerFilePath, "x4sectorcreator-command.log");

                _commandLogFilePath = fullPath;
                _commandLogWriter = new StreamWriter(fullPath, append: true) { AutoFlush = true };
                _commandLogWriter.WriteLine($"Logger initialized: {DateTime.Now:O}");
                _commandLogWriter.WriteLine($"Target file: {fullPath}");
                _commandLogWriter.WriteLine($"Process path: {Environment.ProcessPath ?? "<unknown>"}");
                _commandLogWriter.WriteLine($"Application root: {LogFileHelper.ResolveApplicationRoot()}");
                _commandLogWriter.WriteLine($"Current directory: {Directory.GetCurrentDirectory()}");
                _commandLogWriter.WriteLine($"Command line: {Environment.CommandLine}");
                _commandLogWriter.Flush();
            }
            catch (Exception ex)
            {
                _commandLogWriter = null;
                _commandLogFilePath = null;
                _commandLoggingInitialized = false;
                LogFileHelper.AppendToSessionLog("Command logging initialization failure", ex.ToString());
                Console.Error.WriteLine($"Warning: failed to initialize command logging: {ex.Message}");
            }
        }

        private static void TryInitializeCommandLoggingFromRawArgs(string[] rawArgs)
        {
            if (rawArgs == null || rawArgs.Length == 0)
                return;

            bool convertPathsToWineStyle = false;
            string loggerFilePath = null;

            for (int i = 0; i < rawArgs.Length; i++)
            {
                string arg = rawArgs[i];
                if (string.Equals(arg, "--wine-path", StringComparison.OrdinalIgnoreCase) ||
                    string.Equals(arg, "--proton-path", StringComparison.OrdinalIgnoreCase))
                {
                    convertPathsToWineStyle = true;
                    continue;
                }

                if ((string.Equals(arg, "--log-file", StringComparison.OrdinalIgnoreCase) ||
                     string.Equals(arg, "--logger-file", StringComparison.OrdinalIgnoreCase)) &&
                    i + 1 < rawArgs.Length)
                {
                    loggerFilePath = rawArgs[i + 1];
                    break;
                }
            }

            if (string.IsNullOrWhiteSpace(loggerFilePath))
                return;

            if (convertPathsToWineStyle)
                loggerFilePath = ConvertToWineStylePath(loggerFilePath);

            SetupCommandLogging(loggerFilePath);
        }

        private static string ConvertToWineStylePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            string trimmed = path.Trim();
            if (trimmed.Length >= 2 && char.IsLetter(trimmed[0]) && trimmed[1] == ':')
                return trimmed;

            if (!trimmed.StartsWith("/", StringComparison.Ordinal))
                return trimmed;

            return "Z:" + trimmed.Replace('/', '\\');
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AttachConsole(uint dwProcessId);

        private sealed class MultiTextWriter(TextWriter primary, TextWriter secondary) : TextWriter
        {
            public override Encoding Encoding => primary.Encoding;

            public override void Write(char value)
            {
                primary.Write(value);
                secondary.Write(value);
            }

            public override void Write(string value)
            {
                primary.Write(value);
                secondary.Write(value);
            }

            public override void WriteLine(string value)
            {
                primary.WriteLine(value);
                secondary.WriteLine(value);
            }

            public override void Flush()
            {
                primary.Flush();
                secondary.Flush();
            }
        }

        private static void ApplyGalaxySettingStartupOptions(StartupOptions startupOptions)
        {
            if (startupOptions == null)
                return;

            if (startupOptions.EnableCustomGalaxy)
            {
                Forms.GalaxySettingsForm.IsCustomGalaxy = true;
                Forms.GalaxySettingsForm.DisableAllStorylines = true;
            }

            if (!string.IsNullOrWhiteSpace(startupOptions.CustomGalaxyName))
            {
                Forms.GalaxySettingsForm.GalaxyName = startupOptions.CustomGalaxyName.ToLowerInvariant();
                Forms.GalaxySettingsForm.IsCustomGalaxy = true;
                Forms.GalaxySettingsForm.DisableAllStorylines = true;
            }

            if (startupOptions.DisableAllStorylines)
            {
                Forms.GalaxySettingsForm.DisableAllStorylines = true;
            }
        }

    }

    public sealed class StartupOptions
    {
        public string ImportModPath { get; private set; }
        public string ImportModMergePath { get; private set; }
        public string CheckSectorIslandsPath { get; private set; }
        public string CheckImportPath { get; private set; }
        public string CheckNameResolutionPath { get; private set; }
        public string CheckIncludedFilesPath { get; private set; }
        public string ListSectorNamesPath { get; private set; }
        public string FixSectorIslandsPath { get; private set; }
        public bool FixReversePathsOnImport { get; private set; }
        public string FixUnifiedSectorTranslationPath { get; private set; }
        public int ClusterHexGap { get; private set; }
        public string LoggerFilePath { get; private set; }
        public string CustomGalaxyName { get; private set; }
        public bool DisableAllStorylines { get; private set; }
        public bool EnableCustomGalaxy { get; private set; }
        public bool ExitAfterImport { get; private set; }
        public bool OpenGalaxyView { get; private set; }
        private bool ConvertPathsToWineStyle { get; set; }
        private bool CheckSectorIslandsRequested { get; set; }
        private bool CheckImportRequested { get; set; }
        private bool CheckNameResolutionRequested { get; set; }
        private bool CheckIncludedFilesRequested { get; set; }
        private bool ListSectorNamesRequested { get; set; }
        private bool FixSectorIslandsRequested { get; set; }
        private bool FixUnifiedSectorTranslationRequested { get; set; }

        public static StartupOptions Parse(string[] args)
        {
            StartupOptions options = new();

            for (int i = 0; i < args.Length; i++)
            {
                string arg = args[i];
                switch (arg.ToLowerInvariant())
                {
                    case "--import-mod":
                        if (i + 1 >= args.Length)
                            throw new ArgumentException("Missing path after --import-mod");
                        options.ImportModPath = args[++i];
                        break;
                    case "--import-mod-merge":
                    case "--merge-import":
                        if (i + 1 >= args.Length)
                            throw new ArgumentException("Missing path after --import-mod-merge");
                        options.ImportModMergePath = args[++i];
                        break;
                    case "--check-sector-islands":
                    case "--check-islands":
                        options.CheckSectorIslandsRequested = true;
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                            options.CheckSectorIslandsPath = args[++i];
                        break;
                    case "--fix-sector-islands":
                    case "--fix-islands":
                        options.FixSectorIslandsRequested = true;
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                            options.FixSectorIslandsPath = args[++i];
                        break;
                    case "--fix-reverse-paths":
                    case "--fix-gate-reverse-paths":
                        options.FixReversePathsOnImport = true;
                        break;
                    case "--cluster-hex-gap":
                        if (i + 1 >= args.Length)
                            throw new ArgumentException("Missing value after --cluster-hex-gap");
                        if (!int.TryParse(args[++i], out int clusterHexGap) || clusterHexGap < 0)
                            throw new ArgumentException("--cluster-hex-gap must be a non-negative integer.");
                        options.ClusterHexGap = clusterHexGap;
                        break;
                    case "--fix-unified-sector-translation":
                    case "--fix-sector-translation-page":
                    case "--single-unified-page-sector-translation-fix":
                        options.FixUnifiedSectorTranslationRequested = true;
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                            options.FixUnifiedSectorTranslationPath = args[++i];
                        break;
                    case "--check-import":
                    case "--audit-import":
                        options.CheckImportRequested = true;
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                            options.CheckImportPath = args[++i];
                        break;
                    case "--check-name-resolution":
                    case "--check-sector-name-resolution":
                    case "--audit-name-resolution":
                        options.CheckNameResolutionRequested = true;
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                            options.CheckNameResolutionPath = args[++i];
                        break;
                    case "--check-included-files":
                    case "--check-included-vs-ignored":
                    case "--audit-included-files":
                        options.CheckIncludedFilesRequested = true;
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                            options.CheckIncludedFilesPath = args[++i];
                        break;
                    case "--list-sector-names":
                    case "--list-sectors":
                    case "--audit-sector-names":
                        options.ListSectorNamesRequested = true;
                        if (i + 1 < args.Length && !args[i + 1].StartsWith("--", StringComparison.Ordinal))
                            options.ListSectorNamesPath = args[++i];
                        break;
                    case "--open-galaxy-view":
                    case "--show-galaxy-view":
                        options.OpenGalaxyView = true;
                        break;
                    case "--disable-story":
                    case "--disable-stories":
                    case "--disable-all-storylines":
                        options.DisableAllStorylines = true;
                        break;
                    case "--custom-galaxy":
                    case "--enable-custom-galaxy":
                        options.EnableCustomGalaxy = true;
                        break;
                    case "--galaxy-name":
                    case "--custom-galaxy-name":
                        if (i + 1 >= args.Length)
                            throw new ArgumentException("Missing value after --galaxy-name");
                        options.CustomGalaxyName = args[++i];
                        break;
                    case "--proton-path":
                    case "--wine-path":
                        options.ConvertPathsToWineStyle = true;
                        break;
                    case "--log-file":
                    case "--logger-file":
                        if (i + 1 >= args.Length)
                            throw new ArgumentException("Missing path after --log-file");
                        options.LoggerFilePath = args[++i];
                        break;
                    case "--exit-after-import":
                    case "--close-after-import":
                        options.ExitAfterImport = true;
                        break;
                    default:
                        throw new ArgumentException($"Unknown argument: {arg}");
                }
            }

            if (options.CheckNameResolutionRequested && string.IsNullOrWhiteSpace(options.CheckNameResolutionPath))
            {
                options.CheckNameResolutionPath = options.ImportModMergePath ?? options.ImportModPath;
                if (string.IsNullOrWhiteSpace(options.CheckNameResolutionPath))
                    throw new ArgumentException("Missing path after --check-name-resolution and no --import-mod path was provided.");
            }

            if (options.CheckSectorIslandsRequested && string.IsNullOrWhiteSpace(options.CheckSectorIslandsPath))
            {
                options.CheckSectorIslandsPath = options.ImportModMergePath ?? options.ImportModPath;
                if (string.IsNullOrWhiteSpace(options.CheckSectorIslandsPath))
                    throw new ArgumentException("Missing path after --check-sector-islands and no import path was provided.");
            }

            if (options.CheckIncludedFilesRequested && string.IsNullOrWhiteSpace(options.CheckIncludedFilesPath))
            {
                options.CheckIncludedFilesPath = options.ImportModMergePath ?? options.ImportModPath;
                if (string.IsNullOrWhiteSpace(options.CheckIncludedFilesPath))
                    throw new ArgumentException("Missing path after --check-included-files and no import path was provided.");
            }

            if (options.ListSectorNamesRequested && string.IsNullOrWhiteSpace(options.ListSectorNamesPath))
            {
                options.ListSectorNamesPath = options.ImportModMergePath ?? options.ImportModPath;
                if (string.IsNullOrWhiteSpace(options.ListSectorNamesPath))
                    throw new ArgumentException("Missing path after --list-sector-names and no import path was provided.");
            }

            if (options.CheckImportRequested && string.IsNullOrWhiteSpace(options.CheckImportPath))
            {
                options.CheckImportPath = options.ImportModMergePath ?? options.ImportModPath;
                if (string.IsNullOrWhiteSpace(options.CheckImportPath))
                    throw new ArgumentException("Missing path after --check-import and no import path was provided.");
            }

            if (options.FixSectorIslandsRequested && string.IsNullOrWhiteSpace(options.FixSectorIslandsPath))
            {
                options.FixSectorIslandsPath = options.ImportModMergePath ?? options.ImportModPath;
                if (string.IsNullOrWhiteSpace(options.FixSectorIslandsPath))
                    throw new ArgumentException("Missing path after --fix-sector-islands and no import path was provided.");
            }

            if (options.FixUnifiedSectorTranslationRequested && string.IsNullOrWhiteSpace(options.FixUnifiedSectorTranslationPath))
            {
                options.FixUnifiedSectorTranslationPath = options.ImportModMergePath ?? options.ImportModPath;
                if (string.IsNullOrWhiteSpace(options.FixUnifiedSectorTranslationPath))
                    throw new ArgumentException("Missing path after --fix-unified-sector-translation and no import path was provided.");
            }

            if (options.ConvertPathsToWineStyle)
            {
                options.ImportModPath = ConvertToWinePath(options.ImportModPath);
                options.ImportModMergePath = ConvertToWinePath(options.ImportModMergePath);
                options.CheckSectorIslandsPath = ConvertToWinePath(options.CheckSectorIslandsPath);
                options.CheckImportPath = ConvertToWinePath(options.CheckImportPath);
                options.CheckNameResolutionPath = ConvertToWinePath(options.CheckNameResolutionPath);
                options.CheckIncludedFilesPath = ConvertToWinePath(options.CheckIncludedFilesPath);
                options.ListSectorNamesPath = ConvertToWinePath(options.ListSectorNamesPath);
                options.FixSectorIslandsPath = ConvertToWinePath(options.FixSectorIslandsPath);
                options.FixUnifiedSectorTranslationPath = ConvertToWinePath(options.FixUnifiedSectorTranslationPath);
                options.LoggerFilePath = ConvertToWinePath(options.LoggerFilePath);
            }

            return options;
        }

        private static string ConvertToWinePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return path;

            string trimmed = path.Trim();
            if (trimmed.Length >= 2 && char.IsLetter(trimmed[0]) && trimmed[1] == ':')
                return trimmed;

            if (!trimmed.StartsWith("/", StringComparison.Ordinal))
                return trimmed;

            return "Z:" + trimmed.Replace('/', '\\');
        }
    }
}
