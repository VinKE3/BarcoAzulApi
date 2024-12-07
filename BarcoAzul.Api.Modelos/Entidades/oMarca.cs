using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oMarca
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }

        public void ProcesarDatos()
        {
            Nombre = Nombre.Trim();
        }
    }
}
