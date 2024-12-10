namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vCuadreStock
    {
        public string Id { get; set; }
        public bool Estado { get; set; }
        public bool Pendiente { get; set; }
        public DateTime FechaRegistro { get; set; }
        public string Numero { get; set; }
        public string ResponsableNombreCompleto { get; set; }
        public string MonedaId { get; set; }
        public decimal TotalSobra { get; set; }
        public decimal TotalFalta { get; set; }
        public decimal SaldoTotal { get; set; }
    }
}
