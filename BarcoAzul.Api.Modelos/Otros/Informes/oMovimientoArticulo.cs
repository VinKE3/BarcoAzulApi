﻿namespace BarcoAzul.Api.Modelos.Otros.Informes
{
    public class oMovimientoArticulo
    {
        public string LineaId { get; set; }
        public string SubLineaId { get; set; }
        public string ArticuloId { get; set; }
        public string CodigoBarras { get; set; }
        public string LineaDescripcion { get; set; }
        public string SubLineaDescripcion { get; set; }
        public string MarcaNombre { get; set; }
        public string ArticuloDescripcion { get; set; }
        public string UnidadMedidaAbreviatura { get; set; }
        public string MonedaId { get; set; }
        public string EstadoStock { get; set; }
        public decimal StockInicial { get; set; }
        public decimal CantidadEntrada { get; set; }
        public decimal CantidadSalida { get; set; }
        public decimal SaldoFinal { get; set; }
        public decimal Precio {  get; set; }
        public decimal Total {  get; set; }
        public decimal StockMinimo {  get; set; }
        public decimal StockMaximo { get; set; }
    }
}
