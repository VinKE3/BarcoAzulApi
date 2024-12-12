using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oCliente : IValidatableObject
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
        public string CodigoEstablecimiento { get; set; }
        public bool IsAgenteRetencion { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        public int? DireccionPrincipalId { get; set; }
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
        public oZona Zona { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoVentaCompra TipoVenta { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoCobroPago TipoCobro { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<oClienteDireccion> Direcciones { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<oClienteContacto> Contactos { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public IEnumerable<oClientePersonal> Personal { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            NumeroDocumentoIdentidad = NumeroDocumentoIdentidad.Trim();
            Nombre = Nombre.Trim();
            Telefono = Telefono?.Trim();
            CorreoElectronico = CorreoElectronico?.Trim();
            DireccionPrincipal = DireccionPrincipal.Trim();
            Observacion = Observacion?.Trim();
        }

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

    public class oClienteDireccion
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El cliente es requerido.")]
        public string ClienteId { get; set; }
        [Required(ErrorMessage = "La dirección es requerida.")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "El departamento es requerido.")]
        public string DepartamentoId { get; set; }
        [Required(ErrorMessage = "La provincia es requerida.")]
        public string ProvinciaId { get; set; }
        [Required(ErrorMessage = "El distrito es requerido.")]
        public string DistritoId { get; set; }
        public string Comentario { get; set; }
        [JsonIgnore]
        public string TipoDireccionId { get; set; } //01 Principal / 02 Secundaria
        public bool IsActivo { get; set; }

        public void ProcesarDatos()
        {
            Direccion = Direccion.Trim();
            Comentario = Comentario?.Trim();
        }
    }

    public class oClienteContacto
    {
        public string Id { get => $"{ClienteId}{ContactoId}"; }
        [Required(ErrorMessage = "El cliente es requerido.")]
        public string ClienteId { get; set; }
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

    public class oClientePersonal
    {
        public string Id { get => $"{ClienteId}{PersonalId}"; }
        public string ClienteId { get; set; }
        public string PersonalId { get; set; }
        public bool Default { get; set; }

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oPersonal Personal { get; set; }
        #endregion
    }
}
