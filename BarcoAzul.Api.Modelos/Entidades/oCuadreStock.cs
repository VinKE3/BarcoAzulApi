using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oCuadreStock : IValidatableObject
    {
        public string Id => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        public string EmpresaId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        [Required(ErrorMessage = "La fecha es requerida.")]
        public DateTime FechaRegistro { get; set; }
        public string HoraRegistro => DateTime.Now.ToString("HH:mm:ss");
        public string MonedaId { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00).")]
        public decimal TipoCambio { get; set; }
        [Required(ErrorMessage = "El responsable es requerido.")]
        public string ResponsableId { get; set; }
        public string Observacion { get; set; }
        public decimal TotalSobra { get; set; }
        public decimal TotalFalta { get; set; }
        public decimal SaldoTotal { get; set; }
        public List<oCuadreStockDetalle> Detalles { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string ClienteId { get; set; }
        [JsonIgnore]
        public string UsuarioId { get; set; }
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oPersonal Responsable { get; set; }
        #endregion

        public void ProcesarDatos()
        {
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
                }
            }
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Detalles is null || Detalles.Count == 0)
                yield return new ValidationResult("No existen detalles.");
        }
    }

    public class oCuadreStockDetalle
    {
        [JsonIgnore]
        public string CuadreStockId => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string TipoDocumentoId { get; set; }
        [JsonIgnore]
        public string Serie { get; set; }
        [JsonIgnore]
        public string Numero { get; set; }
        public int DetalleId { get; set; }
        public string LineaId { get; set; }
        public string SubLineaId { get; set; }
        public string ArticuloId { get; set; }
        public int? MarcaId { get; set; }
        public string UnidadMedidaId { get; set; }
        public string Descripcion { get; set; }
        public string CodigoBarras { get; set; }
        public string UnidadMedidaDescripcion { get; set; }
        public decimal StockFinal { get; set; }
        public decimal PrecioUnitario { get; set; }
        public decimal Inventario { get; set; }
        public decimal CantidadFalta { get; set; }
        public decimal TotalFalta { get; set; }
        public decimal CantidadSobra { get; set; }
        public decimal TotalSobra { get; set; }
        public decimal CantidadSaldo { get; set; }
        public decimal TotalSaldo { get; set; }
        public string MarcaNombre { get; set; }
        public string LineaDescripcion { get; set; }
        public string SubLineaDescripcion { get; set; }
        public string TipoExistenciaId { get; set; }
    }
}
