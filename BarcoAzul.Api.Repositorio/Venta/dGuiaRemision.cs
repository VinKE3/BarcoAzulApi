using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;
using MCWebAPI.Modelos.Otros;
using System.Data;

namespace BarcoAzul.Api.Repositorio.Venta
{
    public class dGuiaRemision : dComun
    {
        public const string TipoDocumentoId = "09";

        public dGuiaRemision(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oGuiaRemision guiaRemision)
        {
            string query = @"	INSERT INTO Venta (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Ven_Fecha, Ven_Venci, Cli_Codigo, Ven_RucDni, Ven_RazonSocial, 
								Per_Codigo, Ven_partViaNom, Id_DireccionLlegada, Ven_llegViaNom, Tran_Codigo, Ven_OtrosSol, Tra_Codigo, Ven_LicenciaCond, Ven_PlacaRodaje, 
								Ven_CertifInscrip, TipO_Codigo, Ven_GuiaRemision, Ven_IngEgrStock, ven_ordencompra, Ven_NroComp, Ven_Observ, Ven_Moneda, Ven_AfectarStock,
								Suc_Codigo, Ven_TVenta, Ven_TPago, Ven_Hora, Ven_TCambio, Ven_PorcIgv, Ven_SubTotal, Ven_PorcDscto, Ven_Descuento, Ven_ValorVenta, Ven_TotalNeto,
								Ven_MontoIgv, Ven_Otros, Ven_PorcReten, Ven_Retencion, Ven_PorcPercep, Ven_Percepcion, Ven_Inafecto, Ven_Total, Ven_Abonado, Ven_Saldo, Ven_Anulado,
								Ven_Cancelado, Ven_ConIgv, Ven_IncluyeIgv, Ven_Retenc, Ven_Percep, Ven_TipoComp, Ven_TTraslado, Ven_FechaReg, Usu_Codigo, Ven_BloqUsu, Ven_BloqSist,
								Ven_BloqProc, Ven_Bloqueado, Ven_Facturado, Ven_DocFactur, Ctrl_Cilindros, Ven_Transaccion, Ven_Costo, Ven_Anticipo)
								VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @FechaEmision, @FechaTraslado, @ClienteId, @ClienteNumeroDocumentoIdentidad, @ClienteNombre, 
								@PersonalId, @DireccionPartida, @ClienteDireccionId, @ClienteDireccion, @EmpresaTransporteId, @CostoMinimo, @ConductorId, @LicenciaConducir, @VehiculoId, 
								@ConstanciaInscripcion, @MotivoTrasladoId, @MotivoSustento, @IngresoEgresoStock, @NumeroFactura, @OrdenPedido, @Observacion, @MonedaId, @AfectarStock,
								'01', 'CO', 'EF', @HoraEmision, @TipoCambio, @PorcentajeIGV, @subTotal, 0, 0, @subTotal, @subTotal,
								@montoIGV, 0, 0, 0, 0, 0, 0, @total, 0, @total, 'N',
								'N', 'S', 'S', 'N', 'N', @DocumentoRelacionadoId, '01', GETDATE(), @UsuarioId, 'N', 'N',
								'N', 'S', 'N', @DocumentoRelacionadoId, 'N', 'N', @CostoTotal, 'N')";

