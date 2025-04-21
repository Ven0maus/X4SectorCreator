using System.Globalization;
using System.Xml.Linq;
using X4SectorCreator.Forms;

namespace X4SectorCreator.XmlGeneration
{
    internal static class PaintmodsGeneration
    {
        public static void Generate(string folder)
        {
            if (FactionsForm.AllCustomFactions.Count == 0)
            {
                return;
            }

            var paintMods = CollectPaintmods();
            if (paintMods == null) return;

            XDocument xmlDocument = new(
                new XDeclaration("1.0", "utf-8", null),
                new XElement("diff",
                    paintMods
                )
            );
            xmlDocument.Save(EnsureDirectoryExists(Path.Combine(folder, $"libraries/paintmods.xml")));
        }

        private static XElement CollectPaintmods()
        {
            var mainElement = new XElement("add", new XAttribute("sel", "/paintmods"));
            int count = 0;
            foreach (var faction in FactionsForm.AllCustomFactions.Values)
            {
                var hsv = ConvertColor(faction.Color);
                var paintmod = new XElement("paint",
                    new XAttribute("ware", $"paintmod_{faction.Id}"),
                    new XAttribute("quality", "1"),
                    new XAttribute("hue", hsv.Hue.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("brightness", hsv.Brightness.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("saturation", hsv.Saturation.ToString(CultureInfo.InvariantCulture)),
                    new XAttribute("metal", "1"),
                    new XAttribute("smooth", "1"),
                    new XAttribute("dirt", "0"));
                mainElement.Add(paintmod);
                count++;
            }
            return count == 0 ? null : mainElement;
        }

        private static string EnsureDirectoryExists(string filePath)
        {
            string directoryPath = Path.GetDirectoryName(filePath);
            if (!Directory.Exists(directoryPath))
            {
                _ = Directory.CreateDirectory(directoryPath);
            }

            return filePath;
        }

        private static PaintModColor ConvertColor(Color color)
        {
            float hue = color.GetHue(); // 0 - 360
            float saturation = color.GetSaturation(); // 0 - 1
            float brightness = color.GetBrightness(); // 0 - 1

            // OPTIONAL: Adjust saturation to expected range (-1 to 1), centered around 0
            // Depending on your system, this might be necessary:
            double adjustedSaturation = (saturation - 0.5) * 2;

            return new PaintModColor
            {
                Hue = Math.Round(hue),
                Saturation = Math.Round(adjustedSaturation, 2),
                Brightness = Math.Round(brightness, 2)
            };
        }

        class PaintModColor
        {
            public double Hue { get; set; }          // Range: 0–360
            public double Saturation { get; set; }   // Range: -1 to 1
            public double Brightness { get; set; }   // Range: 0 to 1
        }
    }
}
