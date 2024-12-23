using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oSalidaAlmacen : IValidatableObject
    {
        public string Id => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        public string EmpresaId { get; set; }
        public string TipoDocumentoId { get; set; }
        [Required(ErrorMessage = "La serie es requerida.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "La serie debe tener 4 caracteres.")]
        public string Serie { get; set; }
        public string Numero { get; set; }
        [Required(ErrorMessage = "La fecha de inicio es requerida.")]
        public DateTime FechaInicio { get; set; }
        [Required(ErrorMessage = "La fecha de terminación es requerida.")]
        public DateTime FechaTerminacion { get; set; }
        [Required(ErrorMessage = "El cliente es requerido.")]
        public string ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteNumeroDocumentoIdentidad { get; set; }
        public string MonedaId { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00)")]
        public decimal TipoCambio { get; set; }
        public string PersonalId { get; set; }
        public string LineaProduccion { get; set; }
        public string Envasado { get; set; }
        public string NumeroLote { get; set; }
        public string GuiaRemision { get; set; }
        public string Observacion { get; set; }
        public bool IncluyeIGV { get; set; }
        public decimal PorcentajeIGV { get; set; }
        public decimal GastosIndirectos { get; set; }
        public string CantidadSolicitada { get; set; }
        public string CantidadProducida { get; set; }
        [Required(ErrorMessage = "El total es requerido.")]
        public decimal Total { get; set; }
        public decimal TotalGalones { get; set; }
        public decimal CostoGalon { get; set; }
        public decimal CostoGalonMasGastoIndirectos { get; set; }
        public decimal CostoGalonMasIGV { get; set; }
        public List<oSalidaAlmacenDetalle> Detalles { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore]
        public string HoraEmision => DateTime.Now.ToString("HH:mm:ss");
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oCliente Cliente { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oPersonal Personal { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            LineaProduccion = LineaProduccion?.Trim();
            Envasado = Envasado?.Trim();
            NumeroLote = NumeroLote?.Trim();
            GuiaRemision = GuiaRemision?.Trim();
            Observacion = Observacion?.Trim();
        }

        public void CompletarDatosDetalles()
        {
            if (Detalles is not null)
            {
                foreach (var detalle in Detalles)
                {
                    detalle.EmpresaId = EmpresaId;
                    detalle.TipoDocumentoId = TipoDocumentoId;
                    detalle.Serie = Serie;
                    detalle.Numero = Numero;
                    detalle.FechaEmision = FechaTerminacion;
                    detalle.MonedaId = MonedaId;
                    detalle.PorcentajeIGV = PorcentajeIGV;
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || !Detalles.Any())
                yield return new ValidationResult("No existen detalles.");
        }
    }

    public class oSalidaAlmacenDetalle : oDetalle
    {
        [JsonIgnore]
        public string SalidaAlmacenId => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string TipoDocumentoId { get; set; }
        [JsonIgnore]
        public string Serie { get; set; }
        [JsonIgnore]
        public string Numero { get; set; }
        [JsonIgnore]
        public DateTime? FechaEmision { get; set; }
        [JsonIgnore]
        public string MonedaId { get; set; }
        [JsonIgnore]
        public decimal PorcentajeIGV { get; set; }
    }
}
