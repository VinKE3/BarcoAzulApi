using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dMarca : dComun
    {
        public dMarca(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oMarca marca)
        {
            string query = @"   SET IDENTITY_INSERT Marca ON
                                INSERT INTO Marca(Mar_Codigo, Mar_Nombre) VALUES(@Id, @Nombre)
                                SET IDENTITY_INSERT Marca OFF";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, marca);
            }
        }

        public async Task Modificar(oMarca marca)
        {
            string query = @"UPDATE Marca SET Mar_Nombre = @Nombre WHERE Mar_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, marca);
            }
        }

        public async Task Eliminar(int id)
        {
            string query = "DELETE Marca WHERE Mar_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oMarca> GetPorId(int id)
        {
            string query = "SELECT Mar_Codigo AS Id, Mar_Nombre AS Nombre FROM Marca WHERE Mar_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oMarca>(query, new { id });
            }
        }

        public async Task<IEnumerable<oMarca>> ListarTodos()
        {
            string query = "SELECT Mar_Codigo AS Id, Mar_Nombre AS Nombre FROM Marca ORDER BY Mar_Nombre";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oMarca>(query);
            }
        }

        public async Task<oPagina<oMarca>> Listar(string nombre, oPaginacion paginacion)
        {
            string query = $"SELECT Mar_Codigo AS Id, Mar_Nombre AS Nombre FROM Marca WHERE Mar_Nombre LIKE '%' + @nombre + '%' ORDER BY Mar_Nombre {GetPaginacionQuery(paginacion)}";
            query += GetCountQuery(query);

            oPagina<oMarca> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<oMarca>
                    {
                        Data = await result.ReadAsync<oMarca>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(int id)
        {
            string query = @"SELECT COUNT(Mar_Codigo) FROM Marca WHERE Mar_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(int? id, string nombre)
        {
            string query = $@"SELECT COUNT(Mar_Codigo) FROM Marca WHERE {(id is null ? string.Empty : "Mar_Codigo <> @id AND")} Mar_Nombre = @nombre";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id,
                    nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 }
                });
                return existe > 0;
            }
        }

        public async Task<int> GetNuevoId()
        {
            using (var db = GetConnection())
            {
                int? nuevoId = await db.QueryFirstOrDefaultAsync<int?>("SELECT MAX(Mar_Codigo) FROM Marca");

                if (nuevoId.HasValue)
                    return nuevoId.Value + 1;

                return 2; //1 es ocupado por la marca por Defecto
            }
        }
        #endregion
    }
}
