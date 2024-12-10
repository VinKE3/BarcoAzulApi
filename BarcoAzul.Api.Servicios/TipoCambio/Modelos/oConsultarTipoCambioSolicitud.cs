using Newtonsoft.Json;

namespace BarcoAzul.Api.Servicios.TipoCambio.Modelos
{
    public class oConsultarTipoCambioSolicitud
    {
        [JsonProperty("token")]
        public string Token { get; set; }
        [JsonProperty("tipo_cambio")]
        public oParametroTipoCambio TipoCambio { get; set; }
    }

    public class oParametroTipoCambio
    {
        [JsonProperty("moneda")]
        public string Moneda { get; set; }
        [JsonProperty("fecha_inicio")]
        public string FechaInicio { get; set; }
        [JsonProperty("fecha_fin")]
        public string FechaFin { get; set; }
    }
}
