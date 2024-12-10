namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oMenu
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public bool IsActivo { get; set; }
        public int SistemaAreaId { get; set; }
        public string SistemaAreaNombre { get; set; }
    }
}
