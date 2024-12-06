namespace BarcoAzul.Api.Modelos.Otros
{
    public class oPagina<T>
    {
        public IEnumerable<T> Data { get; set; }
        public int Total { get; set; }
    }
}

