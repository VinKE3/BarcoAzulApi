using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dTipoUsuario
    {
        public static IEnumerable<oTipoUsuario> ListarTodos()
        {
            yield return new oTipoUsuario { Id = "NO", Descripcion = "NO CONFIGURADO" };
            yield return new oTipoUsuario { Id = "AD", Descripcion = "ADMINISTRADOR" };
            yield return new oTipoUsuario { Id = "MA", Descripcion = "MANTENIMIENTO" };
            yield return new oTipoUsuario { Id = "CO", Descripcion = "CONSULTA" };
            yield return new oTipoUsuario { Id = "PE", Descripcion = "PERSONALIZADO" };
        }

        public static oTipoUsuario GetPorId(string id) => ListarTodos().FirstOrDefault(x => x.Id == id);
    }
}
