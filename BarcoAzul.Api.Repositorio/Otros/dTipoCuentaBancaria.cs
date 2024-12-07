using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dTipoCuentaBancaria
    {
        public static IEnumerable<oTipoCuentaBancaria> ListarTodos()
        {
            yield return new oTipoCuentaBancaria { Id = "AHORRO", Descripcion = "AHORRO" };
            yield return new oTipoCuentaBancaria { Id = "CORRIENTE", Descripcion = "CORRIENTE" };
            yield return new oTipoCuentaBancaria { Id = "HABER", Descripcion = "HABER" };
            yield return new oTipoCuentaBancaria { Id = "MAESTRA", Descripcion = "MAESTRA" };
            yield return new oTipoCuentaBancaria { Id = "DETRACCION", Descripcion = "DETRACCION" };
            yield return new oTipoCuentaBancaria { Id = "INTERBANCARIA", Descripcion = "INTERBANCARIA" };
        }

        public static oTipoCuentaBancaria GetPorId(string id) => ListarTodos().FirstOrDefault(x => x.Id == id);
    }
}
