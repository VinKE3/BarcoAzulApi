using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oEnviarDocumentoVenta
    {
        public bool Enviar { get; set; }
        [Required(ErrorMessage = "La venta es requerida.")]
        public IEnumerable<string> Ids { get; set; }
    }
}
