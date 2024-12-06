namespace BarcoAzul.Api.Modelos.Otros
{
    public class oRespuesta<T>
    {
        public bool Success { get; set; }
        public T Data { get; set; }
        public IEnumerable<oMensajeAgrupado> Messages { get; set; }
    }

    public class oRespuesta : oRespuesta<object> { }
}
