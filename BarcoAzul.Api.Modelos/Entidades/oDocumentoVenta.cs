using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades.Extensiones;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oDocumentoVenta : IValidatableObject
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
        public string ClienteNombre { get; set; }
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
        public List<oDocumentoVentaAnticipo> Anticipos { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore]
        public string IngresoEgresoStock => TipoDocumentoId == "07" && (MotivoNotaId == "01" || MotivoNotaId == "02" || MotivoNotaId == "06" || MotivoNotaId == "07") ? "+" : "-";
        [JsonIgnore]
        public string HoraEmision => DateTime.Now.ToString("HH:mm:ss");
        [JsonIgnore]
        public bool ControlarCilindros => Detalles is not null && Detalles.Any(x => x.TipoDocumentoId == "15");
        [JsonIgnore]
        public decimal NetoMercaderia => Detalles is null ? 0 : Detalles.Where(x => x.CodigoBarras != "ICBPER").Sum(x => x.Importe);
        [JsonIgnore]
        public decimal NetoBolsa => Detalles is null ? 0 : Detalles.Where(x => x.CodigoBarras == "ICBPER").Sum(x => x.Importe);
        [JsonIgnore]
        public decimal CostoTotal => Detalles is null ? 0 : Detalles.Sum(x => x.Costo);
        [JsonIgnore]
        public decimal UtilidadTotal => Detalles is null ? 0 : Detalles.Sum(x => x.Utilidad);
        [JsonIgnore]
        public int NumeroCuotas => Cuotas is null ? 0 : Cuotas.Count;
        [JsonIgnore]
        public oConfiguracionGlobal ConfiguracionGlobal { get; set; }
        [JsonIgnore]
        public bool IsAnulado { get; set; }
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oCliente Cliente { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoDocumento TipoDocumento { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oPersonal Personal { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoVentaCompra TipoVenta { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoCobroPago TipoCobro { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oCuentaCorriente CuentaCorriente { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMotivoNota MotivoNota { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            Observacion = Observacion?.Trim();
            NumeroOperacion = NumeroOperacion?.Trim();
            MotivoSustento = MotivoSustento?.Trim();
            GuiaRemision = GuiaRemision?.Trim();
            NumeroPedido = NumeroPedido?.Trim();
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
                    detalle.AfectarStock = AfectarStock;
                    detalle.IngresoEgresoStock = IngresoEgresoStock;
                    detalle.FechaEmision = FechaEmision;
                    detalle.MonedaId = MonedaId;
                    detalle.PorcentajeIGV = PorcentajeIGV;
                    detalle.MontoICBPER = detalle.CodigoBarras == "ICBPER" ? decimal.Round(detalle.Cantidad * FactorImpuestoBolsa, 2, MidpointRounding.AwayFromZero) : 0;
                }
            }
        }

        public void CompletarDatosCuotas()
        {
            if (TipoVentaId == "CR" && TipoCobroId != "CU")
            {
                Cuotas = new List<oDocumentoVentaCuota>
                {
                    new oDocumentoVentaCuota
                    {
                        CuotaId = 1,
                        Monto = Total,
                        FechaPago = FechaVencimiento,
                    }
                };
            }

            if (Cuotas is not null)
            {
                foreach (var cuota in Cuotas)
                {
                    cuota.EmpresaId = EmpresaId;
                    cuota.TipoDocumentoId = TipoDocumentoId;
                    cuota.Serie = Serie;
                    cuota.Numero = Numero;
                }
            }
        }

        public void CompletarDatosAnticipos()
        {
            if (Anticipos is not null)
            {
                foreach (var anticipo in Anticipos)
                {
                    anticipo.EmpresaId = EmpresaId;
                    anticipo.TipoDocumentoId = TipoDocumentoId;
                    anticipo.Serie = Serie;
                    anticipo.Numero = Numero;
                    anticipo.UsuarioId = UsuarioId;
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var serieLetraInicial = Serie.Left(1);

            if (Detalles is null || !Detalles.Any())
                yield return new ValidationResult("No existen detalles.");

            if (Total == 0 && (!IsOperacionGratuita || (Anticipos is not null && Anticipos.Count != 0)))
            {
                yield return new ValidationResult("El total no puede ser igual a cero (0.00)");
            }

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

                var validacionCuota = ValidarCuotas();

                if (validacionCuota is not null)
                    yield return validacionCuota;
            }
            else if (TipoDocumentoId == "03")
            {
                if (serieLetraInicial != "B")
                {
                    yield return new ValidationResult($"La serie {Serie} no corresponde a una serie electrónica. Formato: BXXX");
                }

                if (ClienteId == ConfiguracionGlobal.DefaultClienteId)
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

        private ValidationResult ValidarCuotas()
        {
            if (TipoVentaId == "CR" && TipoCobroId == "CU")
            {
                if (Cuotas is null || Cuotas.Count == 0)
                    return new ValidationResult("Debe ingresar las cuotas del comprobante cuando el tipo de venta es CREDITO - CUOTAS.");

                var pagosId = Cuotas.Where(x => x.FechaPago < FechaEmision).Select(x => x.Numero);

                if (pagosId.Any())
                    return new ValidationResult($"Existen cuotas con fecha de pago menor a la fecha de emisión del comprobante (Cuotas: {string.Join(", ", pagosId)})");

                var totalMonto = Cuotas.Sum(x => x.Monto);

                if (totalMonto > Total)
                    return new ValidationResult($"El total de los montos de las cuotas debe ser menor o igual al total del comprobante.");
            }

            return null;
        }
    }

    public class oDocumentoVentaDetalle : oDetalle
    {
        [JsonIgnore]
        public string DocumentoVentaId => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
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
        public bool AfectarStock { get; set; }
        [JsonIgnore]
        public string IngresoEgresoStock { get; set; }
        [JsonIgnore]
        public DateTime? FechaEmision { get; set; }
        [JsonIgnore]
        public string MonedaId { get; set; }
        [JsonIgnore]
        public decimal PorcentajeIGV { get; set; }
        [JsonIgnore]
        public decimal MontoICBPER { get; set; }
        [JsonIgnore]
        public decimal Costo => decimal.Round(Cantidad * PrecioCompra, 2, MidpointRounding.AwayFromZero);
        [JsonIgnore]
        public decimal Utilidad => decimal.Round(Importe - Costo, 2, MidpointRounding.AwayFromZero);
        public decimal CantidadPendiente { get; set; }
    }

    public class oDocumentoVentaCuota
    {
        [JsonIgnore]
        public string DocumentoVentaId => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string TipoDocumentoId { get; set; }
        [JsonIgnore]
        public string Serie { get; set; }
        [JsonIgnore]
        public string Numero { get; set; }
        public int CuotaId { get; set; }
        public DateTime FechaPago { get; set; }
        public decimal Monto { get; set; }
    }

    public class oDocumentoVentaAnticipo
    {
        [JsonIgnore]
        public string DocumentoVentaId => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string TipoDocumentoId { get; set; }
        [JsonIgnore]
        public string Serie { get; set; }
        [JsonIgnore]
        public string Numero { get; set; }
        public int AnticipoId { get; set; }
        public string DocumentoRelacionadoId { get; set; }
        public DateTime FechaEmision { get; set; }
        public string MonedaId { get; set; }
        public decimal TipoCambio { get; set; }
        public decimal SubTotal { get; set; }
        [JsonIgnore]
        public string UsuarioId { get; set; }
    }
}
