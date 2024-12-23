namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vCuentaPorPagar
    {
        public string Id { get; set; }
        public DateTime FechaContable { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string NumeroDocumento { get; set; }
        public string ProveedorNombre { get; set; }
        public string MonedaId { get; set; }
        public decimal Total { get; set; }
        public decimal Abonado { get; set; }
        public decimal Saldo { get; set; }
        public bool IsCancelado { get; set; }
    }
}
