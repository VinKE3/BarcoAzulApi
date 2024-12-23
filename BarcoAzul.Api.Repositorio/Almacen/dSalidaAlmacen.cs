using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Repositorio.Almacen
{
    public class dSalidaAlmacen : dComun
    {
        public const string TipoDocumentoId = "SA";

        public dSalidaAlmacen(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oSalidaAlmacen salidaAlmacen)
        {
            string query = @"	INSERT INTO Venta (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Ven_Venci, Ven_Fecha, Cli_Codigo, Ven_RucDni, Ven_Moneda, Ven_TCambio, 
								Per_Codigo, Ven_DireccionPart, Ven_DireccionLleg, Ven_NroComp, Ven_GuiaRemision, Ven_Observ, Ven_IncluyeIgv, Ven_PorcIgv, Ven_Otros, Ven_Flat01, 
								Ven_Flat02, Ven_Hora, Usu_Codigo, Ven_Cancelado, Ven_Abonado, Ven_Saldo, Ven_Documento, Ven_FechaReg, Suc_Codigo, Ven_AfectarStock, Ven_IngEgrStock,
                                Ven_TVenta, Ven_TPago, Ven_SubTotal, Ven_PorcDscto, Ven_Descuento, Ven_ValorVenta, Ven_TotalNeto, Ven_MontoIgv, Ven_PorcReten, Ven_Retencion,
                                Ven_PorcPercep, Ven_Percepcion, Ven_Inafecto, Ven_Total, Ven_Anulado, Ven_ConIgv, Ven_Retenc, Ven_Percep, Ven_TotalSol, Ven_TotalDol, Ven_BloqUsu, 
                                Ven_BloqSist, Ven_BloqProc, Ven_SubTotalSol, Ven_ValorVentaSol, Ven_TotalNetoSol, Ven_MontoIgvSol, Ven_OtrosSol, Ven_RetencionSol, Ven_PercepcionSol,
                                Ven_InafectoSol, Ven_SubTotalDol, Ven_ValorVentaDol, Ven_TotalNetoDol, Ven_MontoIgvDol, Ven_OtrosDol, Ven_RetencionDol, Ven_PercepcionDol, Ven_InafectoDol,
                                Ven_Bloqueado)
								VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @FechaInicio, @FechaTerminacion, @ClienteId, @ClienteNumeroDocumentoIdentidad, @MonedaId, @TipoCambio, 
								@PersonalId, @LineaProduccion, @Envasado, @NumeroLote, @GuiaRemision, @Observacion, @IncluyeIGV, @PorcentajeIGV, @GastosIndirectos, @CantidadSolicitada, 
								@CantidadProducida, @HoraEmision, @UsuarioId, 'N', 0, @Total, @NumeroDocumento, GETDATE(), '01', 'S', '-',
                                'CO', 'EF', @subTotal, 0, @TotalGalones, @subTotal, @Total, @montoIGV, 0, 0,
                                0, 0, 0, @Total, 'N', 'S', 'N', 'N', @totalPEN, @totalUSD, 'N',
                                'N', 'N', @subTotalPEN, @subTotalPEN, @totalPEN, @montoIGVPEN, 0, 0, 0,
                                0, @subTotalUSD, @subTotalUSD, @totalUSD, @montoIGVUSD, 0, 0, 0, 0,
                                'N')";

            decimal subTotal = 0, montoIGV = 0;
            decimal subTotalPEN = 0, montoIGVPEN = 0, totalPEN = 0;
            decimal subTotalUSD = 0, montoIGVUSD = 0, totalUSD = 0;

            montoIGV = decimal.Round(salidaAlmacen.Total - (salidaAlmacen.Total / ((100 + salidaAlmacen.PorcentajeIGV) / 100)), 2, MidpointRounding.AwayFromZero);
            subTotal = salidaAlmacen.Total - montoIGV;

            if (salidaAlmacen.MonedaId == "S")
            {
                totalPEN = salidaAlmacen.Total;
                montoIGVPEN = decimal.Round(totalPEN - (totalPEN / ((100 + salidaAlmacen.PorcentajeIGV) / 100)), 2, MidpointRounding.AwayFromZero);
                subTotalPEN = totalPEN - montoIGVPEN;

                totalUSD = decimal.Round(totalPEN / salidaAlmacen.TipoCambio, 2, MidpointRounding.AwayFromZero);
                montoIGVUSD = decimal.Round(totalUSD - (totalUSD / ((100 + salidaAlmacen.PorcentajeIGV) / 100)), 2, MidpointRounding.AwayFromZero);
                subTotalUSD = totalUSD - montoIGVUSD;
            }
            else
            {
                totalUSD = salidaAlmacen.Total;
                montoIGVUSD = decimal.Round(totalUSD - (totalUSD / ((100 + salidaAlmacen.PorcentajeIGV) / 100)), 2, MidpointRounding.AwayFromZero);
                subTotalUSD = totalUSD - montoIGVUSD;

                totalPEN = decimal.Round(totalUSD * salidaAlmacen.TipoCambio, 2, MidpointRounding.AwayFromZero);
                montoIGVPEN = decimal.Round(totalPEN - (totalPEN / ((100 + salidaAlmacen.PorcentajeIGV) / 100)), 2, MidpointRounding.AwayFromZero);
                subTotalPEN = totalPEN - montoIGVPEN;
            }

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    salidaAlmacen.EmpresaId,
                    salidaAlmacen.TipoDocumentoId,
                    salidaAlmacen.Serie,
                    salidaAlmacen.Numero,
                    salidaAlmacen.FechaInicio,
                    salidaAlmacen.FechaTerminacion,
                    salidaAlmacen.ClienteId,
                    salidaAlmacen.ClienteNumeroDocumentoIdentidad,
                    salidaAlmacen.MonedaId,
                    salidaAlmacen.TipoCambio,
                    salidaAlmacen.PersonalId,
                    salidaAlmacen.LineaProduccion,
                    salidaAlmacen.Envasado,
                    salidaAlmacen.NumeroLote,
                    salidaAlmacen.GuiaRemision,
                    salidaAlmacen.Observacion,
                    IncluyeIGV = salidaAlmacen.IncluyeIGV ? "S" : "N",
                    salidaAlmacen.PorcentajeIGV,
                    salidaAlmacen.GastosIndirectos,
                    salidaAlmacen.CantidadSolicitada,
                    salidaAlmacen.CantidadProducida,
                    salidaAlmacen.HoraEmision,
                    salidaAlmacen.UsuarioId,
                    salidaAlmacen.Total,
                    salidaAlmacen.NumeroDocumento,
                    salidaAlmacen.TotalGalones,
                    subTotal,
                    montoIGV,
                    totalPEN,
                    totalUSD,
                    subTotalPEN,
                    subTotalUSD,
                    montoIGVPEN,
                    montoIGVUSD
                });
            }
        }

        public async Task Modificar(oSalidaAlmacen salidaAlmacen)
        {
            string query = @"   UPDATE Venta SET Ven_Venci = @FechaInicio, Ven_Fecha = @FechaTerminacion, Cli_Codigo = @ClienteId, Ven_RucDni = @ClienteNumeroDocumentoIdentidad, 
                                Ven_Moneda = @MonedaId, Ven_TCambio = @TipoCambio, Per_Codigo = @PersonalId, Ven_DireccionPart = @LineaProduccion, Ven_DireccionLleg = @Envasado, 
                                Ven_NroComp = @NumeroLote, Ven_GuiaRemision = @GuiaRemision, Ven_Observ = @Observacion, Ven_IncluyeIgv = @IncluyeIGV, Ven_PorcIgv = @PorcentajeIGV, 
                                Ven_Otros = @GastosIndirectos, Ven_Flat01 = @CantidadSolicitada, Ven_Flat02 = @CantidadProducida, Ven_Total = @Total, Ven_Descuento = @TotalGalones,
                                Ven_Hora = @HoraEmision, Usu_Codigo = @UsuarioId, Ven_Cancelado = 'N', Ven_Abonado = 0, Ven_Saldo = @Total, Ven_FechaMod = GETDATE(),
                                Ven_SubTotal = @subTotal, Ven_ValorVenta = @subTotal, Ven_TotalNeto = @Total, Ven_MontoIgv = @montoIGV, Ven_TotalSol = @totalPEN, Ven_TotalDol = @totalUSD,
                                Ven_SubTotalSol = @subTotalPEN, Ven_ValorVentaSol = @subTotalPEN, Ven_TotalNetoSol = @totalPEN, Ven_MontoIgvSol = @montoIGVPEN,
                                Ven_SubTotalDol = @subTotalUSD, Ven_ValorVentaDol = @subTotalUSD, Ven_TotalNetoDol = @totalUSD, Ven_MontoIgvDol = @montoIGVUSD
                                WHERE Conf_Codigo = @EmpresaId AND TDoc_Codigo = @TipoDocumentoId AND Ven_Serie = @Serie AND Ven_Numero = @Numero";

            decimal subTotal = 0, montoIGV = 0;
            decimal subTotalPEN = 0, montoIGVPEN = 0, totalPEN = 0;
            decimal subTotalUSD = 0, montoIGVUSD = 0, totalUSD = 0;

            montoIGV = decimal.Round(salidaAlmacen.Total - (salidaAlmacen.Total / ((100 + salidaAlmacen.PorcentajeIGV) / 100)), 2, MidpointRounding.AwayFromZero);
            subTotal = salidaAlmacen.Total - montoIGV;

            if (salidaAlmacen.MonedaId == "S")
            {
                totalPEN = salidaAlmacen.Total;
                montoIGVPEN = decimal.Round(totalPEN - (totalPEN / ((100 + salidaAlmacen.PorcentajeIGV) / 100)), 2, MidpointRounding.AwayFromZero);
                subTotalPEN = totalPEN - montoIGVPEN;

                totalUSD = decimal.Round(totalPEN / salidaAlmacen.TipoCambio, 2, MidpointRounding.AwayFromZero);
                montoIGVUSD = decimal.Round(totalUSD - (totalUSD / ((100 + salidaAlmacen.PorcentajeIGV) / 100)), 2, MidpointRounding.AwayFromZero);
                subTotalUSD = totalUSD - montoIGVUSD;
            }
            else
            {
                totalUSD = salidaAlmacen.Total;
                montoIGVUSD = decimal.Round(totalUSD - (totalUSD / ((100 + salidaAlmacen.PorcentajeIGV) / 100)), 2, MidpointRounding.AwayFromZero);
                subTotalUSD = totalUSD - montoIGVUSD;

                totalPEN = decimal.Round(totalUSD * salidaAlmacen.TipoCambio, 2, MidpointRounding.AwayFromZero);
                montoIGVPEN = decimal.Round(totalPEN - (totalPEN / ((100 + salidaAlmacen.PorcentajeIGV) / 100)), 2, MidpointRounding.AwayFromZero);
                subTotalPEN = totalPEN - montoIGVPEN;
            }

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    salidaAlmacen.FechaInicio,
                    salidaAlmacen.FechaTerminacion,
                    salidaAlmacen.ClienteId,
                    salidaAlmacen.ClienteNumeroDocumentoIdentidad,
                    salidaAlmacen.MonedaId,
                    salidaAlmacen.TipoCambio,
                    salidaAlmacen.PersonalId,
                    salidaAlmacen.LineaProduccion,
                    salidaAlmacen.Envasado,
                    salidaAlmacen.NumeroLote,
                    salidaAlmacen.GuiaRemision,
                    salidaAlmacen.Observacion,
                    IncluyeIGV = salidaAlmacen.IncluyeIGV ? "S" : "N",
                    salidaAlmacen.PorcentajeIGV,
                    salidaAlmacen.GastosIndirectos,
                    salidaAlmacen.CantidadSolicitada,
                    salidaAlmacen.CantidadProducida,
                    salidaAlmacen.Total,
                    salidaAlmacen.TotalGalones,
                    salidaAlmacen.HoraEmision,
                    salidaAlmacen.UsuarioId,
                    subTotal,
                    montoIGV,
                    totalPEN,
                    totalUSD,
                    subTotalPEN,
                    subTotalUSD,
                    montoIGVPEN,
                    montoIGVUSD,
                    salidaAlmacen.EmpresaId,
                    salidaAlmacen.TipoDocumentoId,
                    salidaAlmacen.Serie,
                    salidaAlmacen.Numero
                });
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = @"DELETE Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task Anular(string id)
        {
            var splitId = SplitId(id);
            string query = @"   UPDATE Venta SET Ven_Anulado = 'S', Ven_AfectarStock = 'N' 
                                WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oSalidaAlmacen> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"	SELECT
									V.Conf_Codigo AS EmpresaId,
									V.TDoc_Codigo AS TipoDocumentoId,
									V.Ven_Serie AS Serie,
									V.Ven_Numero AS Numero,
									V.Ven_Venci AS FechaInicio,
									V.Ven_Fecha AS FechaTerminacion,
									V.Cli_Codigo AS ClienteId,
									C.Cli_RazonSocial AS ClienteNombre,
									V.Ven_RucDni AS ClienteNumeroDocumentoIdentidad,
									V.Ven_Moneda AS MonedaId,
									V.Ven_TCambio AS TipoCambio,
									V.Per_Codigo AS PersonalId,
									V.Ven_DireccionPart AS LineaProduccion,
									V.Ven_DireccionLleg AS Envasado,
									V.Ven_NroComp AS NumeroLote,
									V.Ven_GuiaRemision AS GuiaRemision,
									V.Ven_Observ AS Observacion,
									CAST(CASE WHEN V.Ven_IncluyeIgv = 'S' THEN 1 ELSE 0 END AS BIT) AS IncluyeIGV,
									V.Ven_PorcIgv AS PorcentajeIGV,
									V.Ven_Otros AS GastosIndirectos,
									V.Ven_Flat01 AS CantidadSolicitada,
									V.Ven_Flat02 AS CantidadProducida,
									V.Ven_Total AS Total,
									V.Ven_Descuento AS TotalGalones
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
                return await db.QueryFirstOrDefaultAsync<oSalidaAlmacen>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<oPagina<vSalidaAlmacen>> Listar(DateTime fechaInicio, DateTime fechaFin, string numeroDocumento, oPaginacion paginacion)
        {
            string query = $@"	SELECT 
									Codigo AS Id,
									Fecha AS FechaEmision,
									Hora AS HoraEmision,
									Documento AS NumeroDocumento,
									Personal AS PersonalNombre,
									Ven_NroComp AS NumeroLote,
                                    Moneda AS MonedaId,
                                    Total AS Total,
									Ven_DireccionPart AS LineaProduccion,
									Ven_GuiaRemision AS GuiaRemision,
									CAST(CASE WHEN Cancelado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsCancelado,
									CAST(CASE WHEN Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado
								FROM 
									v_lst_Venta
								WHERE 
									TipoDoc = '{TipoDocumentoId}'
									AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
									AND Documento LIKE '%' + @numeroDocumento + '%'
								ORDER BY
									Fecha DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vSalidaAlmacen> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    numeroDocumento = new DbString { Value = numeroDocumento, IsAnsi = true, IsFixedLength = false, Length = 24 }
                }))
                {
                    pagina = new oPagina<vSalidaAlmacen>
                    {
                        Data = await result.ReadAsync<vSalidaAlmacen>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<string> GetSerie(string empresaId)
        {
            string query = "SELECT MAX(Ven_Serie) FROM Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId";

            using (var db = GetConnection())
            {
                var serie = await db.QueryFirstOrDefaultAsync<string>(query, new
                {
                    empresaId = new DbString { Value = empresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 }
                });

                return string.IsNullOrWhiteSpace(serie) ? "0001" : serie;
            }
        }

        public async Task<string> GetNuevoNumero(string empresaId, string serie)
        {
            return await GetNuevoId("SELECT MAX(Ven_Numero) FROM Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND ISNUMERIC(Ven_Numero) = 1", new
            {
                empresaId = new DbString { Value = empresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                tipoDocumentoId = new DbString { Value = TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                serie = new DbString { Value = serie, IsAnsi = true, IsFixedLength = true, Length = 4 }
            }, "000000000#");
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

        public async Task<(bool IsBloqueado, string Mensaje)> IsBloqueado(string id)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = @"   SELECT
                                        CAST(CASE WHEN Ven_Cancelado = 'S' THEN 1 ELSE 0 END AS BIT),
                                        CAST(CASE WHEN Ven_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT),
                                        CAST(CASE WHEN Ven_Anulado = 'S' THEN 1 ELSE 0 END AS BIT)
                                    FROM 
                                        Venta
                                    WHERE 
                                        Conf_Codigo = @empresaId 
                                        AND TDoc_Codigo = @tipoDocumentoId 
                                        AND Ven_Serie = @serie 
                                        AND Ven_Numero = @numero";

                var (isCerrado, isBloqueado, isAnulado) = await db.QueryFirstAsync<(bool IsCerrado, bool IsBloqueado, bool IsAnulado)>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });

                if (isCerrado)
                {
                    return (true, "El registro está cerrado.");
                }
                else if (isBloqueado)
                {
                    return (true, "El registro está bloqueado.");
                }
                else if (isAnulado)
                {
                    return (true, "El registro está anulado.");
                }

                return (false, string.Empty);
            }
        }

        public async Task ActualizarEstadoCancelado(string id, bool isCancelado)
        {
            var splitId = SplitId(id);

            string query = @"   UPDATE Venta SET Ven_Cancelado = @isCancelado WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    isCancelado = new DbString { Value = isCancelado ? "S" : "N", IsAnsi = true, IsFixedLength = true, Length = 1 },
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public static oSplitDocumentoVentaId SplitId(string id) => new(id);
        #endregion
    }
}
