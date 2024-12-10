using BarcoAzul.Api.Modelos.Otros;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oPersonal
    {
        public string Id { get; set; }
        public string NumeroDocumentoIdentidad { get; set; }
        [Required(ErrorMessage = "El apellido paterno es requerido.")]
        [MinLength(2, ErrorMessage = "La longitud mínima del apellido paterno es 2.")]
        public string ApellidoPaterno { get; set; }
        public string ApellidoMaterno { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        [MinLength(2, ErrorMessage = "La longitud mínima del nombre es 2.")]
        public string Nombres { get; set; }
        public string DepartamentoId { get; set; }
        public string ProvinciaId { get; set; }
        public string DistritoId { get; set; }
        public string Direccion { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string SexoId { get; set; }
        public string EstadoCivilId { get; set; }
        public string CorreoElectronico { get; set; }
        public int? CargoId { get; set; }
        public string Observacion { get; set; }
        public int? EntidadBancariaId { get; set; }
        public string TipoCuentaBancariaId { get; set; }
        public string MonedaId { get; set; }
        public string CuentaCorriente { get; set; }
        public bool IsActivo { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore]
        public string NombreCompleto => $"{ApellidoPaterno} {ApellidoMaterno} {Nombres}";
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oDepartamento Departamento { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oProvincia Provincia { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oDistrito Distrito { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oSexo Sexo { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oEstadoCivil EstadoCivil { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oCargo Cargo { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oEntidadBancaria EntidadBancaria { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoCuentaBancaria TipoCuentaBancaria { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            NumeroDocumentoIdentidad = NumeroDocumentoIdentidad?.Trim();
            ApellidoPaterno = ApellidoPaterno.Trim();
            ApellidoMaterno = ApellidoMaterno?.Trim();
            Nombres = Nombres.Trim();
            Direccion = Direccion?.Trim();
            Telefono = Telefono?.Trim();
            Celular = Celular?.Trim();
            CorreoElectronico = CorreoElectronico?.Trim();
            Observacion = Observacion?.Trim();
            CuentaCorriente = CuentaCorriente?.Trim();
        }
    }
}
