namespace BarcoAzul.Api.Modelos.Otros
{
    public class oArticuloParaCuadreStock
    {
        public string LineaId { get; set; }
        public string SubLineaId { get; set; }
        public string ArticuloId { get; set; }
        public int? MarcaId { get; set; }
        public string UnidadMedidaId { get; set; }
        public string CodigoBarras { get; set; }
        public string Descripcion { get; set; }
        public string LineaDescripcion { get; set; }
        public string SubLineaDescripcion { get; set; }
        public string MarcaNombre { get; set; }
        public string UnidadMedidaDescripcion { get; set; }
        public decimal Stock { get; set; }
        public decimal PrecioCompra { get; set; }
        public string TipoExistenciaId { get; set; }
    }
}
