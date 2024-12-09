using System.Text.Json.Serialization;


namespace BarcoAzul.Api.Modelos.DTOs
{
    public class DepartamentoViewDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<ProvinciaViewDTO> Provincias { get; set; }
    }
}
