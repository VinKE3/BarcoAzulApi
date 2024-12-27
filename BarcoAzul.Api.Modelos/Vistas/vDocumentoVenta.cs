namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vDocumentoVenta
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public string NumeroDocumento { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteNumero { get; set; }
        public string MonedaId { get; set; }
        public decimal Total { get; set; }
        public bool IsCancelado { get; set; }
        public bool AfectarStock { get; set; }
        public bool IsAnulado { get; set; }
        public bool IsBloqueado { get; set; }
        public string Cotizacion { get; set; }
        public string OrdenPedido { get; set; }
        public string GuiaRemision { get; set; }
        public string TiendaVendedor { get; set; }
        public bool IsEnviado { get; set; }
        public bool Enviar { get; set; }
    }
}
