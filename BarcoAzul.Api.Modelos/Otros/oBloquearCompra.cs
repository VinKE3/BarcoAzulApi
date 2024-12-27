using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oBloquearCompra
    {
        public bool IsBloqueado { get; set; }
        [Required(ErrorMessage = "La compra es requerida.")]
        public IEnumerable<string> Ids { get; set; }
    }
}
