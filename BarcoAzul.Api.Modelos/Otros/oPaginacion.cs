namespace BarcoAzul.Api.Modelos.Otros
{
    public class oPaginacion
    {
        private int _pagina = 1;
        private int _cantidad = 50;
        private readonly int _cantidadMaxima = 50;

        public int Pagina
        {
            get => _pagina;
            set => _pagina = value <= 0 ? 1 : value;
        }
        public int Cantidad
        {
            get => _cantidad;
            set => _cantidad = value > _cantidadMaxima ? _cantidadMaxima : value;
        }
    }
}
