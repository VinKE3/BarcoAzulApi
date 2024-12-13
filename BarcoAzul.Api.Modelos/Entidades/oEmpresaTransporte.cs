using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oEmpresaTransporte
    {
        public string Id { get => $"{EmpresaId}{EmpresaTransporteId}"; }
        public string EmpresaId { get; set; }
        public string EmpresaTransporteId { get; set; }
        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        public string NumeroDocumentoIdentidad { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string CorreoElectronico { get; set; }
        public string Direccion { get; set; }
        public string DepartamentoId { get; set; }
        public string ProvinciaId { get; set; }
        public string DistritoId { get; set; }
        public string Observacion { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oDepartamento Departamento { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oProvincia Provincia { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oDistrito Distrito { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            NumeroDocumentoIdentidad = NumeroDocumentoIdentidad?.Trim();
            Nombre = Nombre?.Trim();
            Telefono = Telefono?.Trim();
            Celular = Celular?.Trim();
            CorreoElectronico = CorreoElectronico?.Trim();
            Direccion = Direccion?.Trim();
            Observacion = Observacion?.Trim();
        }
    }
}
