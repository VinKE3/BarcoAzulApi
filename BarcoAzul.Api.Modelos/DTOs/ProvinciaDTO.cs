using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class ProvinciaDTO
    {
        public string Id { get => $"{DepartamentoId}{ProvinciaId}"; }
        [Required(ErrorMessage = "El departamento es requerido.")]
        public string DepartamentoId { get; set; }
        [Required(ErrorMessage = "El código es requerido.")]
        public string ProvinciaId { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
    }
}
