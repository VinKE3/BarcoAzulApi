namespace BarcoAzul.Api.Modelos.Otros.Informes
{
    public class oRegistroCompra
    {
        public string TipoDocumentoId { get; set; }
        public DateTime? FechaContable { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string NumeroDocumento { get; set; }
        public string ProveedorNombre { get; set; }
        public string ProveedorNumeroDocumentoIdentidad { get; set; }
        public string MonedaId { get; set; }
        public decimal Total { get; set; }
    }
}
