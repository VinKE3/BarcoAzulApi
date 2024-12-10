using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class UsuarioModificarDTO
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "El nick es requerido.")]
        public string Nick { get; set; }
        public string Observacion { get; set; }
        public bool IsActivo { get; set; }
        public bool HabilitarAfectarStock { get; set; }
        public string PersonalId { get; set; }
    }
}
