using Newtonsoft.Json;

namespace BarcoAzul.Api.Servicios.TipoCambio.Modelos
{
    public class oConsultarTipoCambioRespuesta
    {
        [JsonProperty("success")]
        public bool Success { get; set; }
        [JsonProperty("exchange_rates")]
        public List<oRespuestaTipoCambio> TiposCambio { get; set; }
    }

    public class oRespuestaTipoCambio
    {
        [JsonProperty("fecha")]
        public DateTime Fecha { get; set; }
        [JsonProperty("moneda")]
        public string Moneda { get; set; }
        [JsonProperty("compra")]
        public decimal PrecioCompra { get; set; }
        [JsonProperty("venta")]
        public decimal PrecioVenta { get; set; }
    }
}
