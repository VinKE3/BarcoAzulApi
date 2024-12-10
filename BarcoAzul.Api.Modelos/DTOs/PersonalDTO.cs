using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class PersonalDTO
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
    }
}
