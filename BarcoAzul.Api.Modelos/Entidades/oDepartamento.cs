using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oDepartamento
    {
        [Required(ErrorMessage = "El código es requerido.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "El código debe tener 2 dígitos.")]
        public string Id { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }

        public void ProcesarDatos()
        {
            Nombre = Nombre.Trim();
        }
    }
}
