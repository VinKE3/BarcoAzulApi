namespace BarcoAzul.Api.Modelos.Otros.Informes
{
    public class oRegistroVenta
    {
        public DateTime? FechaEmision { get; set; }
        public string NumeroDocumento { get; set; }
        public string ClienteNombre { get; set; }
        public string MonedaId { get; set; }
        public decimal Total { get; set; }
        public string TipoCobroDescripcion { get; set; }
        public string GuiaRemision { get; set; }
        public string PersonalNombreCompleto { get; set; }
        public string OrdenPedido { get; set; }
        public bool IsAnulado { get; set; }
    }
}
