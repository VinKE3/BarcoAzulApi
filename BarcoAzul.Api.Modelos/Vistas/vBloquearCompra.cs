namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vBloquearCompra
    {
        public string Id { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaContable { get; set; }
        public string ProveedorNombre { get; set; }
        public string ProveedorNumero { get; set; }
        public string MonedaId { get; set; }
        public decimal Total { get; set; }
        public bool IsBloqueado { get; set; }
    }
}
