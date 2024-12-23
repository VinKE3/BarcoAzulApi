using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oEntradaAlmacen : IValidatableObject
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
        public string PersonalId { get; set; }
        public DateTime FechaEmision { get; set; }
        public string MonedaId { get; set; }
        public decimal TipoCambio { get; set; }
        public string NumeroOP { get; set; }
        public string Observacion { get; set; }
        public List<oEntradaAlmacenDetalle> Detalles { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore]
        public string HoraEmision => DateTime.Now.ToString("HH:mm:ss");
        public string NumeroDocumento => Comun.CompraIdADocumento(Id);
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oProveedor Proveedor { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oPersonal Personal { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            NumeroOP = NumeroOP?.Trim();
            Observacion = Observacion?.Trim();
        }

        public void CompletarDatosDetalles()
        {
            if (Detalles is not null)
            {
                foreach (var detalle in Detalles)
                {
                    detalle.EmpresaId = EmpresaId;
                    detalle.ProveedorId = ProveedorId;
                    detalle.TipoDocumentoId = TipoDocumentoId;
                    detalle.Serie = Serie;
                    detalle.Numero = Numero;
                    detalle.ClienteId = ClienteId;
                    detalle.FechaEmision = FechaEmision;
                    detalle.MonedaId = MonedaId;
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || Detalles.Count == 0)
                yield return new ValidationResult("No existen detalles.");
        }
    }

    public class oEntradaAlmacenDetalle : oDetalle
    {
        [JsonIgnore]
        public string EntradaAlmacenId => $"{EmpresaId}{ProveedorId}{TipoDocumentoId}{Serie}{Numero}{ClienteId}";
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string ProveedorId { get; set; }
        [JsonIgnore]
        public string TipoDocumentoId { get; set; }
        [JsonIgnore]
        public string Serie { get; set; }
        [JsonIgnore]
        public string Numero { get; set; }
        [JsonIgnore]
        public string ClienteId { get; set; }
        [JsonIgnore]
        public DateTime? FechaEmision { get; set; }
        [JsonIgnore]
        public string MonedaId { get; set; }
        public decimal PrecioVenta { get; set; }
    }
}
