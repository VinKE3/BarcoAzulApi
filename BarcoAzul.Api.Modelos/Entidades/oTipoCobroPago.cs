using BarcoAzul.Api.Modelos.Otros;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oTipoCobroPago
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "Seleccione un tipo de venta - compra.")]
        public string TipoVentaCompraId { get; set; }
        [Required(ErrorMessage = "La descripción es requerida.")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "La abreviatura es requerida.")]
        public string Abreviatura { get; set; }
        public int Plazo { get; set; }

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoVentaCompra TipoVentaCompra { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            Descripcion = Descripcion.Trim();
            Abreviatura = Abreviatura.Trim();
        }
    }
}
