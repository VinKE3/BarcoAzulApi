using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oCerrarMes
    {
        public int Anio { get; set; }
        public int MesNumero { get; set; }
        public bool IsCerrado { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string EmpresaId { get; set; }
        [JsonIgnore]
        public string UsuarioId { get; set; }
        #endregion
    }
}
