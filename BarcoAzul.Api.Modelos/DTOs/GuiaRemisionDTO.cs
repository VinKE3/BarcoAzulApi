using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class GuiaRemisionDTO : IValidatableObject
    {
        public string Id => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        public string EmpresaId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        [Required(ErrorMessage = "La fecha de emisión es requerida.")]
        public DateTime FechaEmision { get; set; }
        [Required(ErrorMessage = "La fecha de traslado es requerida.")]
        public DateTime FechaTraslado { get; set; }
        [Required(ErrorMessage = "El cliente es requerido.")]
        public string ClienteId { get; set; }
        public string ClienteNumeroDocumentoIdentidad { get; set; }
        public string ClienteNombre { get; set; }
        public string PersonalId { get; set; }
        public string DireccionPartida { get; set; }
        public int ClienteDireccionId { get; set; }
        public string ClienteDireccion { get; set; }
        public string EmpresaTransporteId { get; set; }
        public decimal CostoMinimo { get; set; }
        public string ConductorId { get; set; }
        public string LicenciaConducir { get; set; }
        public string VehiculoId { get; set; }
        public string ConstanciaInscripcion { get; set; }
        [Required(ErrorMessage = "El motivo de traslado es requerido.")]
        public string MotivoTrasladoId { get; set; }
        [Required(ErrorMessage = "El sustento de traslado es requerido.")]
        public string MotivoSustento { get; set; }
        public string IngresoEgresoStock { get; set; }
        public string NumeroFactura { get; set; }
        public string OrdenPedido { get; set; }
        public string Observacion { get; set; }
        public string MonedaId { get; set; }
        public bool AfectarStock { get; set; }
        public string DocumentoRelacionadoId { get; set; }
        public List<oGuiaRemisionDetalle> Detalles { get; set; }
        public List<oGuiaRemisionVehiculo> Vehiculos { get; set; }
        public List<oGuiaRemisionTransportista> Transportistas { get; set; }
        public List<oGuiaRemisionDocumentoRelacionado> DocumentosRelacionados { get; set; }
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || !Detalles.Any())
                yield return new ValidationResult("No existen detalles.");
        }
    }
}
