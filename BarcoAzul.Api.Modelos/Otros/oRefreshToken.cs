namespace BarcoAzul.Api.Modelos.Otros
{
    public class oRefreshToken
    {
        public string UsuarioId { get; set; }
        public string Token { get; set; }
        public DateTime Expiracion { get; set; }
    }
}
