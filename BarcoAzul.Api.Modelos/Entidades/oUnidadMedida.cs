using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oUnidadMedida
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "La descripción es requerida.")]
        public string Descripcion { get; set; }
        [Required(ErrorMessage = "El código de sunat es requerido.")]
        public string CodigoSunat { get; set; }

        public void ProcesarDatos()
        {
            Descripcion = Descripcion.Trim();
            CodigoSunat = CodigoSunat.Trim();
        }
    }
}
