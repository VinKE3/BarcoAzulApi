using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dTipoOperacionBancaria
    {
        public static IEnumerable<oTipoOperacionBancaria> ListarTodos()
        {
            yield return new oTipoOperacionBancaria { Id = "DE", Descripcion = "DEPOSITO" };
            yield return new oTipoOperacionBancaria { Id = "TR", Descripcion = "TRANSFERENCIA" };
            yield return new oTipoOperacionBancaria { Id = "CH", Descripcion = "CHEQUE" };
            yield return new oTipoOperacionBancaria { Id = "CC", Descripcion = "CARGO A CUENTA" };
        }

        public static oTipoOperacionBancaria GetPorId(string id) => ListarTodos().FirstOrDefault(x => x.Id == id);
    }
}
