using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Compra
{
    public class dBloquearCompra : dComun
    {
        public dBloquearCompra(string connectionString) : base(connectionString) { }

        public async Task Procesar(oBloquearCompra bloquearCompra)
        {
            string query = @"	UPDATE 
									Compra 
								SET 
									Com_Bloqueado = @isBloqueado 
								WHERE 
									Conf_Codigo = @empresaId 
									AND Prov_Codigo = @proveedorId 
									AND TDoc_Codigo = @tipoDocumentoId 
									AND Com_Serie = @serie 
									AND Com_Numero = @numero 
									AND Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                foreach (var id in bloquearCompra.Ids)
                {
                    var splitId = SplitId(id);

                    await db.QueryAsync(query, new
                    {
                        isBloqueado = bloquearCompra.IsBloqueado ? "S" : "N",
                        empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                        tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                        clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                    });
                }
            }
        }

        public async Task<oPagina<vBloquearCompra>> Listar(string[] tiposDocumento, string tipoDocumentoId, DateTime fechaInicio, DateTime fechaFin, oPaginacion paginacion)
        {
            string query = $@"	SELECT
									Codigo AS Id,
									Documento AS NumeroDocumento,
									Fecha AS FechaContable,
									Proveedor AS ProveedorNombre,
									Ruc AS ProveedorNumero,
									Moneda AS MonedaId,
									Total,
									CAST(CASE WHEN Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado
								FROM
									v_lst_compra
								WHERE
                                    {(string.IsNullOrWhiteSpace(tipoDocumentoId) ? string.Empty : "TipoDoc = @tipoDocumentoId AND")}
									{(tiposDocumento == null || tiposDocumento.Length == 0 ? string.Empty : $"TipoDoc IN ({JoinToQuery(tiposDocumento)}) AND ")}
									(Fecha BETWEEN @fechaInicio AND @fechaFin)
								ORDER BY 
									Documento,
									Fecha
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vBloquearCompra> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    tipoDocumentoId = new DbString { Value = tipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    fechaInicio,
                    fechaFin
                }))
                {
                    pagina = new oPagina<vBloquearCompra>
                    {
                        Data = await result.ReadAsync<vBloquearCompra>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        private static oSplitDocumentoCompraId SplitId(string id) => new(id);
    }
}
