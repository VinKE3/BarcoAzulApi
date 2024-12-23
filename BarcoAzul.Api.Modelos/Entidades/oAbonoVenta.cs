using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oAbonoVenta : IValidatableObject
    {
        public string Id => $"{EmpresaId}{TipoDocumentoId}{Serie}{Numero}";
        public string EmpresaId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public int AbonoId { get; set; }
        [Required(ErrorMessage = "La fecha es requerida.")]
        public DateTime Fecha { get; set; }
        [Required(ErrorMessage = "El concepto es requerido.")]
        public string Concepto { get; set; }
        public string MonedaId { get; set; }
        [Required(ErrorMessage = "El tipo de cambio es requerido.")]
        [Range(0.001, int.MaxValue, ErrorMessage = "El tipo de cambio no puede ser igual a cero (0.00)")]
        public decimal TipoCambio { get; set; }
        [Required(ErrorMessage = "El monto es requerido.")]
        [Range(0.01, int.MaxValue, ErrorMessage = "El monto no puede ser igual a cero (0.00)")]
        public decimal Monto { get; set; }
        public decimal MontoPEN { get; set; }
        public decimal MontoUSD { get; set; }
        public string DocumentoVentaId { get; set; }
        [Required(ErrorMessage = "El tipo de pago es requerido.")]
        public string TipoCobroId { get; set; }
        public string CuentaCorrienteId { get; set; }
        public string NumeroOperacion { get; set; }

        #region Adicionales
        [JsonIgnore]
        public string UsuarioId { get; set; }
        [JsonIgnore]
        public bool IsBloqueado { get; set; }
        [JsonIgnore]
        public string Hora => DateTime.Now.ToString("HH:mm:ss");
        #endregion

        public void ProcesarDatos()
        {
            Concepto = Concepto?.Trim();
        }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (TipoCobroId == "CH" || TipoCobroId == "DE" || TipoCobroId == "TR")
            {
                if (string.IsNullOrEmpty(CuentaCorrienteId))
                    yield return new ValidationResult("La cuenta corriente es requerida.");

                if (string.IsNullOrWhiteSpace(NumeroOperacion))
                    yield return new ValidationResult("El número de operación es requerido.");
            }
        }
    }
}
