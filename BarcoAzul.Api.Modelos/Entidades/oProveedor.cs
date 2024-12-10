using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oProveedor : IValidatableObject
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

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoDocumentoIdentidad TipoDocumentoIdentidad { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oDepartamento Departamento { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oProvincia Provincia { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oDistrito Distrito { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<oProveedorContacto> Contactos { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<oProveedorCuentaCorriente> CuentasCorrientes { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            NumeroDocumentoIdentidad = NumeroDocumentoIdentidad.Trim();
            Nombre = Nombre.Trim();
            Telefono = Telefono?.Trim();
            CorreoElectronico = CorreoElectronico?.Trim();
            DireccionPrincipal = DireccionPrincipal?.Trim();
            Condicion = Condicion?.Trim();
            Estado = Estado?.Trim();
            Observacion = Observacion?.Trim();
        }

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

    public class oProveedorContacto
    {
        public string Id { get => $"{ProveedorId}{ContactoId}"; }
        [Required(ErrorMessage = "El proveedor es requerido.")]
        public string ProveedorId { get; set; }
        public int ContactoId { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombres { get; set; }
        public string NumeroDocumentoIdentidad { get; set; }
        public string Celular { get; set; }
        public string Telefono { get; set; }
        public int? CargoId { get; set; }
        public string CorreoElectronico { get; set; }
        public string Direccion { get; set; }

        public void ProcesarDatos()
        {
            Nombres = Nombres.Trim();
            NumeroDocumentoIdentidad = NumeroDocumentoIdentidad?.Trim();
            Celular = Celular?.Trim();
            Telefono = Telefono?.Trim();
            CorreoElectronico = CorreoElectronico?.Trim();
            Direccion = Direccion?.Trim();
        }
    }

    public class oProveedorCuentaCorriente
    {
        public string Id { get => $"{ProveedorId}{CuentaCorrienteId}"; }
        [Required(ErrorMessage = "El proveedor es requerido.")]
        public string ProveedorId { get; set; }
        public int CuentaCorrienteId { get; set; }
        public string MonedaId { get; set; }
        [Required(ErrorMessage = "El número de cuenta es requerido.")]
        public string Numero { get; set; }
        [Required(ErrorMessage = "La entidad bancaria es requerida.")]
        public int EntidadBancariaId { get; set; }

        public void ProcesarDatos()
        {
            Numero = Numero?.Trim();
        }
    }
}
