﻿namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vSalidaAlmacen
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public string HoraEmision { get; set; }
        public string NumeroDocumento { get; set; }
        public string PersonalNombre { get; set; }
        public string Concepto { get; set; }
        public string Observacion { get; set; }
        public string MonedaId { get; set; }
        public string LineaProduccion { get; set; }
        public string GuiaRemision { get; set; }
        public decimal Total { get; set; }
        public bool IsCancelado { get; set; }
        public bool IsBloqueado { get; set; }
    }
}
