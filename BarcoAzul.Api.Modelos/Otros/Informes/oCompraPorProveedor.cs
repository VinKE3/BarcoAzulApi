namespace BarcoAzul.Api.Modelos.Otros.Informes
{
    public class oCompraPorProveedor
    {
        public int Item { get; set; }
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string NumeroDocumento { get; set; }
        public string ProveedorNumeroDocumentoIdentidad { get; set; }
        public string ProveedorNombre { get; set; }
        public string PersonalNombreCompleto { get; set; }
        public decimal Total { get; set; }
        public string ArticuloDescripcion { get; set; }
        public string UnidadMedidaDescripcion { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Importe { get; set; }
        public decimal? TotalProveedor { get; set; } //Por limitaciones del reporteador se tiene que agregar un campo para el total por proveedor
    }
}
