using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vEntidadBancaria
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El tipo es requerido.")]
        public string Tipo { get; set; }
        public string NumeroDocumentoIdentidad { get; set; }
    }
}
