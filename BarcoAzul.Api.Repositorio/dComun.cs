using Dapper;
using BarcoAzul.Api.Modelos.Otros;
using System.Data.SqlClient;

namespace BarcoAzul.Api.Repositorio
{
    public class dComun
    {
        internal readonly string _connectionString;

        public dComun(string connectionString) {
            _connectionString = connectionString;
        }

        internal SqlConnection GetConnection() => new SqlConnection(_connectionString);

        internal string GetPaginacionQuery(oPaginacion paginacion) => $" OFFSET {(paginacion.Pagina - 1) * paginacion.Cantidad} ROWS FETCH NEXT {paginacion.Cantidad} ROWS ONLY";

        internal string GetCountQuery(string query)
        {
            var indexSelect = query.IndexOf("SELECT");
            var indexFrom = query.IndexOf("FROM", indexSelect);

            string resultado = query.Substring(indexSelect + 6, indexFrom - indexSelect - 6);
            query = query.Replace(resultado, " COUNT(*) ");

            var indexOrderBy = query.IndexOf("ORDER BY");
            query = query.Substring(0, indexOrderBy);

            return " " + query;
        }

        internal string JoinToQuery(string[] values)
        {
            if (values == null || values.Length == 0)
                return "''";

            return string.Join(", ", values.Select(x => $"'{x}'"));
        }

        internal async Task<string> GetNuevoId(string query, object parameters, string formato)
        {
            var nuevoId = string.Empty;
            int @default = 1;

            using (var db = GetConnection())
            {
                if (parameters == null)
                    nuevoId = await db.QueryFirstOrDefaultAsync<string>(query);
                else
                    nuevoId = await db.QueryFirstOrDefaultAsync<string>(query, parameters);
            }

            if (int.TryParse(nuevoId, out int Id))
                return (Id + 1).ToString(formato);

            return @default.ToString(formato);
        }
    }
}
