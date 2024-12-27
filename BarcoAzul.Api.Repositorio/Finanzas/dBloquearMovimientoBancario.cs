using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Finanzas
{
    public class dBloquearMovimientoBancario : dComun
    {
        public dBloquearMovimientoBancario(string connectionString) : base(connectionString) { }

        public async Task Procesar(oBloquearMovimientoBancario bloquearMovimientoBancario)
        {
            string query = "UPDATE MovCtaCte01 SET Mov_Bloqueado = @isBloqueado WHERE Mov_Codigo = @id";

            using (var db = GetConnection())
            {
                foreach (var id in bloquearMovimientoBancario.Ids)
                {
                    await db.QueryAsync(query, new
                    {
                        isBloqueado = bloquearMovimientoBancario.IsBloqueado ? "S" : "N",
                        id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 10 }
                    });
                }
            }
        }

        public async Task<oPagina<vBloquearMovimientoBancario>> Listar(DateTime fechaInicio, DateTime fechaFin, oPaginacion paginacion)
        {
            string query = $@"   SELECT
	                                Mov_Codigo AS Id,
                                    Mov_Fecha AS FechaEmision,
	                                Mov_Numero AS NumeroOperacion,
	                                Mov_Concepto AS Concepto,
	                                Mov_Moneda AS MonedaId,
	                                Mov_Total AS Monto,
	                                CAST(CASE WHEN Mov_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado
                                FROM
	                                MovCtaCte01
                                WHERE
	                                Mov_Fecha BETWEEN @fechaInicio ANd @fechaFin
                                ORDER BY
	                                Id
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vBloquearMovimientoBancario> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { fechaInicio, fechaFin }))
                {
                    pagina = new oPagina<vBloquearMovimientoBancario>
                    {
                        Data = await result.ReadAsync<vBloquearMovimientoBancario>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }
    }
}
