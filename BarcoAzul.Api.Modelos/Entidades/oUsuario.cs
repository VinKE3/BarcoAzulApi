using BarcoAzul.Api.Modelos.Otros;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oUsuario
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "El nick es requerido.")]
        public string Nick { get; set; }
        public string TipoUsuarioId { get; set; }
        public string Observacion { get; set; }
        public bool IsActivo { get; set; }
        public bool HabilitarAfectarStock { get; set; }
        public bool HabilitarTipoCambio { get; set; }
        public bool EditarFechaPedidoVenta {  get; set; }
        public bool ReaperturaCerrarCuadre { get; set; }
        public string PersonalId { get; set; }
        [Required(ErrorMessage = "La clave es requerida.")]
        [DataType(DataType.Password)]
        public string Clave { get; set; }
        [Required(ErrorMessage = "No se ingresó la confirmación de clave.")]
        [DataType(DataType.Password)]
        [Compare(nameof(Clave), ErrorMessage = "La confirmación no coincide con la clave.")]
        public string ClaveConfirmacion { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioAutorizadorId { get; set; }
        #endregion

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oTipoUsuario TipoUsuario { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oPersonal Personal { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            Nick = Nick?.Trim();
            Observacion = Observacion?.Trim();
        }
    }
}
