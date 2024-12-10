using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class ProveedorDTO : IValidatableObject
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
        [Required(ErrorMessage = "El departamento es requerido.")]
        public string DepartamentoId { get; set; }
        [Required(ErrorMessage = "La provincia es requerida.")]
        public string ProvinciaId { get; set; }
        [Required(ErrorMessage = "El distrito es requerido.")]
        public string DistritoId { get; set; }
        public string Condicion { get; set; }
        public string Estado { get; set; }
        public string Observacion { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
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
