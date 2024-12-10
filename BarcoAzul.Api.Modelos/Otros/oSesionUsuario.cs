using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSesionUsuario
    {
        [Required(ErrorMessage = "El usuario es requerido.")]
        public string Usuario { get; set; }

        [Required(ErrorMessage = "La clave es requerida.")]
        public string Clave { get; set; }
    }
}
