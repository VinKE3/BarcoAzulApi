using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oVehiculo
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

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oEmpresaTransporte EmpresaTransporte { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            NumeroPlaca = NumeroPlaca?.Trim();
            Marca = Marca?.Trim();
            Modelo = Modelo?.Trim();
            CertificadoInscripcion = CertificadoInscripcion?.Trim();
            Observacion = Observacion?.Trim();
        }
    }
}
