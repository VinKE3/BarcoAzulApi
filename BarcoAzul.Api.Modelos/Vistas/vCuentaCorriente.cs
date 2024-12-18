namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vCuentaCorriente
    {
        public string Id { get; set; }
        public string EmpresaId { get; set; }
        public string CuentaCorrienteId { get; set; }
        public string Numero { get; set; }
        public string EntidadBancariaNombre { get; set; }
        public string EntidadBancariaTipo { get; set; }
        public string MonedaId { get; set; }
        public string TipoCuentaDescripcion { get; set; }
        public decimal SaldoFinal { get; set; }
    }
}
