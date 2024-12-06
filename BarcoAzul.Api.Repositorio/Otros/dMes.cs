using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dMes
    {
        public static IEnumerable<oMes> ListarTodos()
        {
            yield return new oMes { Numero = 1, Nombre = "ENERO" };
            yield return new oMes { Numero = 2, Nombre = "FEBRERO" };
            yield return new oMes { Numero = 3, Nombre = "MARZO" };
            yield return new oMes { Numero = 4, Nombre = "ABRIL" };
            yield return new oMes { Numero = 5, Nombre = "MAYO" };
            yield return new oMes { Numero = 6, Nombre = "JUNIO" };
            yield return new oMes { Numero = 7, Nombre = "JULIO" };
            yield return new oMes { Numero = 8, Nombre = "AGOSTO" };
            yield return new oMes { Numero = 9, Nombre = "SEPTIEMBRE" };
            yield return new oMes { Numero = 10, Nombre = "OCTUBRE" };
            yield return new oMes { Numero = 11, Nombre = "NOVIEMBRE" };
            yield return new oMes { Numero = 12, Nombre = "DICIEMBRE" };
        }

        public static oMes GetPorNumero(int numero) => ListarTodos().FirstOrDefault(x => x.Numero == numero);
    }
}
