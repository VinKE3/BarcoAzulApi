using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class DistritoDTO
    {
        public string Id { get => $"{DepartamentoId}{ProvinciaId}{DistritoId}"; }
        [Required(ErrorMessage = "El departamento es requerido.")]
        public string DepartamentoId { get; set; }
        [Required(ErrorMessage = "La provincia es requerida.")]
        public string ProvinciaId { get; set; }
        [Required(ErrorMessage = "El código es requerido.")]
        public string DistritoId { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
    }
}
