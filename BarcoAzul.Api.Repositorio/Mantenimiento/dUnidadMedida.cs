using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dUnidadMedida : dComun
    {
        public dUnidadMedida(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oUnidadMedida unidadMedida)
        {
            string query = "INSERT INTO Unidad_Medida (Uni_Codigo, Uni_Nombre, Uni_CodigoSunat) VALUES (@Id, @Descripcion, @CodigoSunat)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, unidadMedida);
            }
        }

        public async Task Modificar(oUnidadMedida unidadMedida)
        {
            string query = @"UPDATE Unidad_Medida SET Uni_Nombre = @Descripcion, Uni_CodigoSunat = @CodigoSunat WHERE Uni_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, unidadMedida);
            }
        }

        public async Task Eliminar(string id)
        {
            string query = "DELETE Unidad_Medida WHERE Uni_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oUnidadMedida> GetPorId(string id)
        {
            string query = @"   SELECT 
                                    Uni_Codigo AS Id,
                                    Uni_Nombre AS Descripcion,
                                    Uni_CodigoSunat AS CodigoSunat
                                FROM 
                                    Unidad_Medida 
                                WHERE 
                                    Uni_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oUnidadMedida>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 } });
            }
        }

        public async Task<IEnumerable<oUnidadMedida>> ListarTodos()
        {
            string query = @"   SELECT 
                                    Uni_Codigo AS Id,
                                    Uni_Nombre AS Descripcion,
                                    Uni_CodigoSunat AS CodigoSunat
                                FROM
                                    Unidad_Medida
                                ORDER BY
                                    Uni_Nombre";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oUnidadMedida>(query);
            }
        }

        public async Task<oPagina<oUnidadMedida>> Listar(string descripcion, oPaginacion paginacion)
        {
            string query = @$"SELECT 
                                    Uni_Codigo AS Id,
                                    Uni_Nombre AS Descripcion,
                                    Uni_CodigoSunat AS CodigoSunat
                                FROM 
                                    Unidad_Medida 
                                WHERE 
                                    Uni_Nombre LIKE '%' + @descripcion + '%'
                                ORDER BY 
                                    Uni_Nombre
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<oUnidadMedida> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<oUnidadMedida>
                    {
                        Data = await result.ReadAsync<oUnidadMedida>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = @"SELECT COUNT(Uni_Codigo) FROM Unidad_Medida WHERE Uni_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 } });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string descripcion)
        {
            string query = $@"SELECT COUNT(Uni_Codigo) FROM Unidad_Medida WHERE {(id is null ? string.Empty : "Uni_Codigo <> @id AND")} Uni_Nombre = @descripcion";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 },
                    descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 60 }
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId() => await GetNuevoId("SELECT Max(Codigo) FROM v_lst_unidadmedida WHERE Codigo <> '99' AND Len(Codigo) = 2", null, "00");
        #endregion
    }
}
