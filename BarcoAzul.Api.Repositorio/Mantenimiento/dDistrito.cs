using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dDistrito : dComun
    {
        public dDistrito(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oDistrito distrito)
        {
            string query = @"INSERT INTO Distrito (Dep_Codigo, Pro_Codigo, Dis_Codigo, Dis_Nombre) VALUES (@DepartamentoId, @ProvinciaId, @DistritoId, @Nombre)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, distrito);
            }
        }

        public async Task Modificar(oDistrito distrito)
        {
            string query = @"UPDATE Distrito SET Dis_Nombre = @Nombre WHERE Dep_Codigo = @DepartamentoId AND Pro_Codigo = @provinciaId AND Dis_Codigo = @DistritoId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, distrito);
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = @"DELETE Distrito WHERE Dep_Codigo = @departamentoId AND Pro_Codigo = @provinciaId AND Dis_Codigo = @distritoId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    departamentoId = new DbString { Value = splitId.DepartamentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    provinciaId = new DbString { Value = splitId.ProvinciaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    distritoId = new DbString { Value = splitId.DistritoId, IsAnsi = true, IsFixedLength = true, Length = 2 }
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oDistrito> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT 
                                    Dep_Codigo AS DepartamentoId,
									Pro_Codigo AS ProvinciaId,
									Dis_Codigo AS DistritoId,
                                    Dis_Nombre AS Nombre
                                FROM 
                                    Distrito
                                WHERE 
                                    Dep_Codigo = @departamentoId 
                                    AND Pro_Codigo = @provinciaId 
                                    AND Dis_Codigo = @distritoId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oDistrito>(query, new
                {
                    departamentoId = new DbString { Value = splitId.DepartamentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    provinciaId = new DbString { Value = splitId.ProvinciaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    distritoId = new DbString { Value = splitId.DistritoId, IsAnsi = true, IsFixedLength = true, Length = 2 }
                });
            }
        }

        public async Task<IEnumerable<oDistrito>> ListarTodos()
        {
            string query = @"  SELECT
                                    Dep_Codigo AS DepartamentoId,
									Pro_Codigo AS ProvinciaId,
									Dis_Codigo AS DistritoId,
                                    Dis_Nombre AS Nombre
                                FROM 
                                    Distrito";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oDistrito>(query);
            }
        }

        public async Task<oPagina<vDistrito>> Listar(string nombre, oPaginacion paginacion)
        {
            string query = @$"  SELECT
                                    DIS.Dep_Codigo AS DepartamentoId,
	                                DIS.Pro_Codigo AS ProvinciaId,
	                                DIS.Dis_Codigo AS DistritoId,
                                    DIS.Dis_Nombre AS Nombre,
	                                DEP.Dep_Nombre AS DepartamentoNombre,
	                                PRO.Pro_Nombre AS ProvinciaNombre
                                FROM 
                                    Distrito DIS
	                                INNER JOIN Departamento DEP ON DIS.Dep_Codigo = DEP.Dep_Codigo
	                                INNER JOIN Provincia PRO ON DIS.Dep_Codigo = PRO.Dep_Codigo AND DIS.Pro_Codigo = PRO.Pro_Codigo
                                WHERE 
                                    DIS.Dis_Nombre LIKE '%' + @nombre + '%'
                                ORDER BY 
	                                DepartamentoNombre, ProvinciaNombre, Nombre
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vDistrito> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<vDistrito>
                    {
                        Data = await result.ReadAsync<vDistrito>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = @"SELECT COUNT(Dis_Codigo) FROM Distrito WHERE Dep_Codigo = @departamentoId AND Pro_Codigo = @provinciaId AND Dis_Codigo = @distritoId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    departamentoId = new DbString { Value = splitId.DepartamentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    provinciaId = new DbString { Value = splitId.ProvinciaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    distritoId = new DbString { Value = splitId.DistritoId, IsAnsi = true, IsFixedLength = true, Length = 2 }
                });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string nombre)
        {
            var splitId = id is null ? null : SplitId(id);

            string query = @$"  SELECT 
                                    COUNT(Dis_Codigo) 
                                FROM 
                                    Distrito 
                                WHERE 
                                    {(id is null ? string.Empty : "NOT(Dep_Codigo = @departamentoId AND Pro_Codigo = @provinciaId AND Dis_Codigo = @distritoId) AND Dep_Codigo = @departamentoId AND Pro_Codigo = @provinciaId AND")}
                                    Dis_Nombre = @nombre";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    departamentoId = new DbString { Value = splitId?.DepartamentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    provinciaId = new DbString { Value = splitId?.ProvinciaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    distritoId = new DbString { Value = splitId?.DistritoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 }
                });
                return existe > 0;
            }
        }

        public static oSplitDistritoId SplitId(string id) => new(id);
        #endregion
    }
}
