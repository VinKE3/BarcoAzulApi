using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dTipoMovimientoBancario
    {
        public static IEnumerable<oTipoMovimientoBancario> ListarTodos()
        {
            yield return new oTipoMovimientoBancario { Id = "IN", Descripcion = "INGRESO" };
            yield return new oTipoMovimientoBancario { Id = "EG", Descripcion = "EGRESO" };
        }

        public static oTipoMovimientoBancario GetPorId(string id) => ListarTodos().FirstOrDefault(x => x.Id == id);
    }
}
