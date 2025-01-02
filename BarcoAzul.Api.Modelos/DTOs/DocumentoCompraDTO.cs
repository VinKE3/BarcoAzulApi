using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class DocumentoCompraDTO : IValidatableObject
    {
        public string Id => $"{EmpresaId}{ProveedorId}{TipoDocumentoId}{Serie}{Numero}{ClienteId}";
        public string EmpresaId { get; set; }
        [Required(ErrorMessage = "El proveedor es requerido.")]
        public string ProveedorId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public string ClienteId { get; set; }
        [Required(ErrorMessage = "La fecha de emisión es requerida.")]
        public DateTime FechaEmision { get; set; }
        [Required(ErrorMessage = "La fecha contable es requerida.")]
        public DateTime FechaContable { get; set; }
        [Required(ErrorMessage = "La fecha de vencimiento es requerida.")]
        public DateTime FechaVencimiento { get; set; }
        public string ProveedorNombre { get; set; }
        public string ProveedorNumeroDocumentoIdentidad { get; set; }
        public string ProveedorDireccion { get; set; }
        public string TipoCompraId { get; set; }
        public string MonedaId { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00)")]
        public decimal TipoCambio { get; set; }
        public string TipoPagoId { get; set; }
        public string NumeroOperacion { get; set; }
        public string CuentaCorrienteId { get; set; }
        public string DocumentoReferenciaId { get; set; }
        public bool Abonar { get; set; }
        public string MotivoNotaId { get; set; }
        public string MotivoSustento { get; set; }
        public string GuiaRemision { get; set; }
        public string Observacion { get; set; }
        public decimal SubTotal { get; set; }
        public decimal PorcentajeIGV { get; set; }
        public decimal MontoIGV { get; set; }
        public decimal TotalNeto { get; set; }
        [Required(ErrorMessage = "El total es requerido.")]
        [Range(0.01, int.MaxValue, ErrorMessage = "El total no puede ser igual a cero (0.00)")]
        public decimal Total { get; set; }
        public bool IncluyeIGV { get; set; }
        public bool AfectarStock { get; set; }
        public bool AfectarPrecio { get; set; }
        public List<oDocumentoCompraDetalle> Detalles { get; set; }
        public List<oDocumentoCompraOrdenCompraRelacionada> OrdenesCompraRelacionadas { get; set; }
        public string NumeroDocumento => Comun.CompraIdADocumento(Id);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || Detalles.Count == 0)
                yield return new ValidationResult("No existen detalles.");

            if (TipoCompraId == "CO" && (TipoPagoId == "DE" || TipoPagoId == "CH"))
            {
                if (string.IsNullOrEmpty(CuentaCorrienteId))
                    yield return new ValidationResult("La cuenta corriente es requerida.");

                if (string.IsNullOrWhiteSpace(NumeroOperacion))
                    yield return new ValidationResult("El número de operación es requerido.");
            }

            if (TipoDocumentoId == "07" || TipoDocumentoId == "08")
            {
                if (string.IsNullOrWhiteSpace(MotivoNotaId))
                    yield return new ValidationResult("El motivo de la nota es requerido.");
            }
        }
    }
}
