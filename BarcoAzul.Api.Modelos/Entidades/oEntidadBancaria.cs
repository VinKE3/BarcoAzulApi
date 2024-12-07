using System.ComponentModel.DataAnnotations;
using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oEntidadBancaria
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "El tipo es requerido.")]
        public string Tipo { get; set; }
        public string NumeroDocumentoIdentidad { get; set; }

        #region Referencias
        public oTipoEntidadBancaria TipoEntidadBancaria { get; set; }
        #endregion

        public void ProcesarDatos()
        {
            Nombre = Nombre.Trim();
            NumeroDocumentoIdentidad = NumeroDocumentoIdentidad?.Trim();
        }
    }
}
