using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dDepartamento : dComun
    {
        public dDepartamento(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oDepartamento departamento)
        {
            string query = "INSERT INTO Departamento (Dep_Codigo, Dep_Nombre) VALUES (@Id, @Nombre)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, departamento);
            }
        }

        public async Task Modificar(oDepartamento departamento)
        {
            string query = "UPDATE Departamento SET Dep_Nombre = @Nombre WHERE Dep_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, departamento);
            }
        }

        public async Task Eliminar(string id)
        {
            string query = "DELETE Departamento WHERE Dep_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 2 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oDepartamento> GetPorId(string id)
        {
            string query = @"SELECT Dep_Codigo AS Id, Dep_Nombre AS Nombre FROM Departamento WHERE Dep_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oDepartamento>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 2 } });
            }
        }

        public async Task<IEnumerable<oDepartamento>> ListarTodos()
        {
            string query = @"   SELECT
                                    Dep_Codigo AS Id,
                                    Dep_Nombre AS Nombre
                                FROM
                                    Departamento
                                ORDER BY
                                    Dep_Nombre";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oDepartamento>(query);
            }
        }

        public async Task<oPagina<oDepartamento>> Listar(string nombre, oPaginacion paginacion)
        {
            string query = @$"  SELECT
                                    Dep_Codigo AS Id,
                                    Dep_Nombre AS Nombre
                                FROM
                                    Departamento
                                WHERE
                                    Dep_Nombre LIKE '%' + @nombre + '%'
                                ORDER BY
                                    Dep_Nombre
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<oDepartamento> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<oDepartamento>
                    {
                        Data = await result.ReadAsync<oDepartamento>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = @"SELECT COUNT(Dep_Codigo) FROM Departamento WHERE Dep_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 2 } });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string nombre)
        {
            string query = @$"SELECT COUNT(Dep_Codigo) FROM Departamento WHERE {(id is null ? string.Empty : "Dep_Codigo <> @id AND")} Dep_Nombre = @nombre";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 }
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId() => await GetNuevoId("SELECT MAX(Codigo) FROM v_lst_departamento", null, "00");
        #endregion
    }
}
