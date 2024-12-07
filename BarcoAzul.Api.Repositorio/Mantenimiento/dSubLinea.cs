using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dSubLinea : dComun
    {
        public dSubLinea(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oSubLinea subLinea)
        {
            string query = "INSERT INTO SubLinea(Lin_Codigo, SubL_Codigo, SubL_Nombre) VALUES (@LineaId, @SubLineaId, @Descripcion)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, subLinea);
            }
        }

        public async Task Modificar(oSubLinea subLinea)
        {
            string query = @"UPDATE SubLinea SET SubL_Nombre = @Descripcion WHERE Lin_Codigo = @LineaId AND SubL_Codigo = @SubLineaId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, subLinea);
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = "DELETE SubLinea WHERE Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    lineaId = new DbString { Value = splitId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 }
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oSubLinea> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT
                                    Lin_Codigo AS LineaId,
									SubL_Codigo AS SubLineaId,
                                    SubL_Nombre AS Descripcion
                                FROM
                                    SubLinea
                                WHERE
                                    Lin_Codigo = @lineaId 
                                    AND SubL_Codigo = @subLineaId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oSubLinea>(query, new
                {
                    lineaId = new DbString { Value = splitId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 }
                });
            }
        }

        public async Task<IEnumerable<oSubLinea>> ListarTodos()
        {
            string query = @"   SELECT
                                    Lin_Codigo AS LineaId,
									SubL_Codigo AS SubLineaId,
                                    SubL_Nombre AS Descripcion
                                FROM
                                    SubLinea
                                WHERE
                                    NOT(Lin_Codigo = '.1' AND SubL_Codigo = '.1')
                                ORDER BY 
                                    SubL_Nombre";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oSubLinea>(query);
            }
        }

        public async Task<oPagina<vSubLinea>> Listar(string descripcion, oPaginacion paginacion)
        {
            string query = @$"SELECT
                                    SL.Lin_Codigo AS LineaId,
									SL.SubL_Codigo AS SubLineaId,
                                    SL.SubL_Nombre AS Descripcion,
									L.Lin_Nombre AS SubLineaDescripcion
                                FROM
                                    SubLinea SL
								    INNER JOIN Linea L ON SL.Lin_Codigo = L.Lin_Codigo
                                WHERE 
                                    SL.SubL_Nombre LIKE '%' + @descripcion + '%' 
                                    AND NOT(SL.Lin_Codigo = '.1' AND SL.SubL_Codigo = '.1')
                                ORDER BY 
                                    SL.SubL_Nombre 
                                {GetPaginacionQuery(paginacion)}";
            query += GetCountQuery(query);

            oPagina<vSubLinea> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<vSubLinea>
                    {
                        Data = await result.ReadAsync<vSubLinea>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = @"SELECT COUNT(Lin_Codigo) FROM SubLinea WHERE Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    lineaId = new DbString { Value = splitId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 }
                });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(string id, string descripcion)
        {
            var splitId = id is null ? null : SplitId(id);
            string query = $@"SELECT COUNT(Lin_Codigo) FROM SubLinea WHERE {(id is null ? string.Empty : "NOT(Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId) AND")} SubL_Nombre = @descripcion";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    lineaId = new DbString { Value = splitId?.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitId?.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 60 }
                });
                return existe > 0;
            }
        }

        public async Task<string> GetNuevoId(string lineaId) => await GetNuevoId("SELECT MAX(SubL_codigo) FROM SubLinea WHERE Lin_codigo = @lineaId", new { lineaId = new DbString { Value = lineaId, IsAnsi = true, IsFixedLength = true, Length = 2 } }, "0#");

        public static oSplitSubLineaId SplitId(string id) => new oSplitSubLineaId(id);
        #endregion
    }
}
