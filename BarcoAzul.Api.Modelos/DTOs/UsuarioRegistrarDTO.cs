using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class UsuarioRegistrarDTO
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "El nick es requerido.")]
        public string Nick { get; set; }
        public string Observacion { get; set; }
        public bool IsActivo { get; set; }
        public bool HabilitarAfectarStock { get; set; }
        public string PersonalId { get; set; }
        [Required(ErrorMessage = "La clave es requerida.")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
        [Required(ErrorMessage = "No se ingresó la confirmación de clave.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Clave), ErrorMessage = "La confirmación no coincide con la clave.")]
        public string ClaveConfirmacion { get; set; }
    }
}
