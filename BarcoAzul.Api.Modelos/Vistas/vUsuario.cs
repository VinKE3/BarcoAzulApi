namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vUsuario
    {
        public string Id { get; set; }
        public string Nick { get; set; }
        public string TipoUsuarioDescripcion { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaModificacion { get; set; }
        public bool IsActivo { get; set; }
    }
}
