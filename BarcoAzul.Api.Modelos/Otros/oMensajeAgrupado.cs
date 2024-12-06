namespace BarcoAzul.Api.Modelos.Otros
{

    public class oMensajeAgrupado
    {
        public MensajeTipo Tipo { get; set; }
        public IEnumerable<string> Textos { get; set; }
    }
}
