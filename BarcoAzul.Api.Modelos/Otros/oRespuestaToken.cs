using BarcoAzul.Api.Modelos.Atributos;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oRespuestaToken
    {
        public string Token { get; set; }
        [JsonConverter(typeof(SelectiveJsonDateTimeConverter))]
        public DateTime Expiration { get; set; }
        public string RefreshToken { get; set; }
    }
}
