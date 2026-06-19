using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;
using X4SectorCreator.Configuration;

namespace X4SectorCreator
{
    internal static class Program
    {
        private const uint AttachParentProcess = 0xFFFFFFFF;

        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = invariantCulture;
            Thread.CurrentThread.CurrentUICulture = invariantCulture;

            StartupOptions startupOptions;
            try
            {
                startupOptions = StartupOptions.Parse(Environment.GetCommandLineArgs().Skip(1).ToArray());
            }
            catch (Exception ex)
            {
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

                ApplicationConfiguration.Initialize();
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
                string logsDirectory = Path.Combine(AppContext.BaseDirectory, "logs");
                _ = Directory.CreateDirectory(logsDirectory);

                string fileName = $"startup-error-{DateTime.Now:yyyyMMdd-HHmmss}.log";
                string fullPath = Path.Combine(logsDirectory, fileName);
                string contents =
                    $"{caption}{Environment.NewLine}" +
                    $"Timestamp: {DateTime.Now:O}{Environment.NewLine}" +
                    $"Command line: {Environment.CommandLine}{Environment.NewLine}{Environment.NewLine}" +
                    ex;

                File.WriteAllText(fullPath, contents);
                return fullPath;
            }
            catch
            {
                return null;
            }
        }

        private static void AttachToConsole()
        {
            if (!AttachConsole(AttachParentProcess))
            {
                _ = AllocConsole();
            }

            var standardOut = new StreamWriter(Console.OpenStandardOutput()) { AutoFlush = true };
            Console.SetOut(standardOut);

            var standardError = new StreamWriter(Console.OpenStandardError()) { AutoFlush = true };
            Console.SetError(standardError);
        }

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AttachConsole(uint dwProcessId);

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
        public string FixSectorIslandsPath { get; private set; }
        public string FixUnifiedSectorTranslationPath { get; private set; }
        public string CustomGalaxyName { get; private set; }
        public bool DisableAllStorylines { get; private set; }
        public bool EnableCustomGalaxy { get; private set; }
        public bool ExitAfterImport { get; private set; }
        public bool OpenGalaxyView { get; private set; }
        private bool ConvertPathsToWineStyle { get; set; }
        private bool CheckSectorIslandsRequested { get; set; }
        private bool CheckImportRequested { get; set; }
        private bool CheckNameResolutionRequested { get; set; }
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
                options.FixSectorIslandsPath = ConvertToWinePath(options.FixSectorIslandsPath);
                options.FixUnifiedSectorTranslationPath = ConvertToWinePath(options.FixUnifiedSectorTranslationPath);
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
