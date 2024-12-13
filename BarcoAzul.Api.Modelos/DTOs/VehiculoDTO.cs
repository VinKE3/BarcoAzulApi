using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class VehiculoDTO
    {
        public string Id { get; set; }
        public string EmpresaId { get; set; }
        [Required(ErrorMessage = "La empresa de transporte es requerida.")]
        public string EmpresaTransporteId { get; set; }
        [Required(ErrorMessage = "El número de placa es requerido.")]
        public string NumeroPlaca { get; set; }
        public string Marca { get; set; }
        public string Modelo { get; set; }
        public string CertificadoInscripcion { get; set; }
        public string Observacion { get; set; }
    }
}
