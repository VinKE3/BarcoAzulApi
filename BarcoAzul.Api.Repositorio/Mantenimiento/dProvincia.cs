using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dProvincia : dComun
    {
        public dProvincia(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oProvincia provincia)
        {
            string query = @"INSERT INTO Provincia (Dep_Codigo, Pro_Codigo, Pro_Nombre) VALUES (@DepartamentoId, @ProvinciaId, @Nombre)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, provincia);
            }
        }

        public async Task Modificar(oProvincia provincia)
        {
            string query = @"UPDATE Provincia SET Pro_Nombre = @Nombre WHERE Dep_Codigo = @DepartamentoId AND Pro_Codigo = @ProvinciaId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, provincia);
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = @"DELETE Provincia WHERE Dep_Codigo = @departamentoId AND Pro_Codigo = @provinciaId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    departamentoId = new DbString { Value = splitId.DepartamentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    provinciaId = new DbString { Value = splitId.ProvinciaId, IsAnsi = true, IsFixedLength = true, Length = 2 }
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oProvincia> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT 
                                    Dep_Codigo AS DepartamentoId,
									Pro_Codigo AS ProvinciaId,
                                    Pro_Nombre AS Nombre
                                FROM 
                                    Provincia
                                WHERE 
                                    Dep_Codigo = @departamentoId 
                                    AND Pro_Codigo = @provinciaId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oProvincia>(query, new
                {
                    departamentoId = new DbString { Value = splitId.DepartamentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    provinciaId = new DbString { Value = splitId.ProvinciaId, IsAnsi = true, IsFixedLength = true, Length = 2 }
                });
            }
        }

        public async Task<IEnumerable<oProvincia>> ListarTodos()
        {
            string query = @"  SELECT 
                                    Dep_Codigo AS DepartamentoId,
									Pro_Codigo AS ProvinciaId,
                                    Pro_Nombre AS Nombre
                                FROM 
                                    Provincia";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oProvincia>(query);
            }
        }

        public async Task<oPagina<vProvincia>> Listar(string nombre, oPaginacion paginacion)
        {
            string query = @$"  SELECT 
                                    P.Dep_Codigo AS DepartamentoId,
	                                P.Pro_Codigo AS ProvinciaId,
                                    P.Pro_Nombre AS Nombre,
	                                D.Dep_Nombre As DepartamentoNombre
                                FROM 
                                    Provincia P
	                                INNER JOIN Departamento D ON P.Dep_Codigo = D.Dep_Codigo
                                WHERE 
                                    P.Pro_Nombre LIKE '%' + @nombre + '%'
                                ORDER BY
	                                DepartamentoNombre, Nombre
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vProvincia> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<vProvincia>
                    {
                        Data = await result.ReadAsync<vProvincia>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = @"SELECT COUNT(Pro_Codigo) FROM Provincia WHERE Dep_Codigo = @departamentoId AND Pro_Codigo = @provinciaId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    departamentoId = new DbString { Value = splitId.DepartamentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    provinciaId = new DbString { Value = splitId.ProvinciaId, IsAnsi = true, IsFixedLength = true, Length = 2 }
                });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string nombre)
        {
            var splitId = id is null ? null : SplitId(id);

            string query = @$"  SELECT 
                                    COUNT(Pro_Codigo) 
                                FROM 
                                    Provincia 
                                WHERE 
                                    {(id is null ? string.Empty : "NOT(Dep_Codigo = @departamentoId AND Pro_Codigo = @provinciaId) AND Dep_Codigo = @departamentoId AND ")}
                                    Pro_Nombre = @nombre";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    departamentoId = new DbString { Value = splitId?.DepartamentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    provinciaId = new DbString { Value = splitId?.ProvinciaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 }
                });
                return existe > 0;
            }
        }

        public static oSplitProvinciaId SplitId(string id) => new(id);
        #endregion
    }
}
