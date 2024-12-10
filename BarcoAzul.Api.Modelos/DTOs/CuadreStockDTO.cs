using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class CuadreStockDTO : IValidatableObject
    {
        public string Id => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        public string EmpresaId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        [Required(ErrorMessage = "La fecha es requerida.")]
        public DateTime FechaRegistro { get; set; }
        public string HoraRegistro => DateTime.Now.ToString("HH:mm:ss");
        public string MonedaId { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00).")]
        public decimal TipoCambio { get; set; }
        [Required(ErrorMessage = "El responsable es requerido.")]
        public string ResponsableId { get; set; }
        public string Observacion { get; set; }
        public decimal TotalSobra { get; set; }
        public decimal TotalFalta { get; set; }
        public decimal SaldoTotal { get; set; }
        public List<oCuadreStockDetalle> Detalles { get; set; }
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || Detalles.Count == 0)
                yield return new ValidationResult("No existen detalles.");
        }
    }
}
