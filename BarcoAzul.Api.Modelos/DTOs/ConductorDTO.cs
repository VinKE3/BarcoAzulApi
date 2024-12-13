using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class ConductorDTO
    {
        public string Id { get; set; }
        //Esto NO inicio
        public string EmpresaId { get; set; }
        public string EmpresaTransporteId { get; set; }
        //Esto NO Fin
        [Required(ErrorMessage = "El tipo de transportista es requerido.")]
        public string TipoConductor { get; set; }

        [Required(ErrorMessage = "El tipo de documento de identidad es requerido.")]
        public string TipoDocumentoIdentidad { get; set; }

        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        public string NumeroDocumentoIdentidad { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        public string Apellidos { get; set; }
        public string LicenciaConducir { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string CorreoElectronico { get; set; }
        public string Direccion { get; set; }
        public string DepartamentoId { get; set; }
        public string ProvinciaId { get; set; }
        public string DistritoId { get; set; }
        public string NroRegistro { get; set; }
    }
}
