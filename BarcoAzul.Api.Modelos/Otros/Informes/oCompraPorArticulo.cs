namespace BarcoAzul.Api.Modelos.Otros.Informes
{
    public class oCompraPorArticulo
    {
        public string SubLineaDescripcion { get; set; }
        public string ArticuloId { get; set; }
        public string ArticuloDescripcion { get; set; }
        public DateTime? FechaEmision { get; set; }
        public string NumeroDocumento { get; set; }
        public string ProveedorNombre { get; set; }
        public string ProveedorNumeroDocumentoIdentidad { get; set; }
        public string UnidadMedidaAbreviatura { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Importe => decimal.Round(Cantidad * PrecioUnitario, 2, MidpointRounding.AwayFromZero);
    }
}
