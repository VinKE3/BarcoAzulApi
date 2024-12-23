namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vDocumentoCompra
    {
        public string Id { get; set; }
        public DateTime FechaContable { get; set; }
        public DateTime FechaEmision { get; set; }
        public string NumeroDocumento { get; set; }
        public string ProveedorNombre { get; set; }
        public string ProveedorNumero { get; set; }
        public string MonedaId { get; set; }
        public decimal Total { get; set; }
        public bool IsCancelado { get; set; }
        public bool IsBloqueado { get; set; }
        public bool AfectarStock { get; set; }
        public string OrdenCompra { get; set; }
        public string GuiaRemision { get; set; }
    }
}
