using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;


namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oDistrito
    {
        public string Id { get => $"{DepartamentoId}{ProvinciaId}{DistritoId}"; }
        [Required(ErrorMessage = "El departamento es requerido.")]
        public string DepartamentoId { get; set; }
        [Required(ErrorMessage = "La provincia es requerida.")]
        public string ProvinciaId { get; set; }
        [Required(ErrorMessage = "El código es requerido.")]
        [StringLength(2, MinimumLength = 2, ErrorMessage = "El código debe tener 2 dígitos.")]
        public string DistritoId { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }

        #region Referencias
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oDepartamento Departamento { get; set; }
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public oProvincia Provincia { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            Nombre = Nombre.Trim();
        }
    }
}
