using BarcoAzul.Api.Modelos.Entidades;
using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oUsuarioConfiguracionPermisos
    {
        [Required(ErrorMessage = "El usuario es requerido.")]
        public string UsuarioId { get; set; }
        [Required(ErrorMessage = "El tipo de usuario es requerido.")]
        public string TipoUsuarioId { get; set; }
        public List<oUsuarioPermiso> Permisos { get; set; }
    }
}
