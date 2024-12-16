using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Otros.Informes.Parametros
{
    public class oParamMovimientoArticulo
    {
        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string AgrupadoPor { get; set; }
        public bool ConStock { get; set; } //Se muestra ConMovimiento en el local
        public string Precio { get; set; }
        public string EstadoStock { get; set; }

        public string TipoExistenciaId { get; set; }
        [JsonIgnore]
        public string TipoExistenciaDescripcion { get; set; }
    }
}
