using Newtonsoft.Json;

namespace BarcoAzul.Api.Servicios.RucDni.Modelos
{
    public class oConsultarRucDniRespuesta
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("data")]
        public oRespuestaRucDni Data { get; set; }
        [JsonProperty("message")]
        public string Message { get; set; }
    }

    public class oRespuestaRucDni
    {
        [JsonProperty("ruc")]
        public string Ruc { get; set; }
        [JsonProperty("nombre_o_razon_social")]
        public string RazonSocial { get; set; }
        [JsonProperty("estado")]
        public string Estado { get; set; }
        [JsonProperty("condicion")]
        public string Condicion { get; set; }
        [JsonProperty("ubigeo")]
        public string[] Ubigeo { get; set; }
        [JsonProperty("direccion")]
        public string Direccion { get; set; }
        [JsonProperty("direccion_completa")]
        public string DireccionCompleta { get; set; }

        [JsonProperty("numero")]
        public string Numero { get; set; }
        [JsonProperty("nombre_completo")]
        public string NombreCompleto { get; set; }
        [JsonProperty("nombres")]
        public string Nombres { get; set; }
        [JsonProperty("apellido_paterno")]
        public string ApellidoPaterno { get; set; }
        [JsonProperty("apellido_materno")]
        public string ApellidoMaterno { get; set; }
    }
}
