﻿namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vBloquearVenta
    {
        public string Id { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime FechaEmision { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteNumero { get; set; }
        public string MonedaId { get; set; }
        public decimal Total { get; set; }
        public bool IsBloqueado { get; set; }
    }
}
