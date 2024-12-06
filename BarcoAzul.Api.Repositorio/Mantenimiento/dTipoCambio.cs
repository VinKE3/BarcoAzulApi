using Dapper;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dTipoCambio : dComun
    {
        public dTipoCambio(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oTipoCambio tipoCambio)
        {
            string query = @"   INSERT INTO Tipo_Cambio (tipc_fecha, tipc_compra, tipc_venta, tipc_activo, usu_codigo, tipc_fechacreacion, tipc_terminalcpu, tipc_prod)
                                VALUES (@Id, @PrecioCompra, @PrecioVenta, 'S', @UsuarioId, GETDATE(), @Origen, @PrecioProduccion)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, tipoCambio);
            }
        }

        public async Task Modificar(oTipoCambio tipoCambio)
        {
            string query = @"   UPDATE Tipo_Cambio SET tipc_compra = @PrecioCompra, tipc_venta = @PrecioVenta, tipc_fechamodificacion = GETDATE(),
                                tipc_usumodifica = @UsuarioId, tipc_prod = @PrecioProduccion WHERE tipc_fecha = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, tipoCambio);
            }
        }

        public async Task Eliminar(DateTime id)
        {
            string query = "DELETE Tipo_Cambio WHERE tipc_fecha = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id = id.ToString("dd/MM/yyyy") });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oTipoCambio> GetPorId(DateTime id)
        {
            string query = @"   SELECT 
                                    tipc_fecha AS Id,
                                    tipc_compra AS PrecioCompra,
                                    tipc_venta AS PrecioVenta,
                                    ISNULL(tipc_prod, 0) AS PrecioProduccion
                                FROM 
                                    Tipo_Cambio
                                WHERE 
                                    tipc_fecha = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oTipoCambio>(query, new { id = id.ToString("dd/MM/yyyy") });
            }
        }

        public async Task<oPagina<oTipoCambio>> Listar(int anio, int? mes, oPaginacion paginacion)
        {
            string query = $@"  SELECT
                                    tipc_fecha AS Id,
                                    tipc_compra AS PrecioCompra,
                                    tipc_venta AS PrecioVenta,
                                    ISNULL(tipc_prod, 0) AS PrecioProduccion
                                FROM
                                    Tipo_Cambio
                                WHERE
                                    YEAR(tipc_fecha) = @anio
                                    {(mes is null ? string.Empty : "AND MONTH(tipc_fecha) = @mes")}
                                ORDER BY
                                    tipc_fecha DESC
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<oTipoCambio> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { anio, mes }))
                {
                    pagina = new oPagina<oTipoCambio>
                    {
                        Data = await result.ReadAsync<oTipoCambio>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(DateTime id)
        {
            string query = @"SELECT COUNT(tipc_fecha) FROM Tipo_Cambio WHERE tipc_fecha = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = id.ToString("dd/MM/yyyy") });
                return existe > 0;
            }
        }
        #endregion
    }
}
