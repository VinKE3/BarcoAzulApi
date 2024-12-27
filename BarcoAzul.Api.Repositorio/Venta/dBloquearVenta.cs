using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Venta
{
    public class dBloquearVenta : dComun
    {
        public dBloquearVenta(string connectionString) : base(connectionString) { }

        public async Task Procesar(oBloquearVenta bloquearVenta)
        {
            string query = "UPDATE Venta SET Ven_Bloqueado = @isBloqueado WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                foreach (var id in bloquearVenta.Ids)
                {
                    var splitId = SplitId(id);

                    await db.QueryAsync(query, new
                    {
                        isBloqueado = bloquearVenta.IsBloqueado ? "S" : "N",
                        empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                    });
                }
            }
        }

        public async Task<oPagina<vBloquearVenta>> Listar(string[] tiposDocumentoNoPermitidos, string tipoDocumentoId, DateTime fechaInicio, DateTime fechaFin, oPaginacion paginacion)
        {
            string query = $@"	SELECT
									Codigo AS Id,
									CAST(CASE WHEN Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado,
									Documento AS NumeroDocumento,
									Fecha AS FechaEmision,
									Razon_Social AS ClienteNombre,
									Ruc_Dni AS ClienteNumero,
									Moneda AS MonedaId,
									Total
								FROM
									v_lst_venta
								WHERE
                                    {(string.IsNullOrWhiteSpace(tipoDocumentoId) ? string.Empty : "TipoDoc = @tipoDocumentoId AND")}
									{(tiposDocumentoNoPermitidos == null || tiposDocumentoNoPermitidos.Length == 0 ? string.Empty : $"TipoDoc NOT IN ({JoinToQuery(tiposDocumentoNoPermitidos)}) AND")}
									(Fecha BETWEEN @fechaInicio AND @fechaFin)
								ORDER BY
									Documento,
									Fecha
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vBloquearVenta> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    tipoDocumentoId = new DbString { Value = tipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    fechaInicio,
                    fechaFin
                }))
                {
                    pagina = new oPagina<vBloquearVenta>
                    {
                        Data = await result.ReadAsync<vBloquearVenta>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        private static oSplitDocumentoVentaId SplitId(string id) => new(id);
    }
}
