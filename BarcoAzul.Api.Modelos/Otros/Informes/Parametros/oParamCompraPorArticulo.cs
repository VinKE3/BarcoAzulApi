namespace BarcoAzul.Api.Modelos.Otros.Informes.Parametros
{
    public class oParamCompraPorArticulo
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        /// <summary>
        /// CA = Compra por Artículo | AC = Artículos más comprados | AP = Artículos Producción
        /// </summary>
        public string TipoReporte { get; set; }
    }
}
