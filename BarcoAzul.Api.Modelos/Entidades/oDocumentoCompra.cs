using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oDocumentoCompra : IValidatableObject
    {
        public string Id => $"{EmpresaId}{ProveedorId}{TipoDocumentoId}{Serie}{Numero}{ClienteId}";
        public string EmpresaId { get; set; }
        [Required(ErrorMessage = "El proveedor es requerido.")]
        public string ProveedorId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public string ClienteId { get; set; }
        [Required(ErrorMessage = "La fecha de emisión es requerida.")]
        public DateTime FechaEmision { get; set; }
        [Required(ErrorMessage = "La fecha contable es requerida.")]
        public DateTime FechaContable { get; set; }
        [Required(ErrorMessage = "La fecha de vencimiento es requerida.")]
        public DateTime FechaVencimiento { get; set; }
        public string ProveedorNombre { get; set; }
        public string ProveedorNumeroDocumentoIdentidad { get; set; }
        public string ProveedorDireccion { get; set; }
        public string TipoCompraId { get; set; }
        public string MonedaId { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00)")]
        public decimal TipoCambio { get; set; }
        public string TipoPagoId { get; set; }
        public string NumeroOperacion { get; set; }
        public string CuentaCorrienteId { get; set; }
        public string DocumentoReferenciaId { get; set; }
        public bool Abonar { get; set; }
        public string MotivoNotaId { get; set; }
        public string MotivoSustento { get; set; }
        public string GuiaRemision { get; set; }
        public string Observacion { get; set; }
        public decimal SubTotal { get; set; }
        public decimal PorcentajeIGV { get; set; }
        public decimal MontoIGV { get; set; }
        public decimal TotalNeto { get; set; }
        [Required(ErrorMessage = "El total es requerido.")]
        [Range(0.01, int.MaxValue, ErrorMessage = "El total no puede ser igual a cero (0.00)")]
        public decimal Total { get; set; }
        public bool IncluyeIGV { get; set; }
        public bool AfectarStock { get; set; }
        public bool AfectarPrecio { get; set; }
        public List<oDocumentoCompraDetalle> Detalles { get; set; }
        public List<oDocumentoCompraOrdenCompraRelacionada> OrdenesCompraRelacionadas { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore]
        public string PersonalId { get; set; }
        [JsonIgnore]
        public string HoraEmision => DateTime.Now.ToString("HH:mm:ss");
        public string NumeroDocumento => Comun.CompraIdADocumento(Id);
        public string NumeroOrdenesCompraRelacionadas
        {
            get
            {
                if (OrdenesCompraRelacionadas is null)
                    return string.Empty;

                return string.Join(", ", OrdenesCompraRelacionadas.Select(x => x.NumeroDocumento));
            }
        }
        #endregion

        #region Referencias 
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoDocumento TipoDocumento { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oProveedor Proveedor { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoVentaCompra TipoCompra { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoCobroPago TipoPago { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oCuentaCorriente CuentaCorriente { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMotivoNota MotivoNota { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            ProveedorDireccion = ProveedorDireccion?.Trim();
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
                    detalle.ProveedorId = ProveedorId;
                    detalle.TipoDocumentoId = TipoDocumentoId;
                    detalle.Serie = Serie;
                    detalle.Numero = Numero;
                    detalle.ClienteId = ClienteId;
                    detalle.AfectarStock = AfectarStock;
                    detalle.FechaEmision = FechaEmision;
                    detalle.MonedaId = MonedaId;
                    detalle.PrecioNeto = IncluyeIGV ? decimal.Round((detalle.PrecioUnitario * 100) / (PorcentajeIGV + 100) * detalle.Cantidad, 4, MidpointRounding.AwayFromZero)
                                                    : decimal.Round(detalle.PrecioUnitario * detalle.Cantidad, 4, MidpointRounding.AwayFromZero);
                }
            }
        }

        public void CompletarDatosOrdenesCompraRelacionadas()
        {
            if (OrdenesCompraRelacionadas is not null)
            {
                foreach (var ordenCompraRelacionada in OrdenesCompraRelacionadas)
                {
                    ordenCompraRelacionada.EmpresaId = EmpresaId;
                    ordenCompraRelacionada.ProveedorId = ProveedorId;
                    ordenCompraRelacionada.TipoDocumentoId = TipoDocumentoId;
                    ordenCompraRelacionada.Serie = Serie;
                    ordenCompraRelacionada.Numero = Numero;
                    ordenCompraRelacionada.ClienteId = ClienteId;
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || Detalles.Count == 0)
                yield return new ValidationResult("No existen detalles.");

            if (TipoCompraId == "CO" && (TipoPagoId == "DE" || TipoPagoId == "CH"))
            {
                if (string.IsNullOrEmpty(CuentaCorrienteId))
                    yield return new ValidationResult("La cuenta corriente es requerida.");

                if (string.IsNullOrWhiteSpace(NumeroOperacion))
                    yield return new ValidationResult("El número de operación es requerido.");
            }

            if (TipoDocumentoId == "07" || TipoDocumentoId == "08")
            {
                if (string.IsNullOrWhiteSpace(MotivoNotaId))
                    yield return new ValidationResult("El motivo de la nota es requerido.");
            }
        }
    }

    public class oDocumentoCompraDetalle : oDetalle
    {
        [JsonIgnore]
        public string DocumentoCompraId => $"{EmpresaId}{ProveedorId}{TipoDocumentoId}{Serie}{Numero}{ClienteId}";
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
        public bool AfectarStock { get; set; }
        [JsonIgnore]
        public DateTime? FechaEmision { get; set; }
        [JsonIgnore]
        public string MonedaId { get; set; }
        [JsonIgnore]
        public decimal PrecioNeto { get; set; }
        public decimal CantidadPendiente { get; set; }
    }

    public class oDocumentoCompraOrdenCompraRelacionada
    {
        [JsonIgnore]
        public string DocumentoCompraId => $"{EmpresaId}{ProveedorId}{TipoDocumentoId}{Serie}{Numero}{ClienteId}";
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
        public string Id { get; set; }
        public string NumeroDocumento { get; set; }
        [JsonIgnore]
        public DateTime Fecha { get; set; }
    }
}
