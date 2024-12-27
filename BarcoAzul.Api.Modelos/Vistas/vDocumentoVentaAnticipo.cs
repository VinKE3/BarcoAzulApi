namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vDocumentoVentaAnticipo
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public string NumeroDocumento { get; set; }
        public string MonedaId { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
    }
}
