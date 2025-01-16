using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades.Extensiones;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.DTOs
{
    public class DocumentoVentaDTO : IValidatableObject
    {
        public string Id => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        public string EmpresaId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        [Required(ErrorMessage = "La fecha de emisión es requerida.")]
        public DateTime FechaEmision { get; set; }
        [Required(ErrorMessage = "La fecha de vencimiento es requerida.")]
        public DateTime FechaVencimiento { get; set; }
        public string Cotizacion { get; set; }
        public string CotizacionId { get; set; }
        [Required(ErrorMessage = "El cliente es requerido.")]
        public string ClienteId { get; set; }
        public string ClienteTipoDocumentoIdentidadId { get; set; }
        public string ClienteNumeroDocumentoIdentidad { get; set; }
        public int ClienteDireccionId { get; set; }
        public string ClienteDireccion { get; set; }
        public string PersonalId { get; set; }
        public string Letra { get; set; }
        public string LetraId { get; set; }
        public string MonedaId { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00)")]
        public decimal TipoCambio { get; set; }
        public string TipoVentaId { get; set; }
        public string TipoCobroId { get; set; }
        public string NumeroOperacion { get; set; }
        public string CuentaCorrienteId { get; set; }
        public string DocumentoReferenciaId { get; set; }
        public DateTime? FechaDocumentoReferencia { get; set; }
        public bool Abonar { get; set; }
        public string MotivoNotaId { get; set; }
        public string MotivoNotaDescripcion { get; set; }
        public string MotivoSustento { get; set; }
        public string GuiaRemision { get; set; }
        public string NumeroPedido { get; set; }
        public string Orden { get; set; }
        public string Observacion { get; set; }
        public bool IsAnticipo { get; set; }
        public bool IsOperacionGratuita { get; set; }
        public bool IncluyeIGV { get; set; }
        public bool AfectarStock { get; set; }
        public decimal TotalOperacionesInafectas { get; set; }
        public decimal TotalOperacionesGratuitas { get; set; }
        public decimal SubTotal { get; set; }
        public decimal TotalAnticipos { get; set; }
        public decimal TotalNeto { get; set; }
        public decimal MontoIGV { get; set; }
        public decimal MontoRetencion { get; set; }
        public decimal MontoDetraccion { get; set; }
        public decimal MontoImpuestoBolsa { get; set; }
        [Required(ErrorMessage = "El total es requerido.")]
        public decimal Total { get; set; }
        public decimal PorcentajeIGV { get; set; }
        public decimal PorcentajeRetencion { get; set; }
        public decimal PorcentajeDetraccion { get; set; }
        public decimal FactorImpuestoBolsa { get; set; }
        public List<oDocumentoVentaDetalle> Detalles { get; set; }
        public List<oDocumentoVentaCuota> Cuotas { get; set; }
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);

        #region Adicionales
        [JsonIgnore]
        public oConfiguracionGlobal ConfiguracionGlobal { get; set; }
        [JsonIgnore]
        public string IngresoEgresoStock => TipoDocumentoId == "07" && (MotivoNotaId == "01" || MotivoNotaId == "02" || MotivoNotaId == "06" || MotivoNotaId == "07") ? "+" : "-";
        #endregion

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var serieLetraInicial = Serie.Left(1);

            if (Detalles is null || !Detalles.Any())
                yield return new ValidationResult("No existen detalles.");

            if (!string.IsNullOrWhiteSpace(GuiaRemision) && GuiaRemision.Mid(4, 1) != "-")
            {
                yield return new ValidationResult("Formato de N° de guía incorrecto. Formato SUNAT (####-########), ejm.: 0001-00000001");
            }

            if (TipoVentaId == "CO" && (TipoCobroId == "DE" || TipoCobroId == "CH"))
            {
                if (string.IsNullOrEmpty(NumeroOperacion) || string.IsNullOrEmpty(CuentaCorrienteId))
                {
                    yield return new ValidationResult("No se registro el número de operación y/o la cuenta corriente.");
                }
            }
            else if (TipoVentaId == "CR" && TipoCobroId == "CU")
            {
                if (Cuotas is null || !Cuotas.Any())
                    yield return new ValidationResult("No existen cuotas.");
                else
                {
                    if (TipoDocumentoId != "07")
                    {
                        if (Cuotas.Sum(x => x.Monto) != Total)
                            yield return new ValidationResult("La sumatorio de las cuotas no coincide con el total del documento.");

                        if (Cuotas.Any(x => x.FechaPago <= FechaEmision))
                            yield return new ValidationResult("La fecha de la cuota no puede ser menor o igual a la fecha de emisión del comprobante.");
                    }
                }
            }

            if (TipoDocumentoId == "01")
            {
                if (serieLetraInicial != "F")
                {
                    yield return new ValidationResult($"La serie {Serie} no corresponde a una serie electrónica. Formato: FXXX");
                }

                if (ClienteTipoDocumentoIdentidadId != "6")
                {
                    yield return new ValidationResult("El tipo de documento de identidad del cliente debe ser RUC.");
                }
                else if (ClienteNumeroDocumentoIdentidad.Length != 11)
                {
                    yield return new ValidationResult("El número de documento de identidad debe contener 11 dígitos.");
                }
                else if (!Validacion.ValidarRuc(ClienteNumeroDocumentoIdentidad))
                {
                    yield return new ValidationResult("El número de documento de identidad no es válido.");
                }
            }
            else if (TipoDocumentoId == "03")
            {
                if (serieLetraInicial != "B")
                {
                    yield return new ValidationResult($"La serie {Serie} no corresponde a una serie electrónica. Formato: BXXX");
                }

                //TODO: Pasar en configuración global el objeto
                if (ClienteId == ConfiguracionGlobal?.DefaultClienteId)
                {
                    if ((MonedaId == "S" && Total >= 700) || (MonedaId == "D" && Total * TipoCambio >= 700))
                    {
                        yield return new ValidationResult("Las boletas con un total mayor o igual a S/700.00 deben llevar un DNI o documento de identidad válido.");
                    }
                }
                else if (ClienteTipoDocumentoIdentidadId == "1" && (ClienteNumeroDocumentoIdentidad.Length != 8 || !Validacion.IsInteger(ClienteNumeroDocumentoIdentidad)))
                {
                    yield return new ValidationResult("El DNI no es válido.");
                }
                else if ((ClienteTipoDocumentoIdentidadId == "4" || ClienteTipoDocumentoIdentidadId == "7") && ClienteNumeroDocumentoIdentidad.Length > 12)
                {
                    yield return new ValidationResult("El número de documento de identidad del cliente no debe ser mayor a 12 dígitos si es carnet de extranjería o número de pasaporte.");
                }
            }
            else if (TipoDocumentoId == "07" || TipoDocumentoId == "08")
            {
                if (serieLetraInicial != "F" && serieLetraInicial != "B")
                {
                    yield return new ValidationResult($"La serie {Serie} no corresponde a una serie electrónica. Formato: FXXX o BXXX");
                }

                if (string.IsNullOrEmpty(DocumentoReferenciaId))
                {
                    yield return new ValidationResult("El documento de referencia es requerido.");
                }
                else if (DocumentoReferenciaId.Mid(4, 1) != serieLetraInicial)
                {
                    yield return new ValidationResult("La serie del documento de referencia no coincide con la serie de la nota de crédito/débito.");
                }

                if (string.IsNullOrEmpty(MotivoNotaId) || string.IsNullOrWhiteSpace(MotivoSustento))
                {
                    yield return new ValidationResult("El motivo y/o sustento es requerido.");
                }

                if (TipoDocumentoId == "07" && serieLetraInicial == "B" && (MotivoNotaId == "04" || MotivoNotaId == "05" || MotivoNotaId == "08"))
                {
                    yield return new ValidationResult("No se puede emitir notas de créditos para boletas con los motivos: 04 - DESCUENTO GLOBAL, 05 - DESCUENTO POR ITEM, 08 - BONIFICACION.");
                }
            }
        }
    }
}
