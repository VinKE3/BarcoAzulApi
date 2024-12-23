using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Compra;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Finanzas
{
    public class dCuentaPorPagar : dComun
    {
        public dCuentaPorPagar(string connectionString) : base(connectionString) { }

        public async Task<oCuentaPorPagar> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT	 
	                                Conf_Codigo AS EmpresaId,
	                                Prov_Codigo AS ProveedorId,
	                                TDoc_Codigo AS TipoDocumentoId,
	                                Com_Serie AS Serie,
	                                Com_Numero AS Numero,
	                                Cli_Codigo AS ClienteId,
	                                Com_Fecha AS FechaContable,
	                                Com_Moneda AS MonedaId,
	                                Com_Total AS Total,
	                                Com_Abonado AS Abonado,
	                                Com_Saldo AS Saldo,
	                                Com_Observ AS Observacion
                                FROM
	                                Compra
                                WHERE
	                                Conf_Codigo = @empresaId
	                                AND Prov_Codigo = @proveedorId
	                                AND TDoc_Codigo = @tipoDocumentoId
	                                AND Com_Serie = @serie
	                                AND Com_Numero = @numero
	                                AND Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oCuentaPorPagar>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });
            }
        }

        public async Task<oPagina<vCuentaPorPagar>> Listar(string[] tiposDocumento, DateTime fechaInicio, DateTime fechaFin, string proveedorNombre, bool? isCancelado, oPaginacion paginacion)
        {
            string query = $@"   SELECT 
	                                Codigo AS Id,
	                                Fecha AS FechaContable,
	                                Vencimiento AS FechaVencimiento,
	                                Documento AS NumeroDocumento,
	                                Proveedor AS ProveedorNombre,
	                                Moneda AS MonedaId,
	                                Total,
	                                Abonado,
	                                Saldo,
	                                CAST(CASE WHEN Cancelado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsCancelado
                                FROM 
	                                v_lst_compra
                                WHERE 
	                                (Fecha BETWEEN @fechaInicio AND @fechaFin)
	                                AND TipoDoc IN ({JoinToQuery(tiposDocumento)})
	                                AND Proveedor LIKE '%' + @proveedorNombre + '%'
	                                {(isCancelado is null ? string.Empty : $"AND Cancelado = @isCancelado")}
                                ORDER BY
                                    Fecha DESC,
                                    Proveedor DESC,
                                    Documento DESC
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vCuentaPorPagar> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    proveedorNombre = new DbString { Value = proveedorNombre, IsAnsi = true, IsFixedLength = false, Length = 100 },
                    isCancelado = new DbString { Value = isCancelado is null || !isCancelado.Value ? "N" : "S", IsAnsi = true, IsFixedLength = true, Length = 1 }
                }))
                {
                    pagina = new oPagina<vCuentaPorPagar>
                    {
                        Data = await result.ReadAsync<vCuentaPorPagar>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<oPagina<vCuentaPorPagarPendiente>> ListarPendientes(string numeroDocumento, oPaginacion paginacion, string tipoDocumentoId = "", string proveedorId = "")
        {
            string query = $@"   SELECT 
	                                Codigo AS Id,
	                                Fecha AS FechaContable,
	                                Emision AS FechaEmision,
	                                Vencimiento AS FechaVencimiento,
	                                Documento,
	                                Moneda AS MonedaId,
	                                Documento + ' / ' + Proveedor AS Descripcion,
	                                Saldo
                                FROM 
	                                v_lst_compra
                                WHERE 
	                                Saldo > 0
                                    {(string.IsNullOrWhiteSpace(tipoDocumentoId) ? "AND TipoDoc NOT IN ('09', 'OC', 'EC', 'OE', 'RO', '13', 'EN')" : "AND TipoDoc = @tipoDocumentoId")}
									{(string.IsNullOrWhiteSpace(proveedorId) ? string.Empty : "AND Prov_Codigo = @proveedorId")}
	                                AND Documento LIKE '%' + @numeroDocumento + '%'
                                ORDER BY
                                    Fecha DESC,
                                    Documento DESC
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vCuentaPorPagarPendiente> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    tipoDocumentoId = new DbString { Value = tipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = proveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    numeroDocumento = new DbString { Value = numeroDocumento, IsAnsi = true, IsFixedLength = false, Length = 24 }
                }))
                {
                    pagina = new oPagina<vCuentaPorPagarPendiente>
                    {
                        Data = await result.ReadAsync<vCuentaPorPagarPendiente>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);

            string query = $@"  SELECT COUNT(Conf_Codigo) FROM Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Com_Serie = @serie AND Com_Numero = @numero {(string.IsNullOrWhiteSpace(splitId.ClienteId) ? string.Empty : "AND Cli_Codigo = @clienteId")}";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });
                return existe > 0;
            }
        }

        public async Task<bool> IsBloqueado(string id)
        {
            var splitId = SplitId(id);

            string query = $@"  SELECT CAST(CASE WHEN Com_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) FROM Compra
                                WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Com_Serie = @serie AND Com_Numero = @numero {(string.IsNullOrWhiteSpace(splitId.ClienteId) ? string.Empty : "AND Cli_Codigo = @clienteId")}";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<bool>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                });
            }
        }

        public async Task<decimal> GetSaldo(string documentoPagoId, string documentoCompraId)
        {
            var splitDocumentoCompraId = dDocumentoCompra.SplitId(documentoCompraId);
            var splitDocumentoPagoId = dDocumentoCompra.SplitId(documentoPagoId);
            decimal abono = 0;

            using (var db = GetConnection())
            {
                string query = @"   SELECT Com_Moneda AS MonedaId, Com_Saldo AS Saldo FROM Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                    AND Com_Serie = @serie AND Com_Numero = @numero";

                (string documentoCompraMonedaId, decimal saldo) = await db.QueryFirstAsync<(string MonedaId, decimal Saldo)>(query, new
                {
                    empresaId = new DbString { Value = splitDocumentoCompraId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitDocumentoCompraId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitDocumentoCompraId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitDocumentoCompraId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitDocumentoCompraId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });

                query = @"  SELECT 
	                            Det_AboCodigo AS DetalleId, 
	                            CAST(CASE WHEN Det_AfectarDeuda = 'S' THEN 1 ELSE 0 END AS BIT) AS AfectarDeuda
                            FROM 
	                            DetLetraPercepcion 
                            WHERE 
	                            (Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero)
	                            AND Det_CompaCod = @documentoCompraId";

                var documentoPagoData = await db.QueryFirstOrDefaultAsync<(int DetalleId, bool AfectarDeuda)?>(query, new
                {
                    empresaId = new DbString { Value = splitDocumentoPagoId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitDocumentoPagoId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitDocumentoPagoId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitDocumentoPagoId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitDocumentoPagoId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    documentoCompraId = new DbString { Value = documentoCompraId, IsAnsi = true, IsFixedLength = true, Length = 24 }
                });

                if (documentoPagoData is not null && documentoPagoData.Value.AfectarDeuda)
                {
                    query = @"SELECT Abo_MontoDol AS AbonoUSD, Abo_MontoSol AS AbonoPEN FROM Abono_Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Abo_Item = @detalleId";

                    (decimal abonoUSD, decimal abonoPEN) = await db.QueryFirstAsync<(decimal AbonoUSD, decimal AbonoPEN)>(query, new
                    {
                        empresaId = new DbString { Value = splitDocumentoCompraId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        proveedorId = new DbString { Value = splitDocumentoCompraId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                        tipoDocumentoId = new DbString { Value = splitDocumentoCompraId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitDocumentoCompraId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitDocumentoCompraId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                        detalleId = documentoPagoData.Value.DetalleId
                    });

                    abono = documentoCompraMonedaId == "S" ? abonoPEN : abonoUSD;
                }

                saldo += abono;

                query = @"  SELECT Com_Moneda, Com_TCambio FROM Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                            AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

                (string documentoPagoMonedaId, decimal tipoCambio) = await db.QueryFirstAsync<(string DocumentoPagoMonedaId, decimal TipoCambio)>(query, new
                {
                    empresaId = new DbString { Value = splitDocumentoPagoId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitDocumentoPagoId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitDocumentoPagoId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitDocumentoPagoId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitDocumentoPagoId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitDocumentoPagoId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });

                if (documentoCompraMonedaId != documentoPagoMonedaId)
                {
                    saldo = documentoPagoMonedaId == "S"
                        ? decimal.Round(saldo * tipoCambio, 2, MidpointRounding.AwayFromZero)
                        : decimal.Round(saldo / tipoCambio, 2, MidpointRounding.AwayFromZero);
                }

                return saldo;
            }
        }

        private static oSplitDocumentoCompraId SplitId(string id) => new(id);
    }
}
