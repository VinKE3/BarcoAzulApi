using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dEstadoCivil
    {
        public static IEnumerable<oEstadoCivil> ListarTodos()
        {
            yield return new oEstadoCivil { Id = "CA", Descripcion = "CASADO/A" };
            yield return new oEstadoCivil { Id = "CO", Descripcion = "CONVIVIENTE" };
            yield return new oEstadoCivil { Id = "DI", Descripcion = "DIVORCIADO/A" };
            yield return new oEstadoCivil { Id = "SO", Descripcion = "SOLTERO/A" };
            yield return new oEstadoCivil { Id = "UL", Descripcion = "UNIÓN LIBRE" };
            yield return new oEstadoCivil { Id = "VI", Descripcion = "VIUDO/A" };
        }

        public static oEstadoCivil GetPorId(string id) => ListarTodos().FirstOrDefault(x => x.Id == id);
    }
}
