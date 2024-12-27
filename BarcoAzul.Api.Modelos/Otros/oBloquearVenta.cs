using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oBloquearVenta
    {
        public bool IsBloqueado { get; set; }
        [Required(ErrorMessage = "La venta es requerida.")]
        public IEnumerable<string> Ids { get; set; }
    }
}
