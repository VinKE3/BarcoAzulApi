using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oGuiaCompra : IValidatableObject
    {
        public string Id => $"{EmpresaId}{ProveedorId}{TipoDocumentoId}{Serie}{Numero}{ClienteId}";
        public string EmpresaId { get; set; }
        [Required(ErrorMessage = "El proveedor es requerido.")]
        public string ProveedorId { get; set; }
        public string TipoDocumentoId { get; set; }
        [Required(ErrorMessage = "La serie es requerida.")]
        public string Serie { get; set; }
        [Required(ErrorMessage = "El número es requerido.")]
        public string Numero { get; set; }
        public string ClienteId { get; set; }
        [Required(ErrorMessage = "La fecha de emisión es requerida.")]
        public DateTime FechaEmision { get; set; }
        public string DireccionPartida { get; set; }
        public string DepartamentoPartidaId { get; set; }
        public string ProvinciaPartidaId { get; set; }
        public string DistritoPartidaId { get; set; }
        public string DepartamentoPartidaNombre { get; set; }
        public string ProvinciaPartidaNombre { get; set; }
        public string DistritoPartidaNombre { get; set; }
        public string DireccionLlegada { get; set; }
        public string DepartamentoLlegadaId { get; set; }
        public string ProvinciaLlegadaId { get; set; }
        public string DistritoLlegadaId { get; set; }
        public string DepartamentoLlegadaNombre { get; set; }
        public string ProvinciaLlegadaNombre { get; set; }
        public string DistritoLlegadaNombre { get; set; }
        public string ProveedorRUC { get; set; }
        public string ProveedorNombre { get; set; }
        public string ProveedorNumeroDocumentoIdentidad { get; set; }
        public string TransportistaId { get; set; }
        public string TransportistaNumeroDocumentoIdentidad { get; set; }
        public string TransportistaCertificadoInscripcion { get; set; }
        public string TransportistaLicenciaConducir { get; set; }
        public string MarcaPlaca { get; set; }
        public string MotivoTrasladoId { get; set; }
        public string MotivoTrasladoSustento { get; set; }
        public string IngresoEgresoStock { get; set; }
        public string Observacion { get; set; }
        public string DocumentoReferenciaId { get; set; }
        public string DocumentoReferencia { get; set; }
        public string MonedaId { get; set; }
        public bool AfectarStock { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00)")]
        public decimal TipoCambio { get; set; }
        public List<oGuiaCompraDetalle> Detalles { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string PersonalId { get; set; }
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore]
        public decimal PorcentajeIGV { get; set; }
        [JsonIgnore]
        public string HoraEmision => DateTime.Now.ToString("HH:mm:ss");
        public string NumeroDocumento => Comun.CompraIdADocumento(Id);
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oProveedor Proveedor { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMotivoTraslado MotivoTraslado { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            DireccionPartida = DireccionPartida?.Trim();
            DireccionLlegada = DireccionLlegada?.Trim();
            ProveedorRUC = ProveedorRUC?.Trim();
            ProveedorNumeroDocumentoIdentidad = ProveedorNumeroDocumentoIdentidad?.Trim();
            TransportistaNumeroDocumentoIdentidad = TransportistaNumeroDocumentoIdentidad?.Trim();
            MarcaPlaca = MarcaPlaca?.Trim();
            TransportistaCertificadoInscripcion = TransportistaCertificadoInscripcion?.Trim();
            TransportistaLicenciaConducir = TransportistaLicenciaConducir?.Trim();
            MotivoTrasladoSustento = MotivoTrasladoSustento?.Trim();
            Observacion = Observacion?.Trim();
            DocumentoReferencia = DocumentoReferencia?.Trim();
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
                    detalle.AfectarStock = AfectarStock;
                    detalle.IngresoEgresoStock = IngresoEgresoStock;
                    detalle.PorcentajeIGV = PorcentajeIGV;
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || Detalles.Count == 0)
                yield return new ValidationResult("No existen detalles.");
        }
    }

    public class oGuiaCompraDetalle : oDetalle
    {
        [JsonIgnore]
        public string GuiaCompraId => $"{EmpresaId}{ProveedorId}{TipoDocumentoId}{Serie}{Numero}{ClienteId}";
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
        [JsonIgnore]
        public bool AfectarStock { get; set; }
        [JsonIgnore]
        public string IngresoEgresoStock { get; set; }
        [JsonIgnore]
        public decimal PorcentajeIGV { get; set; }
        public decimal CantidadPendiente { get; set; }
    }
}
