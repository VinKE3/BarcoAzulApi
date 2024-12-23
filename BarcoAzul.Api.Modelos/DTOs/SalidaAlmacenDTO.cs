using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class SalidaAlmacenDTO : IValidatableObject
    {
        public string Id => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        public string EmpresaId { get; set; }
        public string TipoDocumentoId { get; set; }
        [Required(ErrorMessage = "La serie es requerida.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "La serie debe tener 4 caracteres.")]
        public string Serie { get; set; }
        public string Numero { get; set; }
        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        public DateTime FechaInicio { get; set; }
        [Required(ErrorMessage = "La fecha de terminación es requerida.")]
        public DateTime FechaTerminacion { get; set; }
        [Required(ErrorMessage = "El cliente es requerido.")]
        public string ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteNumeroDocumentoIdentidad { get; set; }
        public string MonedaId { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00)")]
        public decimal TipoCambio { get; set; }
        public string PersonalId { get; set; }
        public string LineaProduccion { get; set; }
        public string Envasado { get; set; }
        public string GuiaRemision { get; set; }
        public string Observacion { get; set; }
        public bool IncluyeIGV { get; set; }
        public decimal PorcentajeIGV { get; set; }
        public decimal GastosIndirectos { get; set; }
        public string CantidadSolicitada { get; set; }
        public string CantidadProducida { get; set; }
        [Required(ErrorMessage = "El total es requerido.")]
        public decimal Total { get; set; }
        public decimal TotalGalones { get; set; }
        public decimal CostoGalon { get; set; }
        public decimal CostoGalonMasGastoIndirectos { get; set; }
        public decimal CostoGalonMasIGV { get; set; }
        public List<oSalidaAlmacenDetalle> Detalles { get; set; }
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || !Detalles.Any())
                yield return new ValidationResult("No existen detalles.");
        }
    }
}
