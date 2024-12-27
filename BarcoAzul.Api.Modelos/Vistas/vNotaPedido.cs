namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vNotaPedido
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public string NumeroDocumento { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteNumero { get; set; }
        public string MonedaId { get; set; }
        public decimal Total { get; set; }
        public bool IsAnulado { get; set; }
        public bool IsBloqueado { get; set; }
        public bool IsFacturado { get; set; }
        public string DocumentoReferencia { get; set; }
        public string PersonalNombre { get; set; }
    }
}
