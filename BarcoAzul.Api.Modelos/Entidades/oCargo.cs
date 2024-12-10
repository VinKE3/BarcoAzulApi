using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oCargo
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "La descripción es requerida.")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "El sueldo es requerido.")]
        [Range(0.00, 9999999.99, ErrorMessage = "El sueldo no puede ser menor a cero.")]
        public decimal Sueldo { get; set; }

        public void ProcesarDatos()
        {
            Descripcion = Descripcion.Trim();
        }
    }
}
