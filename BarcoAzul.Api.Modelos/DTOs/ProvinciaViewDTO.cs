using System.Text.Json.Serialization;


namespace BarcoAzul.Api.Modelos.DTOs
{
    public class ProvinciaViewDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<DistritoViewDTO> Distritos { get; set; }
    }
}
