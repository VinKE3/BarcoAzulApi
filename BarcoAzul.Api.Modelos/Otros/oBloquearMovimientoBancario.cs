using System.ComponentModel.DataAnnotations;


namespace BarcoAzul.Api.Modelos.Otros
{
    public class oBloquearMovimientoBancario
    {
        public bool IsBloqueado { get; set; }
        [Required(ErrorMessage = "El movimiento bancario es requerido.")]
        public IEnumerable<string> Ids { get; set; }
    }
}
