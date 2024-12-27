using BarcoAzul.Api.Utilidades;

namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vCuentaPorCobrarPendiente
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string ClienteNumero { get; set; }
        public string ClienteNombre { get; set; }
        public string MonedaId { get; set; }
        public decimal Saldo { get; set; }
        public string Descripcion { get; set; }
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);
    }
}
