using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dTipoCobroPago : dComun
    {
        public dTipoCobroPago(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oTipoCobroPago tipoCobroPago)
        {
            string query = @"   INSERT INTO TIPO_PAGO(id_tipopago, tipp_nombre, tipp_abreviatura, tipp_plazo, id_tipoventa, id_formapago, tipp_activo)
                                VALUES (@Id, @Descripcion, @Abreviatura, @Plazo, @TipoVentaCompraId, 'EF', 'A')";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, tipoCobroPago);
            }
        }

        public async Task Modificar(oTipoCobroPago tipoCobroPago)
        {
            string query = "UPDATE TIPO_PAGO SET tipp_nombre = @Descripcion, tipp_abreviatura = @Abreviatura, tipp_plazo = @Plazo, id_tipoventa = @TipoVentaCompraId WHERE id_tipopago = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, tipoCobroPago);
            }
        }

        public async Task Eliminar(string id)
        {
            string query = @"DELETE TIPO_PAGO WHERE id_tipopago = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oTipoCobroPago> GetPorId(string id)
        {
            string query = @"   SELECT
	                                id_tipopago AS Id,
	                                id_tipoventa AS TipoVentaCompraId,
	                                tipp_nombre AS Descripcion,
	                                tipp_abreviatura AS Abreviatura,
	                                tipp_plazo AS Plazo
                                FROM 
                                    Tipo_Pago
                                WHERE 
                                    id_tipopago = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oTipoCobroPago>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 } });
            }
        }

        public async Task<IEnumerable<oTipoCobroPago>> ListarTodos()
        {
            string query = @"   SELECT
	                                id_tipopago AS Id,
	                                id_tipoventa AS TipoVentaCompraId,
	                                tipp_nombre AS Descripcion,
	                                tipp_abreviatura AS Abreviatura,
	                                tipp_plazo AS Plazo
                                FROM 
                                    Tipo_Pago
                                ORDER BY
                                    id_tipoventa, tipp_nombre";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oTipoCobroPago>(query);
            }
        }

        public async Task<oPagina<oTipoCobroPago>> Listar(string descripcion, oPaginacion paginacion)
        {
            string query = @$"  SELECT
	                                id_tipopago AS Id,
	                                id_tipoventa AS TipoVentaCompraId,
	                                tipp_nombre AS Descripcion,
	                                tipp_abreviatura AS Abreviatura,
	                                tipp_plazo AS Plazo
                                FROM 
                                    Tipo_Pago
                                WHERE 
                                    tipp_nombre LIKE '%' + @descripcion + '%'
                                ORDER BY
                                    id_tipoventa, tipp_nombre
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<oTipoCobroPago> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 50 } }))
                {
                    pagina = new oPagina<oTipoCobroPago>
                    {
                        Data = await result.ReadAsync<oTipoCobroPago>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = @"SELECT COUNT(id_tipopago) FROM TIPO_PAGO WHERE id_tipopago = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 } });
                return existe > 0;
            }
        }

        public async Task<(bool Existe, string ValorRepetido)> DatosRepetidos(string id, string descripcion, string abreviatura)
        {
            string query = $"SELECT COUNT(id_tipopago) FROM TIPO_PAGO WHERE {(id is null ? string.Empty : "id_tipopago <> @id AND")} tipp_nombre = @descripcion";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 },
                    descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 50 }
                });

                if (existe > 0)
                {
                    return (true, "descripción");
                }

                query = $"SELECT COUNT(id_tipopago) FROM TIPO_PAGO WHERE {(id is null ? string.Empty : "id_tipopago <> @id AND")} tipp_abreviatura = @abreviatura";

                existe = await db.QueryFirstAsync<int>(query, new
                {
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 },
                    abreviatura = new DbString { Value = abreviatura, IsAnsi = true, IsFixedLength = false, Length = 30 }
                });

                if (existe > 0)
                {
                    return (true, "abreviatura");
                }

                return (false, null);
            }
        }

        public async Task<string> GetNuevoId() => await GetNuevoId("SELECT MAX(Id_tipopago) FROM TIPO_PAGO WHERE id_tipopago NOT IN ('.0', '.1', 'CH','DE','EF','EI','II','CP', 'CU')", null, "0#");
        #endregion
    }
}
