using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Modelos.Atributos
{
    public class MensajeException : Exception
    {
        public oMensaje Mensaje { get; private set; }

        public MensajeException(oMensaje mensaje)
        {
            Mensaje = mensaje;
        }
    }
}
