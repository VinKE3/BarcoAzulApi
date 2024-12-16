using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Informes
{
    public class FormatoUtilidades
    {
        public static string GetExtension(FormatoInforme formato)
        {
            return formato switch
            {
                FormatoInforme.PDF => ".pdf",
                FormatoInforme.Excel => ".xlsx",
                _ => string.Empty
            };
        }

        public static IEnumerable<oFormatoInforme> ListarTodos()
        {
            foreach (var formato in Enum.GetValues(typeof(FormatoInforme)))
            {
                yield return new oFormatoInforme { Id = (int)formato, Descripcion = formato.ToString() };
            }
        }
    }
}
