using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oSubLinea
    {
        public string Id { get => LineaId + SubLineaId; }

        public string SubLineaId { get; set; }
        [Required(ErrorMessage = "La línea es requerida.")]
        public string LineaId { get; set; }
        [Required(ErrorMessage = "La descripción es requerida.")]
        public string Descripcion { get; set; }

        public void ProcesarDatos()
        {
            Descripcion = Descripcion.Trim();
        }
    }
}
