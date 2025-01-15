using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oConductor
    {
        public string Id { get; set; }
        //Esto NO inicio
        public string EmpresaId { get; set; }
        public string EmpresaTransporteId { get; set; }
        //Esto NO Fin
        [Required(ErrorMessage = "El tipo de transportista es requerido.")]
        public string TipoConductor {  get; set; }

        [Required(ErrorMessage = "El tipo de documento de identidad es requerido.")]
        public string TipoDocumentoIdentidadId { get; set; }

        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        public string NumeroDocumentoIdentidad { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]

        public string Nombre { get; set; }
        public string Apellidos {  get; set; }
        public string LicenciaConducir { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string CorreoElectronico { get; set; }
        public string Direccion { get; set; }
        public string DepartamentoId { get; set; }
        public string ProvinciaId { get; set; }
        public string DistritoId { get; set; }
        public string NumeroRegistroMTC { get; set; }

        //Esto NO inicio
        //public string Observacion { get; set; }
        //public bool IsActivo { get; set; }
        //Esto No fin

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oEmpresaTransporte EmpresaTransporte { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oDepartamento Departamento { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oProvincia Provincia { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oDistrito Distrito { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            Nombre = Nombre?.Trim();
            Apellidos = Apellidos?.Trim();
            NumeroDocumentoIdentidad = NumeroDocumentoIdentidad?.Trim();
            LicenciaConducir = LicenciaConducir?.Trim();
            Telefono = Telefono?.Trim();
            Celular = Celular?.Trim();
            CorreoElectronico = CorreoElectronico?.Trim();
            Direccion = Direccion?.Trim();
            NumeroRegistroMTC = NumeroRegistroMTC?.Trim();
        }
    }
}
