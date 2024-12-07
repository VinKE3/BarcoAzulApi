using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dMoneda
    {
        public static IEnumerable<oMoneda> ListarTodos()
        {
            yield return new oMoneda { Id = "S", Abreviatura = "S/", Descripcion = "SOLES" };
            yield return new oMoneda { Id = "D", Abreviatura = "US$", Descripcion = "DÓLARES AMERICANOS" };
        }

        public static oMoneda GetPorId(string id) => ListarTodos().FirstOrDefault(x => x.Id == id);
    }
}
