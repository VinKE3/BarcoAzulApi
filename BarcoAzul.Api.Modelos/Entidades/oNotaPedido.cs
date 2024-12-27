using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oNotaPedido : IValidatableObject
    {
        public string Id => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        public string EmpresaId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        [Required(ErrorMessage = "La fecha de emisión es requerida.")]
        public DateTime FechaEmision { get; set; }
        public DateTime FechaVencimiento { get; set; }
        [Required(ErrorMessage = "El cliente es requerido.")]
        public string ClienteId { get; set; }
        public string ClienteNombre { get; set; }
        public string ClienteNumeroDocumentoIdentidad { get; set; }
        public int ClienteDireccionId { get; set; }
        public string ClienteDireccion { get; set; }
        public string ClienteTelefono { get; set; }
        public string DepartamentoId { get; set; }
        public string ProvinciaId { get; set; }
        public string DistritoId { get; set; }
        public string ContactoId { get; set; }
        public string ContactoNombre { get; set; }
        public string ContactoTelefono { get; set; }
        public string ContactoCorreoElectronico { get; set; }
        public int? ContactoCargoId { get; set; }
        public string ContactoCargoDescripcion { get; set; }
        public string ContactoCelular { get; set; }
        public string PersonalId { get; set; }
        public string MonedaId { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00)")]
        public decimal TipoCambio { get; set; }
        public string TipoVentaId { get; set; }
        public string TipoCobroId { get; set; }
        public string NumeroOperacion { get; set; }
        public string CuentaCorrienteDescripcion { get; set; }
        public string Validez { get; set; }
        public string Observacion { get; set; }
        public decimal SubTotal { get; set; }
        public decimal MontoIGV { get; set; }
        public decimal TotalNeto { get; set; }
        public decimal MontoRetencion { get; set; }
        public decimal MontoPercepcion { get; set; }
        [Required(ErrorMessage = "El total es requerido.")]
        [Range(0.01, int.MaxValue, ErrorMessage = "El total no puede ser igual a cero (0.00)")]
        public decimal Total { get; set; }
        public decimal PorcentajeIGV { get; set; }
        public decimal PorcentajeRetencion { get; set; }
        public decimal PorcentajePercepcion { get; set; }
        public bool IncluyeIGV { get; set; }
        public List<oNotaPedidoDetalle> Detalles { get; set; }

        #region Adicionales
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string ClienteTipoDocumentoIdentidadId { get; set; }
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore]
        public string HoraEmision => DateTime.Now.ToString("HH:mm:ss");
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);
        [JsonIgnore]
        public decimal CostoTotal => Detalles is null ? 0 : Detalles.Sum(x => x.Costo);
        [JsonIgnore]
        public decimal UtilidadTotal => Detalles is null ? 0 : Detalles.Sum(x => x.Utilidad);
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oCliente Cliente { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oPersonal Personal { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoVentaCompra TipoVenta { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoCobroPago TipoCobro { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            Validez = Validez?.Trim();
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
                    detalle.FechaEmision = FechaEmision;
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

    public class oNotaPedidoDetalle : oDetalle
    {
        [JsonIgnore]
        public string NotaPedidoId => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string TipoDocumentoId { get; set; }
        [JsonIgnore]
        public string Serie { get; set; }
        [JsonIgnore]
        public string Numero { get; set; }
        public decimal PrecioCompra { get; set; }
        [JsonIgnore]
        public DateTime? FechaEmision { get; set; }
        [JsonIgnore]
        public string MonedaId { get; set; }
        [JsonIgnore]
        public decimal PorcentajeIGV { get; set; }
        [JsonIgnore]
        public decimal Costo => decimal.Round(Cantidad * PrecioCompra, 2, MidpointRounding.AwayFromZero);
        [JsonIgnore]
        public decimal Utilidad => decimal.Round(Importe - Costo, 2, MidpointRounding.AwayFromZero);
    }
}
