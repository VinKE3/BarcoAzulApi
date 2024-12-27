using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oGuiaRemision : IValidatableObject
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
        public List<oGuiaRemisionDocumentoRelacionado> DocumentosRelacionados { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore]
        public decimal TipoCambio { get; set; }
        [JsonIgnore]
        public decimal PorcentajeIGV { get; set; }
        [JsonIgnore]
        public decimal CostoTotal => Detalles is null ? 0 : Detalles.Sum(x => x.Costo);
        [JsonIgnore]
        public string HoraEmision => DateTime.Now.ToString("HH:mm:ss");
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oCliente Cliente { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oPersonal Personal { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oEmpresaTransporte EmpresaTransporte { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oConductor Conductor { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oVehiculo Vehiculo { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMotivoTraslado MotivoTraslado { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            DireccionPartida = DireccionPartida?.Trim();
            LicenciaConducir = LicenciaConducir?.Trim();
            ConstanciaInscripcion = ConstanciaInscripcion?.Trim();
            MotivoSustento = MotivoSustento?.Trim();
            NumeroFactura = NumeroFactura?.Trim();
            OrdenPedido = OrdenPedido?.Trim();
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
                    detalle.AfectarStock = AfectarStock;
                    detalle.MonedaId = MonedaId;
                    detalle.PorcentajeIGV = PorcentajeIGV;
                    detalle.IngresoEgresoStock = IngresoEgresoStock;
                }
            }
        }

        public void CompletarDatosDocumentosRelacionados()
        {
            if (DocumentosRelacionados is not null)
            {
                foreach (var documentoRelacionado in DocumentosRelacionados)
                {
                    documentoRelacionado.EmpresaId = EmpresaId;
                    documentoRelacionado.TipoDocumentoId = TipoDocumentoId;
                    documentoRelacionado.Serie = Serie;
                    documentoRelacionado.Numero = Numero;
                    documentoRelacionado.FechaEmision = FechaEmision;
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || !Detalles.Any())
                yield return new ValidationResult("No existen detalles.");
        }
    }

    public class oGuiaRemisionDetalle : oDetalle
    {
        [JsonIgnore]
        public string GuiaRemisionId => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
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
        public DateTime FechaEmision { get; set; }
        [JsonIgnore]
        public bool AfectarStock { get; set; }
        [JsonIgnore]
        public string MonedaId { get; set; }
        [JsonIgnore]
        public decimal PorcentajeIGV { get; set; }
        [JsonIgnore]
        public string IngresoEgresoStock { get; set; }
        [JsonIgnore]
        public decimal Costo => decimal.Round(Cantidad * PrecioCompra, 2, MidpointRounding.AwayFromZero);
        public decimal? CantidadPendiente { get; set; }
    }

    public class oGuiaRemisionDocumentoRelacionado
    {
        [JsonIgnore]
        public string GuiaRemisionId => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string TipoDocumentoId { get; set; }
        [JsonIgnore]
        public string Serie { get; set; }
        [JsonIgnore]
        public string Numero { get; set; }
        public string Id { get; set; }
        public string NumeroDocumento { get; set; }
        [JsonIgnore]
        public DateTime FechaEmision { get; set; }
    }
}