            decimal total = guiaRemision.Detalles.Sum(x => x.Importe);
            decimal subTotal = decimal.Round(total / ((guiaRemision.PorcentajeIGV / 100) + 1), 2, MidpointRounding.AwayFromZero);
            decimal montoIGV = total - subTotal;

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    guiaRemision.EmpresaId,
                    guiaRemision.TipoDocumentoId,
                    guiaRemision.Serie,
                    guiaRemision.Numero,
                    guiaRemision.FechaEmision,
                    guiaRemision.FechaTraslado,
                    guiaRemision.ClienteId,
                    guiaRemision.ClienteNumeroDocumentoIdentidad,
                    guiaRemision.ClienteNombre,
                    guiaRemision.PersonalId,
                    guiaRemision.DireccionPartida,
                    guiaRemision.ClienteDireccionId,
                    guiaRemision.ClienteDireccion,
                    guiaRemision.EmpresaTransporteId,
                    guiaRemision.CostoMinimo,
                    guiaRemision.ConductorId,
                    guiaRemision.LicenciaConducir,
                    guiaRemision.VehiculoId,
                    guiaRemision.ConstanciaInscripcion,
                    guiaRemision.MotivoTrasladoId,
                    guiaRemision.MotivoSustento,
                    guiaRemision.IngresoEgresoStock,
                    guiaRemision.NumeroFactura,
                    guiaRemision.OrdenPedido,
                    guiaRemision.Observacion,
                    guiaRemision.MonedaId,
                    AfectarStock = guiaRemision.AfectarStock ? "S" : "N",
                    guiaRemision.HoraEmision,
                    guiaRemision.TipoCambio,
                    guiaRemision.PorcentajeIGV,
                    subTotal,
                    montoIGV,
                    total,
                    guiaRemision.DocumentoRelacionadoId,
                    guiaRemision.UsuarioId,
                    guiaRemision.CostoTotal
                });
            }
        }

        public async Task Modificar(oGuiaRemision guiaRemision)
        {
            string query = @"   UPDATE Venta SET Ven_Fecha = @FechaEmision, Ven_Venci = @FechaTraslado, Cli_Codigo = @ClienteId, Ven_RucDni = @ClienteNumeroDocumentoIdentidad, 
                                Ven_RazonSocial = @ClienteNombre, Per_Codigo = @PersonalId, Ven_partViaNom = @DireccionPartida, Id_DireccionLlegada = @ClienteDireccionId, 
                                Ven_llegViaNom = @ClienteDireccion, Tran_Codigo = @EmpresaTransporteId, Ven_OtrosSol = @CostoMinimo, Tra_Codigo = @ConductorId, 
                                Ven_LicenciaCond = @LicenciaConducir, Ven_PlacaRodaje = @VehiculoId, Ven_CertifInscrip = @ConstanciaInscripcion, TipO_Codigo = @MotivoTrasladoId, 
                                Ven_GuiaRemision = @MotivoSustento, Ven_IngEgrStock = @IngresoEgresoStock, ven_ordencompra = @NumeroFactura, Ven_NroComp = @OrdenPedido, 
                                Ven_Observ = @Observacion, Ven_Moneda = @MonedaId, Ven_AfectarStock = @AfectarStock, Ven_TipoComp = @DocumentoRelacionadoId, Ven_Hora = @HoraEmision,
                                Ven_TCambio = @TipoCambio, Ven_PorcIgv = @PorcentajeIGV, Ven_SubTotal = @subTotal, Ven_ValorVenta = @subTotal, Ven_TotalNeto = @subTotal,
                                Ven_MontoIgv = @montoIGV, Ven_Total = @total, Ven_Saldo = @total, Ven_FechaMod = GETDATE(), Usu_Codigo = @UsuarioId, Ven_DocFactur = @DocumentoRelacionadoId,
                                Ven_Costo = @CostoTotal, Ven_Cancelado = 'N', Ven_Abonado = 0, Ven_Facturado = 'N', Ven_Bloqueado = 'S'
                                WHERE Conf_Codigo = @EmpresaId AND TDoc_Codigo = @TipoDocumentoId AND Ven_Serie = @Serie AND Ven_Numero = @Numero";

            decimal total = guiaRemision.Detalles.Sum(x => x.Importe);
            decimal subTotal = decimal.Round(total / ((guiaRemision.PorcentajeIGV / 100) + 1), 2, MidpointRounding.AwayFromZero);
            decimal montoIGV = total - subTotal;

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    guiaRemision.FechaEmision,
                    guiaRemision.FechaTraslado,
                    guiaRemision.ClienteId,
                    guiaRemision.ClienteNumeroDocumentoIdentidad,
                    guiaRemision.ClienteNombre,
                    guiaRemision.PersonalId,
                    guiaRemision.DireccionPartida,
                    guiaRemision.ClienteDireccionId,
                    guiaRemision.ClienteDireccion,
                    guiaRemision.EmpresaTransporteId,
                    guiaRemision.CostoMinimo,
                    guiaRemision.ConductorId,
                    guiaRemision.LicenciaConducir,
                    guiaRemision.VehiculoId,
                    guiaRemision.ConstanciaInscripcion,
                    guiaRemision.MotivoTrasladoId,
                    guiaRemision.MotivoSustento,
                    guiaRemision.IngresoEgresoStock,
                    guiaRemision.NumeroFactura,
                    guiaRemision.OrdenPedido,
                    guiaRemision.Observacion,
                    guiaRemision.MonedaId,
                    AfectarStock = guiaRemision.AfectarStock ? "S" : "N",
                    guiaRemision.HoraEmision,
                    guiaRemision.TipoCambio,
                    guiaRemision.PorcentajeIGV,
                    subTotal,
                    montoIGV,
                    total,
                    guiaRemision.DocumentoRelacionadoId,
                    guiaRemision.UsuarioId,
                    guiaRemision.CostoTotal,
                    guiaRemision.EmpresaId,
                    guiaRemision.TipoDocumentoId,
                    guiaRemision.Serie,
                    guiaRemision.Numero
                });
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = "DELETE Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

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

            string query = @"   UPDATE Venta SET Ven_Anulado = 'S', Ven_AfectarStock = 'N', Ven_DocFactur = '', Ven_OrdenCompra = '', Ven_Bloqueado = 'S'
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
        public async Task<oGuiaRemision> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"	SELECT
									Conf_Codigo AS EmpresaId,
									TDoc_Codigo AS TipoDocumentoId,
									Ven_Serie AS Serie,
									Ven_Numero AS Numero,
									Ven_Fecha AS FechaEmision,
									Ven_Venci AS FechaTraslado,
									Cli_Codigo AS ClienteId,
									Ven_RucDni AS ClienteNumeroDocumentoIdentidad,
									Ven_RazonSocial AS ClienteNombre,
									Per_Codigo AS PersonalId,
									Ven_partViaNom AS DireccionPartida,
									Id_DireccionLlegada AS ClienteDireccionId,
									Ven_llegViaNom AS ClienteDireccion,
									Tran_Codigo AS EmpresaTransporteId,
									Ven_OtrosSol AS CostoMinimo,
									Tra_Codigo AS ConductorId,
									Ven_LicenciaCond AS LicenciaConducir,
									Ven_PlacaRodaje AS VehiculoId,
									Ven_CertifInscrip AS ConstanciaInscripcion,
									TipO_Codigo AS MotivoTrasladoId,
									Ven_GuiaRemision AS MotivoSustento,
									Ven_IngEgrStock AS IngresoEgresoStock,
									ven_ordencompra AS NumeroFactura,
									Ven_NroComp AS OrdenPedido,
									Ven_Observ AS Observacion,
									Ven_Moneda AS MonedaId,
									CAST(CASE WHEN Ven_AfectarStock = 'S' THEN 1 ELSE 0 END AS BIT) AS AfectarStock,
									Ven_TipoComp AS DocumentoRelacionadoId
								FROM
									Venta
								WHERE
									Conf_Codigo = @empresaId
									AND TDoc_Codigo = @tipoDocumentoId
									AND Ven_Serie = @serie
									AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oGuiaRemision>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<oPagina<vGuiaRemision>> Listar(string[] tiposSeries, DateTime fechaInicio, DateTime fechaFin, string clienteNombre, string personalId, oPaginacion paginacion)
        {
            string query = $@"	SELECT 
									Codigo AS Id,
									Fecha AS FechaEmision,
									Serie + '-' + RIGHT(Numero, 8) AS NumeroDocumento,
									Razon_Social AS ClienteNombre,
									Ruc_Dni AS ClienteNumero,
									Marca_Placa AS Placa,
                                    Serie AS Serie,
									CAST(CASE WHEN AfectarStock = 'S' THEN 1 ELSE 0 END AS BIT) AS AfectarStock,
									CAST(CASE WHEN Anulado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsAnulado,
									CAST(CASE WHEN Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado
								FROM 
									v_lst_Venta
								WHERE 
									TipoDoc = '{TipoDocumentoId}'
                                    AND Serie IN ({JoinToQuery(tiposSeries)})
									AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
									AND Razon_Social LIKE '%' + @clienteNombre + '%'
									{(string.IsNullOrWhiteSpace(personalId) ? string.Empty : "AND Per_Codigo = @personalId")}
								ORDER BY
									Fecha DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vGuiaRemision> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    clienteNombre = new DbString { Value = clienteNombre, IsAnsi = true, IsFixedLength = false, Length = 250 },
                    personalId = new DbString { Value = personalId, IsAnsi = true, IsFixedLength = true, Length = 8 }
                }))
                {
                    pagina = new oPagina<vGuiaRemision>
                    {
                        Data = await result.ReadAsync<vGuiaRemision>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<string> GetNuevoNumero(string empresaId, string serie)
        {
            using (var db = GetConnection())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@empresa", empresaId, dbType: DbType.AnsiString, size: 2);
                parametros.Add("@sucursal", "01", dbType: DbType.AnsiString, size: 2);
                parametros.Add("@tdoc", TipoDocumentoId, dbType: DbType.AnsiString, size: 2);
                parametros.Add("@serie", serie, dbType: DbType.AnsiString, size: 4);
                parametros.Add("@numero", dbType: DbType.AnsiString, size: 10, direction: ParameterDirection.Output);

                await db.ExecuteAsync("sp_correlativoventa", parametros, commandType: CommandType.StoredProcedure);

                return parametros.Get<string>("@numero");
            }
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

        public async Task<bool> IsBloqueado(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT CAST(CASE WHEN Ven_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) FROM Venta
                                WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryFirstAsync<bool>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<bool> IsAnulado(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT CAST(CASE WHEN Ven_Anulado = 'S' THEN 1 ELSE 0 END AS BIT) FROM Venta
                                WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryFirstAsync<bool>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task ActualizarCantidadPendiente(string id, Operacion operacion)
        {
            using (var db = GetConnection())
            {
                await db.ExecuteAsync("Sp_UpdateCantPendVenta", new
                {
                    Codigo = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 18 },
                    Tipo = new DbString { Value = operacion == Operacion.Aumentar ? "+" : "-", IsAnsi = true, IsFixedLength = false, Length = 1 }
                }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            }
        }

        public async Task ActualizarEstadoDocumentoRelacionado(string id)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = "SELECT Ven_DocFactur FROM Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

                var documentoRelacionadoId = await db.QueryFirstOrDefaultAsync<string>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });

                if (!string.IsNullOrWhiteSpace(documentoRelacionadoId))
                {
                    splitId = SplitId(documentoRelacionadoId);

                    query = @"  UPDATE Venta SET Ven_Facturado = 'N', Ven_guiaRemision = '' 
                                WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

                    await db.ExecuteAsync(query, new
                    {
                        empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                    });
                }
            }
        }

        public static oSplitDocumentoVentaId SplitId(string id) => new(id);
        #endregion
    }
}
