using BarcoAzul.Api.Modelos.Entidades;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oCuentaPorPagar
    {
        public string Id => $"{EmpresaId}{ProveedorId}{TipoDocumentoId}{Serie}{Numero}{ClienteId}";
        public string EmpresaId { get; set; }
        public string ProveedorId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public string ClienteId { get; set; }
        public DateTime FechaContable { get; set; }
        public string MonedaId { get; set; }
        public decimal Total { get; set; }
        public decimal Abonado { get; set; }
        public decimal Saldo { get; set; }
        public string Observacion { get; set; }
        public List<oCuentaPorPagarAbono> Abonos { get; set; }

        #region Referencias
        public oTipoDocumento TipoDocumento { get; set; }
        public oProveedor Proveedor { get; set; }
        public oMoneda Moneda { get; set; }
        #endregion
    }

    public class oCuentaPorPagarAbono
    {
        public int AbonoId { get; set; }
        public DateTime Fecha { get; set; }
        public string Concepto { get; set; }
        public string MonedaId { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal Monto { get; set; }
        public decimal MontoPEN { get; set; }
        public decimal MontoUSD { get; set; }
        public string TipoPagoId { get; set; }
        public string TipoPagoDescripcion
        {
            get
            {
                return TipoPagoId switch
                {
                    "EF" => "EFECTIVO",
                    "CH" => "CHEQUE",
                    "DE" => "DEPOSITO",
                    "EI" => "EGR-INTER",
                    "LC" => "LETRA CAMBIO",
                    "PE" => "PERCEPCION",
                    "TR" => "TRANSFERENCIA",
                    "TJ" => "TARJETA",
                    "FE" => "OPERACION FEC",
                    "RE" => "RECIBO EGRESO",
                    "FN" => "NEGOCIABLE",
                    "CF" => "C.E.F.",
                    _ => string.Empty,
                };
            }
        }
    }
}
