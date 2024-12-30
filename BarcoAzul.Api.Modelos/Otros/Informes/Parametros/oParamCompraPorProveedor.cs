namespace BarcoAzul.Api.Modelos.Otros.Informes.Parametros
{
    public class oParamCompraPorProveedor
    {
        public string MonedaId { get; set; }
        public string ProveedorId { get; set; }
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string TipoReporte { get; set; }
    }
}
