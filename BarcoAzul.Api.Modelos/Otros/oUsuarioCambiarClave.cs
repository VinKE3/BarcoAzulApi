using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oUsuarioCambiarClave
    {
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [Required(ErrorMessage = "La clave anterior es requerida.")]
        [DataType(DataType.Password)]
        public string ClaveAnterior { get; set; }
        [Required(ErrorMessage = "La clave nueva es requerida.")]
        [DataType(DataType.Password)]
        public string ClaveNueva { get; set; }
        [Required(ErrorMessage = "La confirmación de la clave nueva es requerida.")]
        [DataType(DataType.Password)]
        [Compare(nameof(ClaveNueva), ErrorMessage = "La confirmación no coincide con la clave.")]
        public string ClaveNuevaConfirmacion { get; set; }
    }
}
