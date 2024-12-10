using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dSexo
    {
        public static IEnumerable<oSexo> ListarTodos()
        {
            yield return new oSexo { Id = "V", Descripcion = "VARÓN" };
            yield return new oSexo { Id = "M", Descripcion = "MUJER" };
        }

        public static oSexo GetPorId(string id) => ListarTodos().FirstOrDefault(x => x.Id == id);
    }
}
