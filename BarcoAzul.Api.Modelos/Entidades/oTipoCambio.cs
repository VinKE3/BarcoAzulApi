using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oTipoCambio
    {
        public DateTime Id { get; set; }
        [Required(ErrorMessage = "El precio de compra es requerido.")]
        [Range(0.0001, 1000, ErrorMessage = "El precio de compra debe ser mayor que cero (0.00).")]
        public decimal PrecioCompra { get; set; }
        [Required(ErrorMessage = "El precio de venta es requerido.")]
        [Range(0.0001, 1000, ErrorMessage = "El precio de venta debe ser mayor que cero (0.00).")]
        public decimal PrecioVenta { get; set; }
        [Required(ErrorMessage = "El precio de producción es requerido.")]
        [Range(0.0001, 1000, ErrorMessage = "El precio de producción debe ser mayor que cero (0.00).")]


        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore]
        public string Origen { get; set; }
        #endregion
    }
}
