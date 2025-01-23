using System.Configuration;
using System.Data.SqlClient;

namespace BarcoAzul.Api.Repositorio.Configuracion
{
    public class Conexion
    {
        //private static string DefaultConexion
        //{
        //    get
        //    {
        //        var connectionString = ConfigurationManager.ConnectionStrings["DefaultConexion"].ConnectionString;

        //        if (string.IsNullOrEmpty(connectionString))
        //            throw new Exception("Agregue la cadena de conexión por defecto en el App o Web config.");

        //        return connectionString;
        //    }
        //}

        //private static SqlConnection conn { get; set; }
        //private static string NombreConexionActual { get; set; }

        /// <summary>
        /// Crea o retorna y abre la conexión a la BD.
        /// </summary>
        /// <param name="NombreConexion">Nombre de la conexión en Web.Config</param>
        /// <returns></returns>
        public static SqlConnection Get(string NombreConexion = "")
        {
            if (string.IsNullOrEmpty(NombreConexion)) //En caso haya que conectarse a más de una BD, sino le pone la conexión por defecto
                NombreConexion = "DefaultConexion";

            return new SqlConnection(ConfigurationManager.ConnectionStrings[NombreConexion].ConnectionString);
        }

        /// <summary>
        /// Crea y abre la conexión en caso no exista. En caso sea una conexión distinta a la actual, reemplazará la actual por la conexión solicitada.
        /// </summary>
        /// <param name="NombreConexion">Nombre de la conexión a usar (Por defecto es DefaultConexion)</param>
        //private static void RevisarConexion(string NombreConexion = "")
        //{
        //    if (string.IsNullOrEmpty(NombreConexion)) //En caso haya que conectarse a más de una BD, sino le pone la conexión por defecto
        //        NombreConexion = DefaultConexion;

        //    if (conn == null || NombreConexionActual != NombreConexion) //Revisar si la conexión existe
        //    {
        //        NombreConexionActual = NombreConexion;
        //        conn = new SqlConnection(NombreConexion);
        //        conn.Open();
        //    }
        //}

        //public static IEnumerable<T> Query<T>(string query, object parametros) => Query<T>(query, parametros, DefaultConexion);

        //public static IEnumerable<T> Query<T>(string query, object parametros, string NombreConexion)
        //{
        //    RevisarConexion(NombreConexion);

        //    if (parametros == null)
        //    {
        //        return conn.Query<T>(query);
        //    }
        //    else
        //    {
        //        return conn.Query<T>(query, parametros);
        //    }
        //}

        //public static dynamic Query(string query, object parametros, string NombreConexion)
        //{
        //    RevisarConexion(NombreConexion);
        //    return conn.Query(query, parametros);
        //}
    }
}
