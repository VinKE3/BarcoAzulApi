namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oDetalle
    {
        public int DetalleId { get; set; }
        /// <summary>
        /// Id completa del Artículo
        /// </summary>
        public string Id => $"{LineaId}{SubLineaId}{ArticuloId}";
        public string LineaId { get; set; }
        public string SubLineaId { get; set; }
        public string ArticuloId { get; set; }
        public string UnidadMedidaId { get; set; }
        public int? MarcaId { get; set; }
        public string Descripcion { get; set; }
        public string CodigoBarras { get; set; }
        public decimal Cantidad { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal SubTotal { get; set; }
        public decimal MontoIGV { get; set; }
        public decimal Importe { get; set; }
        public string Presentacion { get; set; }
        public decimal TotalPeso { get; set; }
        public string UnidadMedidaDescripcion { get; set; }
    }
}
