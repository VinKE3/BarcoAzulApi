namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vBloquearMovimientoBancario
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public string NumeroOperacion { get; set; }
        public string Concepto { get; set; }
        public string MonedaId { get; set; }
        public decimal Monto { get; set; }
        public bool IsBloqueado { get; set; }
    }
}
