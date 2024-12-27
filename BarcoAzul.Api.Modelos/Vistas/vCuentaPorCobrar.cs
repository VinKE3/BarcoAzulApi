namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vCuentaPorCobrar
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string TiendaVendedor { get; set; }
        public string NumeroDocumento { get; set; }
        public string ClienteNombre { get; set; }
        public string MonedaId { get; set; }
        public decimal MontoDetraccion { get; set; }
        public decimal Total { get; set; }
        public decimal Abonado { get; set; }
        public decimal Saldo { get; set; }
        public bool IsCancelado { get; set; }
    }
}
