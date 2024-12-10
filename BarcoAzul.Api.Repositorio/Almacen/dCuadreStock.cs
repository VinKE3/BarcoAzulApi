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
    public class dCuadreStock : dComun
    {
        public const string TipoDocumentoId = "CU";

        public dCuadreStock(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oCuadreStock cuadreStock)
        {
            string query = @"   INSERT INTO Venta ( 
				                Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Suc_Codigo, Ven_AfectarStock, Ven_IngEgrStock, Ven_TVenta, Ven_TPago, Ven_Fecha, 
				                Ven_Hora, Ven_Venci, Cli_Codigo, Ven_RucDni, Ven_Telefono, Per_Codigo, Ven_Moneda, Ven_TCambio, Ven_GuiaRemision, 
				                Ven_PorcIgv, Ven_SubTotal, Ven_PorcDscto, Ven_Descuento, Ven_ValorVenta, Ven_TotalNeto, Ven_MontoIgv, Ven_Otros, Ven_PorcReten, Ven_Retencion, 
				                Ven_PorcPercep, Ven_Percepcion, Ven_Inafecto, Ven_Total, Ven_Abonado, Ven_Saldo, Ven_Anulado, Ven_Cancelado, Ven_ConIgv, Ven_IncluyeIgv, 
				                Ven_Retenc, Ven_Percep, Ven_CtaCtble, Ven_CtaCbleNom, Ven_CCosto, 
				                Ven_CCostoNom, Tra_Codigo, Ven_TTraslado, Ven_Observ, Ven_FechaReg,
				                Usu_Codigo, Ven_BloqUsu, Ven_BloqSist, Ven_BloqProc, Ven_AboItem,
				                Ven_Abonar, Ven_NroComp, Ven_TipoComp, Ven_CertifInscrip, Ven_PlacaRodaje,
				                Ven_DireccionPart, Ven_DireccionLleg, Ven_Bloqueado, Ven_Documento, Ven_Turno
				                ) VALUES ( 
				                @EmpresaId, @TipoDocumentoId, @Serie, @Numero,'01', 'N', '-', 'CO', 'EF', @FechaRegistro, 
				                @HoraRegistro, @FechaRegistro, @ClienteId, '', '', @ResponsableId, @MonedaId, @TipoCambio, '', 
				                0, NULL, NULL, 0, NULL, NULL, NULL, 0, 0, NULL,
				                0, NULL, 0, @TotalSobra, @TotalFalta, @SaldoTotal, 'N', 'S', 'S', 'S', 
				                'N', 'N', '', '', '',
				                '', NULL, '', @Observacion, GETDATE(), 
				                @UsuarioId, 'N', 'N', 'N', NULL, 
				                NULL, '', '', '', '', 
				                '', '', 'N', @NumeroDocumento, NULL)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    cuadreStock.EmpresaId,
                    cuadreStock.TipoDocumentoId,
                    cuadreStock.Serie,
                    cuadreStock.Numero,
                    cuadreStock.FechaRegistro,
                    cuadreStock.HoraRegistro,
                    cuadreStock.ClienteId,
                    cuadreStock.ResponsableId,
                    cuadreStock.MonedaId,
                    cuadreStock.TipoCambio,
                    cuadreStock.TotalSobra,
                    cuadreStock.TotalFalta,
                    cuadreStock.SaldoTotal,
                    cuadreStock.Observacion,
                    cuadreStock.UsuarioId,
                    cuadreStock.NumeroDocumento
                });
            }
        }

        public async Task Modificar(oCuadreStock cuadreStock)
        {
            string query = @"   UPDATE Venta SET Ven_Fecha = @FechaRegistro, Ven_Hora = @HoraRegistro, Ven_Venci = @FechaRegistro, Ven_Moneda = @MonedaId, Ven_TCambio = @TipoCambio, 
                                Per_Codigo = @ResponsableId, Ven_Observ = @Observacion, Ven_FechaMod = GETDATE(), Ven_Total = @TotalSobra, Ven_Abonado = @TotalFalta, Ven_Saldo = @SaldoTotal,
                                Usu_Codigo = @UsuarioId WHERE Conf_Codigo = @EmpresaId AND TDoc_Codigo = @TipoDocumentoId AND Ven_Serie = @Serie AND Ven_Numero = @Numero";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    cuadreStock.FechaRegistro,
                    cuadreStock.HoraRegistro,
                    FechaVencimiento = cuadreStock.FechaRegistro,
                    cuadreStock.MonedaId,
                    cuadreStock.TipoCambio,
                    cuadreStock.ResponsableId,
                    cuadreStock.Observacion,
                    cuadreStock.TotalSobra,
                    cuadreStock.TotalFalta,
                    cuadreStock.SaldoTotal,
                    cuadreStock.UsuarioId,
                    cuadreStock.EmpresaId,
                    cuadreStock.TipoDocumentoId,
                    cuadreStock.Serie,
                    cuadreStock.Numero
                });
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = @"DELETE Abono_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });

                query = @"DELETE Detalle_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });

                query = @"DELETE Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task AbrirCerrar(string id, bool estado)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                await db.ExecuteAsync("Sp_CuadreStock", new
                {
                    empresa = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    TipoDoc = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    Serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    Numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    Estado = new DbString { Value = estado ? "S" : "N", IsAnsi = true, IsFixedLength = true, Length = 1 }
                }, commandType: System.Data.CommandType.StoredProcedure);
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oCuadreStock> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT 
	                                Conf_Codigo AS EmpresaId,
	                                TDoc_Codigo AS TipoDocumentoId,
	                                Ven_Serie AS Serie,
	                                Ven_Numero AS Numero,
	                                Ven_Fecha AS FechaRegistro, 
	                                Ven_Moneda AS MonedaId, 
	                                Ven_TCambio AS TipoCambio, 
	                                Per_Codigo AS ResponsableId, 
	                                Ven_AfectarStock AS Estado,
	                                Ven_Observ AS Observacion,
	                                Ven_Total AS TotalSobra, 
	                                Ven_Abonado AS TotalFalta, 
	                                Ven_Saldo AS SaldoTotal
                                FROM 
                                    Venta 
                                WHERE 
                                    Conf_Codigo = @empresaId 
                                    AND TDoc_Codigo = @tipoDocumentoId 
                                    AND Ven_Serie = @serie 
                                    AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oCuadreStock>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<oPagina<vCuadreStock>> Listar(DateTime fechaInicio, DateTime fechaFin, oPaginacion paginacion)
        {
            string query = $@"	SELECT  
									codigo AS Id,
									CAST(CASE WHEN Estado = 'S' THEN 1 ELSE 0 END AS BIT) AS Estado,
									CAST(CASE WHEN Pendiente = 'S' THEN 1 ELSE 0 END AS BIT) AS Pendiente,
									Fecha AS FechaRegistro,
									Numero,
									Responsable AS ResponsableNombreCompleto,
									Moneda AS MonedaId,
									Total_Sobra AS TotalSobra,
									Total_Falta AS TotalFalta,
									Saldo AS SaldoTotal
								FROM 
									v_lst_cuadrestock
								WHERE 
									TDoc_Codigo = '{TipoDocumentoId}'
									AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
								ORDER BY
									Numero
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vCuadreStock> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { fechaInicio, fechaFin }))
                {
                    pagina = new oPagina<vCuadreStock>
                    {
                        Data = await result.ReadAsync<vCuadreStock>(),
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

        public async Task<IEnumerable<oArticuloParaCuadreStock>> GetArticulosParaCuadreStock()
        {
            string query = @"   SELECT
                                    Lin_Codigo AS LineaId,
                                    SubL_Codigo AS SubLineaId,
                                    Art_Codigo AS ArticuloId,
                                    Mar_Codigo AS MarcaId,
                                    Uni_Codigo AS UnidadMedidaId,
                                    Art_CodBarra AS CodigoBarras,
                                    Art_Descripcion AS Descripcion,
                                    Lin_Nombre AS LineaDescripcion,
                                    SubL_Nombre AS SubLineaDescripcion,
                                    Mar_Nombre AS MarcaNombre,
                                    Uni_Nombre AS UnidadMedidaDescripcion,
                                    Art_Stock01 AS Stock,
                                    Art_PCompra AS PrecioCompra,
                                    TipE_Codigo AS TipoExistenciaId
                                FROM
	                                v_lst_articulocuadrestock
                                WHERE 
	                                Art_CtrlStock = 'S'";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oArticuloParaCuadreStock>(query);
            }
        }

        public async Task RecalcularStock(oRecalcularStock recalcularStock)
        {
            using (var db = GetConnection())
            {
                foreach (var articulo in recalcularStock.Articulos)
                {
                    var parametros = new DynamicParameters();
                    parametros.Add("@Empresa", recalcularStock.EmpresaId, System.Data.DbType.AnsiStringFixedLength, size: 2);
                    parametros.Add("@Linea", articulo.LineaId, System.Data.DbType.AnsiStringFixedLength, size: 2);
                    parametros.Add("@SubLin", articulo.SubLineaId, System.Data.DbType.AnsiStringFixedLength, size: 2);
                    parametros.Add("@ArtCod", articulo.ArticuloId, System.Data.DbType.AnsiStringFixedLength, size: 4);
                    parametros.Add("@Final", recalcularStock.Fecha, System.Data.DbType.DateTime);
                    parametros.Add("@StockFinal", dbType: System.Data.DbType.Decimal, direction: System.Data.ParameterDirection.Output, precision: 12, scale: 4);

                    await db.QueryAsync("SP_RecalStock", parametros, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 60);

                    articulo.Stock = parametros.Get<decimal>("@StockFinal");
                }
            }
        }

        public async Task<bool> IsBloqueado(string id)
        {
            var splitId = SplitId(id);
            string query = @"   SELECT CAST(CASE WHEN Ven_AfectarStock = 'S' THEN 1 ELSE 0 END AS BIT) FROM Venta 
                                WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<bool>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<string> GetNuevoNumero(string empresaId, string serie)
        {
            return await GetNuevoId("SELECT MAX(Ven_Numero) FROM Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie", new
            {
                empresaId = new DbString { Value = empresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                tipoDocumentoId = new DbString { Value = TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                serie = new DbString { Value = serie, IsAnsi = true, IsFixedLength = true, Length = 4 }
            }, "000000000#");
        }

        public async Task<DateTime?> GetFechaUltimoCuadre()
        {
            string query = $"SELECT TOP 1 Ven_Fecha FROM Venta WHERE TDoc_Codigo = '{TipoDocumentoId}' AND Ven_AfectarStock = 'S' ORDER BY Ven_Fecha DESC";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<DateTime?>(query);
            }
        }

        public static oSplitDocumentoVentaId SplitId(string id) => new(id);
        #endregion
    }
}
