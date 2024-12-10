namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oUsuarioPermiso
    {
        public string UsuarioId { get; set; }
        public string MenuId { get; set; }
        public bool Registrar { get; set; }
        public bool Modificar { get; set; }
        public bool Eliminar { get; set; }
        public bool Consultar { get; set; }
        public bool Anular { get; set; }
    }
}
