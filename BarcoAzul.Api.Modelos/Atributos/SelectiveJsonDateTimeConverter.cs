using System.Text.Json;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Atributos
{
    public class SelectiveJsonDateTimeConverter : JsonConverter<DateTime>
    {
        private const string Format = "s";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.ToString(Format));
        }
    }
}
