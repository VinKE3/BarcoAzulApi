using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vMovimientoBancario
    {
        public string Id { get; set; }
        public string CuentaBancaria { get; set; }
        public DateTime FechaEmision { get; set; }
        public string TipoMovimientoId { get; set; }
        public string TipoOperacionId { get; set; }
        public string NumeroOperacion { get; set; }
        public string ClienteProveedorNombre { get; set; }
        public string TipoBeneficiarioId { get; set; }
        public string Concepto { get; set; }
        public decimal Monto { get; set; }
        public decimal ITF { get; set; }
        public decimal Total { get; set; }
        public bool IsBloqueado { get; set; }
    }
}
