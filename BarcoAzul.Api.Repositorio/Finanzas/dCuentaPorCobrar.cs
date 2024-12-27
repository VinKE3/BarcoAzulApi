using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Venta;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Finanzas
{
    public class dCuentaPorCobrar : dComun
    {
        public dCuentaPorCobrar(string connectionString) : base(connectionString) { }

        public async Task<oCuentaPorCobrar> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"	SELECT
									V.Conf_Codigo AS EmpresaId,
									V.TDoc_Codigo AS TipoDocumentoId,
									V.Ven_Serie AS Serie,
									V.Ven_Numero AS Numero,
									V.Ven_Fecha AS FechaEmision,
									V.Ven_Venci AS FechaVencimiento,
									V.Ven_Moneda AS MonedaId,
									V.Cli_Codigo AS ClienteId,
									RTRIM(CASE WHEN LEN(ISNULL(V.Ven_RazonSocial, '')) = 0 THEN C.Cli_RazonSocial ELSE V.Ven_RazonSocial END) AS ClienteNombre,
									V.Ven_Detraccion AS MontoDetraccion,
									V.Ven_Total AS Total,
									V.Ven_Abonado AS Abonado,
									V.Ven_Saldo AS Saldo,
									V.Ven_Observ AS Observacion
								FROM
									Venta V
									INNER JOIN Cliente C ON V.Cli_Codigo = C.Cli_Codigo
								WHERE
	                                V.Conf_Codigo = @empresaId
	                                AND V.TDoc_Codigo = @tipoDocumentoId
	                                AND V.Ven_Serie = @serie
	                                AND V.Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oCuentaPorCobrar>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<oPagina<vCuentaPorCobrar>> Listar(string[] tiposDocumento, DateTime fechaInicio, DateTime fechaFin, string clienteNombre, bool? isCancelado, string personalId, oPaginacion paginacion)
        {
            string query = @$"	SELECT 
									Codigo AS Id,
									Fecha AS FechaEmision,
									Documento AS NumeroDocumento,
									Vencimiento AS FechaVencimiento,
									Razon_Social AS ClienteNombre,
									Moneda AS MonedaId,
									Ven_Detraccion AS MontoDetraccion,
									Total,
									Abonado,
									Saldo,
									CAST(CASE WHEN Cancelado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsCancelado
								FROM 
									v_lst_venta
								WHERE 
									Anulado = 'N'
									AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
									AND TipoDoc IN ({JoinToQuery(tiposDocumento)})
									AND Razon_Social LIKE '%' + @clienteNombre + '%'
									{(isCancelado is null ? string.Empty : $"AND Cancelado = @isCancelado")}
									{(string.IsNullOrWhiteSpace(personalId) ? string.Empty : $"AND Per_Codigo = @personalId")}
								ORDER BY
									Fecha DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vCuentaPorCobrar> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    clienteNombre = new DbString { Value = clienteNombre, IsAnsi = true, IsFixedLength = false, Length = 250 },
                    isCancelado = new DbString { Value = isCancelado is null || !isCancelado.Value ? "N" : "S", IsAnsi = true, IsFixedLength = true, Length = 1 },
                    personalId = new DbString { Value = personalId, IsAnsi = true, IsFixedLength = true, Length = 8 }
                }))
                {
                    pagina = new oPagina<vCuentaPorCobrar>
                    {
                        Data = await result.ReadAsync<vCuentaPorCobrar>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<oPagina<vCuentaPorCobrarPendiente>> ListarPendientes(string numeroDocumento, oPaginacion paginacion, string tipoDocumentoId = "", string clienteId = "")
        {
            string query = $@"	SELECT
									Codigo AS Id,
									Fecha AS FechaEmision,
									Vencimiento AS FechaVencimiento,
									RTRIM(Ruc_Dni) AS ClienteNumero,
									Razon_Social AS ClienteNombre,
									Moneda AS MonedaId,
									Saldo AS Saldo,
									(CASE TipoDoc WHEN 'RI'
										THEN Ven_DireccionPart + ' / ' + Personal
										ELSE Documento + ' / ' + Razon_Social
									END) AS Descripcion
								FROM
									v_lst_Venta
								WHERE
									Anulado = 'N'
									AND Saldo > 0
									{(string.IsNullOrWhiteSpace(tipoDocumentoId) ? "AND TipoDoc IN ('01', '03', '08', '07', 'LC', 'RI')" : "AND TipoDoc = @tipoDocumentoId")}
									{(string.IsNullOrWhiteSpace(clienteId) ? string.Empty : "AND Cli_Codigo = @clienteId")}
									AND Documento LIKE '%' + @numeroDocumento + '%'
								ORDER BY
									Fecha DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vCuentaPorCobrarPendiente> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    tipoDocumentoId = new DbString { Value = tipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    numeroDocumento = new DbString { Value = numeroDocumento, IsAnsi = true, IsFixedLength = false, Length = 20 }
                }))
                {
                    pagina = new oPagina<vCuentaPorCobrarPendiente>
                    {
                        Data = await result.ReadAsync<vCuentaPorCobrarPendiente>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = "SELECT COUNT(Conf_Codigo) FROM Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
                return existe > 0;
            }
        }

        public async Task<decimal> GetSaldo(string documentoCobroId, string documentoVentaId)
        {
            var splitDocumentoVentaId = dDocumentoVenta.SplitId(documentoVentaId);
            var splitDocumentoCobroId = dDocumentoVenta.SplitId(documentoCobroId);
            decimal abono = 0;

            using (var db = GetConnection())
            {
                string query = @"SELECT Ven_Moneda, Ven_Saldo FROM Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

                var (documentoVentaMonedaId, saldo) = await db.QueryFirstAsync<(string MonedaId, decimal Saldo)>(query, new
                {
                    empresaId = new DbString { Value = splitDocumentoVentaId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitDocumentoVentaId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitDocumentoVentaId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitDocumentoVentaId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });

                query = @"  SELECT 
	                            Det_AboCodigo, 
	                            CAST(CASE WHEN Det_AfectarDeuda = 'S' THEN 1 ELSE 0 END AS BIT)
                            FROM 
	                            DetLetraRetencion 
                            WHERE 
	                            (Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero)
	                            AND Det_VentaCod = @documentoVentaId";

                var documentoCobroDatos = await db.QueryFirstOrDefaultAsync<(int DetalleId, bool AfectarDeuda)?>(query, new
                {
                    empresaId = new DbString { Value = splitDocumentoCobroId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitDocumentoCobroId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitDocumentoCobroId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitDocumentoCobroId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    documentoVentaId = new DbString { Value = documentoVentaId, IsAnsi = true, IsFixedLength = true, Length = 18 }
                });

                if (documentoCobroDatos is not null && documentoCobroDatos.Value.AfectarDeuda)
                {
                    query = @"SELECT Abo_MontoDol, Abo_MontoSol FROM Abono_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero AND Abo_Item = @detalleId";

                    var (abonoUSD, abonoPEN) = await db.QueryFirstAsync<(decimal AbonoUSD, decimal AbonoPEN)>(query, new
                    {
                        empresaId = new DbString { Value = splitDocumentoVentaId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        tipoDocumentoId = new DbString { Value = splitDocumentoVentaId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitDocumentoVentaId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitDocumentoVentaId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                        detalleId = documentoCobroDatos.Value.DetalleId
                    });

                    abono = documentoVentaMonedaId == "S" ? abonoPEN : abonoUSD;
                }

                saldo += abono;

                query = @"SELECT Ven_Moneda, Ven_TCambio FROM Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

                var (documentoCobroMonedaId, tipoCambio) = await db.QueryFirstAsync<(string DocumentoPagoMonedaId, decimal TipoCambio)>(query, new
                {
                    empresaId = new DbString { Value = splitDocumentoCobroId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitDocumentoCobroId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitDocumentoCobroId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitDocumentoCobroId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });

                if (documentoVentaMonedaId != documentoCobroMonedaId)
                {
                    saldo = documentoCobroMonedaId == "S"
                        ? decimal.Round(saldo * tipoCambio, 2, MidpointRounding.AwayFromZero)
                        : decimal.Round(saldo / tipoCambio, 2, MidpointRounding.AwayFromZero);
                }

                return saldo;
            }
        }

        private static oSplitDocumentoVentaId SplitId(string id) => new(id);
    }
}
