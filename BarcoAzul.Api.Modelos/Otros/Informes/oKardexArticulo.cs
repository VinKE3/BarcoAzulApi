namespace BarcoAzul.Api.Modelos.Otros.Informes
{
    public class oKardexArticulo
    {
        public oKardexArticulo(IEnumerable<oKardexArticuloDetalle> detalles)
        {
            Detalles = detalles.ToList();
            CompletarDatos();
        }

        public decimal EntradaCantidadTotal { get; set; }
        public decimal EntradaCostoTotal { get; set; }
        public decimal EntradaImporteTotal { get; set; }
        public decimal SalidaCantidadTotal { get; set; }
        public decimal SalidaCostoTotal { get; set; }
        public decimal SalidaImporteTotal { get; set; }
        public decimal SaldoCantidadTotal { get; set; }
        public decimal SaldoCostoTotal { get; set; }
        public decimal SaldoImporteTotal { get; set; }

        public List<oKardexArticuloDetalle> Detalles { get; set; } = new List<oKardexArticuloDetalle>();

        private void CompletarDatos()
        {
            EntradaCantidadTotal = Detalles.Sum(x => x.EntradaCantidad);
            EntradaImporteTotal = Detalles.Sum(x => x.EntradaImporte);
            SalidaCantidadTotal = Detalles.Sum(x => x.SalidaCantidad);
            SalidaImporteTotal = Detalles.Sum(x => x.SalidaImporte);

            EntradaCostoTotal = EntradaCantidadTotal == 0 ? EntradaImporteTotal : decimal.Round(decimal.Divide(EntradaImporteTotal, EntradaCantidadTotal), 2, MidpointRounding.AwayFromZero);
            SalidaCostoTotal = SalidaCantidadTotal == 0 ? SalidaImporteTotal : decimal.Round(decimal.Divide(SalidaImporteTotal, SalidaCantidadTotal), 2, MidpointRounding.AwayFromZero);

            if (Detalles.Count > 0)
            {
                SaldoCantidadTotal = Detalles.Last().SaldoCantidad;
                SaldoCostoTotal = Detalles.Last().SaldoCosto;
                SaldoImporteTotal = Detalles.Last().SaldoImporte;
            }
        }
    }

    public class oKardexArticuloDetalle
    {
        public int Numero { get; set; }
        public DateTime FechaEmision { get; set; }
        public string NumeroDocumento { get; set; }
        public string ClienteNombre { get; set; }
        public decimal EntradaCantidad { get; set; }
        public decimal EntradaCosto { get; set; }
        public decimal EntradaImporte { get; set; }
        public decimal SalidaCantidad { get; set; }
        public decimal SalidaCosto { get; set; }
        public decimal SalidaImporte { get; set; }
        public decimal SaldoCantidad { get; set; }
        public decimal SaldoCosto { get; set; }
        public decimal SaldoImporte { get; set; }
    }
}
