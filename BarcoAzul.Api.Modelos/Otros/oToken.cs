using System.ComponentModel.DataAnnotations;


namespace BarcoAzul.Api.Modelos.Otros
{
    public class oToken
    {
        [Required(ErrorMessage = "El token de acceso es requerido.")]
        public string Token { get; set; }
        [Required(ErrorMessage = "El token de actualización es requerido.")]
        public string RefreshToken { get; set; }
    }
}
