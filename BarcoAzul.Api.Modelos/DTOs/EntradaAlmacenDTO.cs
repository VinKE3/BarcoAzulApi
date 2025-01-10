using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class EntradaAlmacenDTO : IValidatableObject
    {
        public string Id => $"{EmpresaId}{ProveedorId}{TipoDocumentoId}{Serie}{Numero}{ClienteId}";
        public string EmpresaId { get; set; }
        [Required(ErrorMessage = "El proveedor es requerido.")]
        public string ProveedorId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public string ClienteId { get; set; }
        public string ProveedorNumeroDocumentoIdentidad { get; set; }
        public string ProveedorNombre { get; set; }
        public string ProveedorDireccion { get; set; }
        [Required(ErrorMessage = "El Personal es requerido.")]
        public string PersonalId { get; set; }
        public DateTime FechaEmision { get; set; }
        public string MonedaId { get; set; }
        public decimal TipoCambio { get; set; }
        public string MotivoId { get; set; }
        public string NumeroOP { get; set; }
        public decimal Total {  get; set; }
        public string Observacion { get; set; }
        public List<oEntradaAlmacenDetalle> Detalles { get; set; }
        public string NumeroDocumento => Comun.CompraIdADocumento(Id);

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || Detalles.Count == 0)
                yield return new ValidationResult("No existen detalles.");
        }
    }
}
