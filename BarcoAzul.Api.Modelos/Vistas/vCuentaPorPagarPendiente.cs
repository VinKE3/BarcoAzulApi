using BarcoAzul.Api.Utilidades;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vCuentaPorPagarPendiente
    {
        public string Id { get; set; }
        public DateTime FechaContable { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string MonedaId { get; set; }
        public decimal Saldo { get; set; }
        public string Descripcion { get; set; }
        public string NumeroDocumento => Comun.CompraIdADocumento(Id);
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string DocumentoRelacionado { get; set; }
        public string OrdenCompraRelacionada { get; set; }
    }
}
