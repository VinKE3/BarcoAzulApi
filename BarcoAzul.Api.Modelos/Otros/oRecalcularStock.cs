using System.Text.Json.Serialization;


namespace BarcoAzul.Api.Modelos.Otros
{
    public class oRecalcularStock
    {
        [JsonIgnore]
        public string EmpresaId { get; set; }
        public DateTime Fecha { get; set; }
        public List<oRecalcularStockArticulo> Articulos { get; set; }
    }

    public class oRecalcularStockArticulo
    {
        public string LineaId { get; set; }
        public string SubLineaId { get; set; }
        public string ArticuloId { get; set; }
        public decimal Stock { get; set; }
    }
}
