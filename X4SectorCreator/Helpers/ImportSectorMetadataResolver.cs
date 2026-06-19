using System.Globalization;
using System.Xml.Linq;
using X4SectorCreator.Objects;

namespace X4SectorCreator.Helpers
{
    internal static class ImportSectorMetadataResolver
    {
        public static string ResolveOwner(XElement areaElement)
        {
            return areaElement?.Attribute("owner")?.Value;
        }

        public static List<Resource> ParseResourceAreas(XElement propertiesElement)
        {
            List<Resource> resources = [];
            if (propertiesElement == null)
                return resources;

            foreach (XElement element in propertiesElement.Elements("resourceareas").Elements("resourcearea"))
            {
                string reference = (string)element.Attribute("ref");
                if (string.IsNullOrWhiteSpace(reference))
                    continue;

                string[] parts = reference.Split('_', StringSplitOptions.RemoveEmptyEntries);
                if (parts.Length < 5 || !parts[0].Equals("sphere", StringComparison.OrdinalIgnoreCase))
                    continue;

                if (!int.TryParse((string)element.Attribute("amount"), NumberStyles.Integer, CultureInfo.InvariantCulture, out int amount))
                    amount = 1;

                resources.Add(new Resource
                {
                    Size = parts[1],
                    Ware = parts[2],
                    Yield = parts[3],
                    Speed = parts[4],
                    Amount = amount,
                });
            }

            return resources;
        }
    }
}
