using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oRucDni
    {
        public string NumeroDocumentoIdentidad { get; set; }
        public string Nombre { get; set; }
        public string Direccion { get; set; }
        public string DireccionCompleta { get; set; }
        public string[] Ubigeo { get; set; }

        //Caso DNI
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Nombres { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string Apellidos { get; set; }
    }
}
