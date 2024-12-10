using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dTipoVentaCompra
    {
        public static IEnumerable<oTipoVentaCompra> ListarTodos()
        {
            yield return new oTipoVentaCompra { Id = "CO", Descripcion = "CONTADO" };
            yield return new oTipoVentaCompra { Id = "CR", Descripcion = "CREDITO" };
        }

        public static oTipoVentaCompra GetPorId(string id) => ListarTodos().FirstOrDefault(x => x.Id == id);
    }
}
