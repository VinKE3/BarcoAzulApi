using BarcoAzul.Api.Modelos.Entidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class MovimientoBancarioDTO
    {
        public string Id { get; set; }
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string TipoDocumentoId { get; set; }
        [Required(ErrorMessage = "La cuenta corriente es requerida.")]
        public string CuentaCorrienteId { get; set; }
        [Required(ErrorMessage = "La fecha de emisión es requerida.")]
        public DateTime FechaEmision { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00)")]
        public decimal TipoCambio { get; set; }
        public string TipoMovimientoId { get; set; }
        public string TipoOperacionId { get; set; }
        [Required(ErrorMessage = "El número de operación es requerido.")]
        public string NumeroOperacion { get; set; }
        public bool IsCierreCaja { get; set; }
        public string TipoBeneficiarioId { get; set; }
        public string ClienteProveedorId { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string ClienteProveedorNombre { get; set; }
        public string Concepto { get; set; }
        public string DocumentoReferencia { get; set; }
        public bool TieneDetraccion { get; set; }
        public decimal PorcentajeITF { get; set; }
        public decimal MontoITF { get; set; }
        public decimal MontoInteres { get; set; }
        public decimal Monto { get; set; }
        [Required(ErrorMessage = "El total es requerido.")]
        [Range(0.01, int.MaxValue, ErrorMessage = "El total no puede ser igual a cero (0.00)")]
        public decimal Total { get; set; }
        public bool TieneCuentaDestino { get; set; }
        public string CuentaDestinoId { get; set; }
        public string MonedaId { get; set; }
        public List<oMovimientoBancarioDetalle> Detalles { get; set; }
    }
}
