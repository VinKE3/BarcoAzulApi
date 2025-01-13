namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vGuiaRemision
    {
        public string Id { get; set; }
        public string Serie { get; set; }
        public DateTime FechaEmision { get; set; }
        public string NumeroDocumento { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteNumero { get; set; }
        public string Placa { get; set; }
        public bool AfectarStock { get; set; }
        public bool IsAnulado { get; set; }
        public bool IsBloqueado { get; set; }
        public string NumeroFactura { get; set; }
        public string TiendaVendedor { get; set; }
    }
}
