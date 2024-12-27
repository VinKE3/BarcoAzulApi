using BarcoAzul.Api.Modelos.Entidades;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oCuentaPorCobrar
    {
        public string Id => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        public string EmpresaId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        public string MonedaId { get; set; }
        public string ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public decimal MontoDetraccion { get; set; }
        public decimal Total { get; set; }
        public decimal Abonado { get; set; }
        public decimal Saldo { get; set; }
        public string Observacion { get; set; }
        public List<oCuentaPorCobrarAbono> Abonos { get; set; }

        #region Referencias
        public oTipoDocumento TipoDocumento { get; set; }
        public oCliente Cliente { get; set; }
        public oMoneda Moneda { get; set; }
        #endregion
    }

    public class oCuentaPorCobrarAbono
    {
        public int AbonoId { get; set; }
        public DateTime Fecha { get; set; }
        public string Concepto { get; set; }
        public string MonedaId { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal Monto { get; set; }
        public string TipoCobroId { get; set; }
        public string TipoCobroDescripcion
        {
            get
            {
                return TipoCobroId switch
                {
                    "EF" => "EFECTIVO",
                    "CH" => "CHEQUE",
                    "DE" => "DEPOSITO",
                    "II" => "ING-INTER",
                    "RE" => "RETENCION",
                    "LC" => "LETRA CAMBIO",
                    "TR" => "TRANSFERENCIA",
                    "DB" => "DEP-BANCO",
                    "PL" => "PLANILLA LETRA",
                    "TJ" => "TARJETA",
                    "RI" => "RECIBO INGRESO",
                    _ => string.Empty,
                };
            }
        }
    }
}
