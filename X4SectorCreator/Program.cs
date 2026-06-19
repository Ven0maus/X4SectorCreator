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
                Environment.ExitCode = SectorIslandCheckService.Run(startupOptions.CheckSectorIslandsPath);
                return;
            }

            if (!string.IsNullOrWhiteSpace(startupOptions.CheckImportPath))
            {
                AttachToConsole();
                Environment.ExitCode = ImportAuditService.Run(startupOptions.CheckImportPath);
                return;
            }

            ApplicationConfiguration.Initialize();

            Application.Run(new MainForm(startupOptions));
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
    }

    public sealed class StartupOptions
    {
        public string ImportModPath { get; private set; }
        public string ImportModMergePath { get; private set; }
        public string CheckSectorIslandsPath { get; private set; }
        public string CheckImportPath { get; private set; }
        public string FixSectorIslandsPath { get; private set; }
        public bool OpenGalaxyView { get; private set; }

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
                        if (i + 1 >= args.Length)
                            throw new ArgumentException("Missing path after --check-sector-islands");
                        options.CheckSectorIslandsPath = args[++i];
                        break;
                    case "--fix-sector-islands":
                    case "--fix-islands":
                        if (i + 1 >= args.Length)
                            throw new ArgumentException("Missing path after --fix-sector-islands");
                        options.FixSectorIslandsPath = args[++i];
                        break;
                    case "--check-import":
                    case "--audit-import":
                        if (i + 1 >= args.Length)
                            throw new ArgumentException("Missing path after --check-import");
                        options.CheckImportPath = args[++i];
                        break;
                    case "--open-galaxy-view":
                    case "--show-galaxy-view":
                        options.OpenGalaxyView = true;
                        break;
                    default:
                        throw new ArgumentException($"Unknown argument: {arg}");
                }
            }

            return options;
        }
    }
}
