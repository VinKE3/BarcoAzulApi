using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class ClienteDTO : IValidatableObject
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "El tipo de documento de identidad es requerido.")]
        public string TipoDocumentoIdentidadId { get; set; }
        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        public string NumeroDocumentoIdentidad { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        [DataType(DataType.EmailAddress, ErrorMessage = "El correo electrónico ingresado no es válido.")]
        public string CorreoElectronico { get; set; }
        [Required(ErrorMessage = "La dirección es requerida.")]
        public string DireccionPrincipal { get; set; }
        public string DepartamentoId { get; set; }
        public string ProvinciaId { get; set; }
        public string DistritoId { get; set; }
        public string ZonaId { get; set; }
        public string TipoVentaId { get; set; }
        public string TipoCobroId { get; set; }
        public decimal MaximoCreditoUSD { get; set; }
        public decimal MaximoCreditoPEN { get; set; }
        public decimal CreditoUSD { get; set; }
        public decimal CreditoPEN { get; set; }
        public string Observacion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TipoDocumentoIdentidadId != "0")
            {
                if (string.IsNullOrWhiteSpace(DepartamentoId))
                {
                    yield return new ValidationResult("El departamento es requerido.");
                }

                if (string.IsNullOrWhiteSpace(ProvinciaId))
                {
                    yield return new ValidationResult("La provincia es requerida.");
                }

                if (string.IsNullOrWhiteSpace(DistritoId))
                {
                    yield return new ValidationResult("El distrito es requerido.");
                }
            }

            if (TipoDocumentoIdentidadId == "1")
            {
                if (NumeroDocumentoIdentidad.Trim().Length != 8)
                {
                    yield return new ValidationResult("El DNI debe estar compuesto por 8 dígitos.");
                }
                else if (!Validacion.IsInteger(NumeroDocumentoIdentidad))
                {
                    yield return new ValidationResult("DNI no válido.");
                }
            }
            else if (TipoDocumentoIdentidadId == "6")
            {
                if (NumeroDocumentoIdentidad.Trim().Length != 11)
                {
                    yield return new ValidationResult("El RUC debe estar compuesto por 11 dígitos.");
                }
                else if (!Validacion.ValidarRuc(NumeroDocumentoIdentidad))
                {
                    yield return new ValidationResult("RUC no válido.");
                }
            }
        }
    }
}
