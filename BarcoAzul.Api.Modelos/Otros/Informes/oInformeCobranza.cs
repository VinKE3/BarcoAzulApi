namespace BarcoAzul.Api.Modelos.Otros.Informes
{
    public class oInformeCobranza
    {
        public DateTime? FechaEmision { get; set; }
        public DateTime? FechaVencimiento { get; set; }
        public string TipoDocumentoId { get; set; }
        public string TipoDocumentoDescripcion { get; set; }
        public string NumeroDocumento { get; set; }
        public string ClienteNombre { get; set; }
        public string MonedaId { get; set; }
        public decimal Total { get; set; }
        public decimal Saldo { get; set; }
        public decimal Abono { get; set; }
        public string PersonalNombreCompleto { get; set; }
    }
}
