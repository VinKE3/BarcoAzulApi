namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oDetalleOperacion
    {
        public string Id => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        public string EmpresaId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public int DetalleId { get; set; }
        public string LineaId { get; set; }
        public string SubLineaId { get; set; }
        public string ArticuloId { get; set; }
        public string Descripcion { get; set; }
        public string UnidadMedidaId { get; set; }
        public decimal Cantidad { get; set; }
    }
}
