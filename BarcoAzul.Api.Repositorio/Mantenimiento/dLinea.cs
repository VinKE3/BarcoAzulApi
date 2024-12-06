using Dapper;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dLinea : dComun
    {
        public dLinea(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oLinea linea)
        {
            string query = "INSERT INTO Linea (Lin_Codigo, Lin_Nombre) VALUES (@Id, @Descripcion)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, linea);
            }
        }
        public async Task Modificar(oLinea linea)
        {
            string query = "UPDATE Linea SET Lin_Nombre = @Descripcion WHERE Lin_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, linea);
            }
        }

        public async Task Eliminar(string id)
        {
            string query = "DELETE Linea WHERE Lin_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 2 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oLinea> GetPorId(string id)
        {
            string query = "SELECT Lin_Codigo AS Id, Lin_Nombre AS Descripcion FROM Linea WHERE Lin_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oLinea>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 2 } });
            }
        }

        public async Task<IEnumerable<oLinea>> ListarTodos()
        {
            string query = "SELECT Lin_Codigo AS Id, Lin_Nombre AS Descripcion FROM Linea WHERE Lin_Codigo <> '.1' ORDER BY Lin_Nombre";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oLinea>(query);
            }
        }

        public async Task<oPagina<oLinea>> Listar(string descripcion, oPaginacion paginacion)
        {
            string query = $"SELECT Lin_Codigo AS Id, Lin_Nombre AS Descripcion FROM Linea WHERE Lin_Nombre LIKE '%' + @descripcion + '%' AND Lin_Codigo <> '.1' ORDER BY Lin_Nombre {GetPaginacionQuery(paginacion)}";
            query += GetCountQuery(query);

            oPagina<oLinea> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<oLinea>
                    {
                        Data = await result.ReadAsync<oLinea>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = @"SELECT COUNT(Lin_Codigo) FROM Linea WHERE Lin_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 2 } });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string descripcion)
        {
            string query = $"SELECT COUNT(Lin_Codigo) FROM Linea WHERE {(id is null ? string.Empty : "Lin_Codigo <> @id AND")} Lin_Nombre = @descripcion";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 60 }
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId() => await GetNuevoId("SELECT MAX(Codigo) FROM v_lst_linea WHERE Codigo <> '99' AND Codigo NOT LIKE '[a-zA-Z][a-zA-Z]%'", null, "00");
        #endregion
    }
}
