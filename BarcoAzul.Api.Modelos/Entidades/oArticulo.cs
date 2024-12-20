using BarcoAzul.Api.Modelos.Otros;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oArticulo
    {
        public string Id => $"{LineaId}{SubLineaId}{ArticuloId}";
        [Required(ErrorMessage = "La línea es requerida.")]
        public string LineaId { get; set; }
        [Required(ErrorMessage = "La subLínea es requerida.")]
        public string SubLineaId { get; set; }
        public string ArticuloId { get; set; }
        public string TipoExistenciaId { get; set; }
        [Required(ErrorMessage = "La unidad de medida es requerida.")]
        public string UnidadMedidaId { get; set; }
        [Required(ErrorMessage = "La marca es requerida.")]
        public int MarcaId { get; set; }
        [Required(ErrorMessage = "La descripción es requerida.")]
        public string Descripcion { get; set; }
        public string Observacion { get; set; }
        public string CodigoBarras { get; set; }
        public decimal Peso { get; set; }
        public string MonedaId { get; set; }
        public string Exportado { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioCompraDescuento { get; set; }
        public decimal PrecioVenta1 { get; set; }
        public decimal PrecioVenta2 { get; set; }
        public decimal PrecioVenta3 { get; set; }
        public decimal PrecioVenta4 { get; set; }
        public decimal PorcentajeUtilidad1 { get; set; }
        public decimal PorcentajeUtilidad2 { get; set; }
        public decimal PorcentajeUtilidad3 { get; set; }
        public decimal PorcentajeUtilidad4 { get; set; }
        public decimal Stock { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal StockMax { get; set; }
        public bool PrecioIncluyeIGV { get; set; }
        public bool PercepcionCompra { get; set; }
        public bool IsActivo { get; set; }
        public bool ControlarStock { get; set; }
        public bool ActualizarPrecioCompra { get; set; }
        public bool Detraccion { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string UnidadMedidaDescripcion { get; set; }
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oLinea Linea { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oSubLinea SubLinea { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoExistencia TipoExistencia { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oUnidadMedida UnidadMedida { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMarca Marca { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            Descripcion = Descripcion?.Trim();
            CodigoBarras = CodigoBarras?.Trim();
            Observacion = Observacion?.Trim();
        }
    }
}
