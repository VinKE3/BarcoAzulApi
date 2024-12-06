using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oLinea
    {
        public string Id { get; set; }
        [Required(ErrorMessage ="La descripción es requerida")]
        public string Descripcion { get; set; }

        public void ProcesarDatos()
        {
            Descripcion = Descripcion.Trim();
        }
    }
}
