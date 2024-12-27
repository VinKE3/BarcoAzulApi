using BarcoAzul.Api.Modelos.Atributos;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oCorrelativo
    {
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [Required(ErrorMessage = "El tipo de documento es requerido.")]
        public string TipoDocumentoId { get; set; }
        public string TipoDocumentoDescripcion { get; set; }
        [Required(ErrorMessage = "La serie es requerida.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "La serie debe tener 4 caracteres.")]
        [NoSpaces(ErrorMessage = "La serie no puede contener espacios en blanco.")]
        public string Serie { get; set; }
        [Required(ErrorMessage = "El número es requerido.")]
        public long Numero { get; set; }

        public void ProcesarDatos()
        {
            Serie = Serie?.Trim().ToUpper();
        }
    }
}
