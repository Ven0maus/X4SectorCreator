using System.Text.Json;
using System.Text.Json.Serialization;

namespace X4SectorCreator.Configuration.Converters
{
    public class ColorJsonConverter : JsonConverter<Color>
    {
        public override Color Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            using (var jsonDoc = JsonDocument.ParseValue(ref reader))
            {
                var root = jsonDoc.RootElement;
                var a = root.GetProperty("A").GetByte();
                var r = root.GetProperty("R").GetByte();
                var g = root.GetProperty("G").GetByte();
                var b = root.GetProperty("B").GetByte();
                return Color.FromArgb(a, r, g, b);
            }
        }

        public override void Write(Utf8JsonWriter writer, Color value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();
            writer.WriteNumber("A", value.A);
            writer.WriteNumber("R", value.R);
            writer.WriteNumber("G", value.G);
            writer.WriteNumber("B", value.B);
            writer.WriteEndObject();
        }
    }
}
