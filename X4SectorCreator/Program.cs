using System.Globalization;

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
            Application.Run(new MainForm());
        }
    }
}