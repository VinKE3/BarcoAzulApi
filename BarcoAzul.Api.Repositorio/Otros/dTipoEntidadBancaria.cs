using BarcoAzul.Api.Modelos.Otros;


namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dTipoEntidadBancaria
    {
        public static IEnumerable<oTipoEntidadBancaria> ListarTodos()
        {
            yield return new oTipoEntidadBancaria { Id = "BANCO", Descripcion = "BANCO" };
            yield return new oTipoEntidadBancaria { Id = "CAJA", Descripcion = "CAJA" };
        }

        public static oTipoEntidadBancaria GetPorId(string id) => ListarTodos().FirstOrDefault(x => x.Id == id);
    }
}
