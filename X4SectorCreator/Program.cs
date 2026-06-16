using System.Globalization;
using System.Linq;

namespace X4SectorCreator
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            CultureInfo invariantCulture = CultureInfo.InvariantCulture;
            Thread.CurrentThread.CurrentCulture = invariantCulture;
            Thread.CurrentThread.CurrentUICulture = invariantCulture;
            ApplicationConfiguration.Initialize();

            StartupOptions startupOptions;
            try
            {
                startupOptions = StartupOptions.Parse(Environment.GetCommandLineArgs().Skip(1).ToArray());
            }
            catch (Exception ex)
            {
                _ = MessageBox.Show(ex.Message, "Invalid command line arguments", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            Application.Run(new MainForm(startupOptions));
        }
    }

    public sealed class StartupOptions
    {
        public string ImportModPath { get; private set; }
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
