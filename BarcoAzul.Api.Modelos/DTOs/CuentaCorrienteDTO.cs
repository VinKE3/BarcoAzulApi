using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class CuentaCorrienteDTO
    {
        public string Id { get => EmpresaId + CuentaCorrienteId; }
        public string EmpresaId { get; set; }
        public string CuentaCorrienteId { get; set; }
        [Required(ErrorMessage = "La entidad bancaria es requerida.")]
        public int EntidadBancariaId { get; set; }
        [Required(ErrorMessage = "El número de cuenta es requerido.")]
        public string Numero { get; set; }
        [Required(ErrorMessage = "El tipo de cuenta es requerido.")]
        public string TipoCuentaDescripcion { get; set; }
        public string MonedaId { get; set; }
        public string Observacion { get; set; }
    }
}
