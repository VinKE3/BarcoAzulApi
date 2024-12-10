namespace BarcoAzul.Api.Modelos.Otros
{
    public class oArticuloValidarStock
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public decimal Stock { get; set; }
        public decimal StockSolicitado { get; set; }
        public bool ControlarStock { get; set; }
        public string DocumentoVentaCompraId { get; set; }
        public bool IsIngreso { get; set; }
    }
}
