using BarcoAzul.Api.Modelos.Otros;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oCuentaCorriente
    {
        public string Id { get => $"{EmpresaId}{CuentaCorrienteId}"; }
        public string EmpresaId { get; set; }
        public string CuentaCorrienteId { get; set; }
        [Required(ErrorMessage = "La entidad bancaria es requerida.")]
        public int EntidadBancariaId { get; set; }
        [Required(ErrorMessage = "El número de cuenta es requerido.")]
        public string Numero { get; set; }
        [Required(ErrorMessage = "El tipo de cuenta es requerido.")]
        public string TipoCuentaDescripcion { get; set; }
        public string MonedaId { get; set; }
        public string Observacion { get; set; }

        #region Datos Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oEntidadBancaria EntidadBancaria { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oMoneda Moneda { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            Numero = Numero.Trim();
            Observacion = Observacion?.Trim();
        }
    }
}
