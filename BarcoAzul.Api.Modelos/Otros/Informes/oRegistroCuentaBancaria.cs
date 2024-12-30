namespace BarcoAzul.Api.Modelos.Otros.Informes
{
    public class oRegistroCuentaBancaria
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public string TipoOperacionDescripcion { get; set; }
        public string NumeroOperacion { get; set; }
        public string PersonalNombreCompleto { get; set; }
        public string ClienteProveedorNombre { get; set; }
        public string Concepto { get; set; }
        public string DocumentoReferencia { get; set; }
        public decimal Ingreso { get; set; }
        public decimal Egreso { get; set; }
        public decimal Saldo { get; set; }
    }
}
