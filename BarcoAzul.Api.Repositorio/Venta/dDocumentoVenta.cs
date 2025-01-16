using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Utilidades;
using Dapper;
using MCWebAPI.Modelos.Otros;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Repositorio.Venta
{
    public class dDocumentoVenta : dComun
    {
        public dDocumentoVenta(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oDocumentoVenta documentoVenta)
        {
            string query = @"	INSERT INTO Venta (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Ven_Fecha, Ven_Venci, Ven_NroComp, Cli_Codigo, Ven_RucDni, 
								Id_DireccionLlegada, Ven_DireccionLleg, Per_Codigo, Ven_Condicion, Ven_Moneda, Ven_TCambio, Ven_TVenta, Ven_TPago, Ven_TipoComp, Ven_Telefono, 
								Ven_CertifInscrip, Ven_Abonar, Ven_PlacaRodaje, Mot_Codigo, Ven_Sustento, Ven_GuiaRemision, Ven_OrdenCompra, Ven_Observ, Ven_Anticipo, Ven_OpeGratuitas, 
								Ven_IncluyeIgv, Ven_AfectarStock, Ven_Inafecto, Ven_TotalOpeGratuitas, Ven_SubTotal, Ven_ValorVenta, Ven_TotalAnticipo, Ven_TotalNeto, Ven_MontoIgv, 
                                Ven_Retencion, Ven_Detraccion, Ven_MontoImpBolsa, Ven_Total, Ven_PorcIgv, Ven_PorcReten, Ven_PorcDetrac, Ven_ImpBolsa, Suc_Codigo, 
                                Ven_IngEgrStock, Ven_Hora, Ven_PorcDscto, Ven_Descuento, Ven_Otros, Ven_PorcPercep, Ven_Percepcion, Ven_Abonado, Ven_Saldo, Ven_Anulado, Ven_Cancelado,
                                Ven_ConIgv, Ven_Retenc, Ven_Percep, Ven_LicenciaCond, Ven_FechaReg, Usu_Codigo, Ven_BloqUsu, Ven_BloqSist, Ven_BloqProc, Ven_Bloqueado, Ven_Documento,
                                Ven_CierreZ, Ven_CierreX, Ven_Facturado, Ven_DocFactur, Ven_Guia, Ven_FechaRecepcion, TipO_Codigo, Ven_AboItem, Ven_EditCliente, Ven_Registrado,
                                Ven_PorcComision, Ven_MontoComision, Cod_Vendedor, Ctrl_Cilindros, Ven_CantCilindros, Ven_Referencia, Cli_TipoDoc, Ven_NetoMercaderia, Ven_NetoBolsa,
                                Ven_Costo, Ven_Utilidad, Ven_NroCuotas, Ven_FechaEmision)
								VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @FechaEmision, @FechaVencimiento, @Cotizacion, @ClienteId, @ClienteNumeroDocumentoIdentidad, 
								@ClienteDireccionId, @ClienteDireccion, @PersonalId, @Letra, @MonedaId, @TipoCambio, @TipoVentaId, @TipoCobroId, @NumeroOperacion, @CuentaCorrienteId, 
								@DocumentoReferenciaId, @Abonar, @MotivoNotaDescripcion, @MotivoNotaId, @MotivoSustento, @GuiaRemision, @NumeroPedido, @Observacion, @IsAnticipo, @IsOperacionGratuita, 
								@IncluyeIGV, @AfectarStock, @TotalOperacionesInafectas, @TotalOperacionesGratuitas, @SubTotal, @SubTotal, @TotalAnticipos, @TotalNeto, @MontoIGV, 
                                @MontoRetencion, @MontoDetraccion, @MontoImpuestoBolsa, @Total, @PorcentajeIGV, @PorcentajeRetencion, @PorcentajeDetraccion, @FactorImpuestoBolsa, '01', 
                                @IngresoEgresoStock, @HoraEmision, 0, 0, 0, 0, 0, 0, @Total, 'N', 'N',
                                'S', 'N', 'N', @DocumentoReferenciaTipo, GETDATE(), @UsuarioId, 'N', 'N', 'N', 'N', @NumeroDocumento,
                                'N', 'N', 'N', @CotizacionId, 'N', @FechaEmision, '01', 0, 'N', 'S',
                                0, 0, '00', @ControlarCilindros, 0, @LetraId, @ClienteTipoDocumentoIdentidadId, @NetoMercaderia, @NetoBolsa,
                                @CostoTotal, @UtilidadTotal, @NumeroCuotas, @FechaDocumentoReferencia)";

            var splitDocumentoReferenciaId = string.IsNullOrWhiteSpace(documentoVenta.DocumentoReferenciaId) ? null : SplitId(documentoVenta.DocumentoReferenciaId);

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    documentoVenta.EmpresaId,
                    documentoVenta.TipoDocumentoId,
                    documentoVenta.Serie,
                    documentoVenta.Numero,
                    documentoVenta.FechaEmision,
                    documentoVenta.FechaVencimiento,
                    documentoVenta.Cotizacion,
                    documentoVenta.ClienteId,
                    documentoVenta.ClienteNumeroDocumentoIdentidad,
                    documentoVenta.ClienteDireccionId,
                    documentoVenta.ClienteDireccion,
                    documentoVenta.PersonalId,
                    documentoVenta.Letra,
                    documentoVenta.MonedaId,
                    documentoVenta.TipoCambio,
                    documentoVenta.TipoVentaId,
                    documentoVenta.TipoCobroId,
                    documentoVenta.NumeroOperacion,
                    documentoVenta.CuentaCorrienteId,
                    documentoVenta.DocumentoReferenciaId,
                    Abonar = documentoVenta.Abonar ? "S" : "N",
                    documentoVenta.MotivoNotaId,
                    documentoVenta.MotivoNotaDescripcion,
                    documentoVenta.MotivoSustento,
                    documentoVenta.GuiaRemision,
                    documentoVenta.NumeroPedido,
                    documentoVenta.Observacion,
                    IsAnticipo = documentoVenta.IsAnticipo ? "S" : "N",
                    IsOperacionGratuita = documentoVenta.IsOperacionGratuita ? "S" : "N",
                    IncluyeIGV = documentoVenta.IncluyeIGV ? "S" : "N",
                    AfectarStock = documentoVenta.AfectarStock ? "S" : "N",
                    documentoVenta.TotalOperacionesInafectas,
                    documentoVenta.TotalOperacionesGratuitas,
                    documentoVenta.SubTotal,
                    documentoVenta.TotalAnticipos,
                    documentoVenta.TotalNeto,
                    documentoVenta.MontoIGV,
                    documentoVenta.MontoRetencion,
                    documentoVenta.MontoDetraccion,
                    documentoVenta.MontoImpuestoBolsa,
                    documentoVenta.Total,
                    documentoVenta.PorcentajeIGV,
                    documentoVenta.PorcentajeRetencion,
                    documentoVenta.PorcentajeDetraccion,
                    documentoVenta.FactorImpuestoBolsa,
                    documentoVenta.IngresoEgresoStock,
                    documentoVenta.HoraEmision,
                    DocumentoReferenciaTipo = splitDocumentoReferenciaId is null ? null : Comun.GetTipoDocumentoAbreviatura(splitDocumentoReferenciaId.TipoDocumentoId),
                    documentoVenta.UsuarioId,
                    documentoVenta.NumeroDocumento,
                    documentoVenta.CotizacionId,
                    ControlarCilindros = documentoVenta.ControlarCilindros ? "S" : "N",
                    documentoVenta.LetraId,
                    documentoVenta.ClienteTipoDocumentoIdentidadId,
                    documentoVenta.NetoMercaderia,
                    documentoVenta.NetoBolsa,
                    documentoVenta.CostoTotal,
                    documentoVenta.UtilidadTotal,
                    documentoVenta.NumeroCuotas,
                    documentoVenta.FechaDocumentoReferencia
                });
            }
        }

        public async Task Modificar(oDocumentoVenta documentoVenta)
        {
            var splitId = SplitId(documentoVenta.Id);

            using (var db = GetConnection())
            {
                string query = @"SELECT Ven_Abonado FROM Venta WHERE Conf_Codigo = @EmpresaId AND TDoc_Codigo = @TipoDocumentoId AND Ven_Serie = @Serie AND Ven_Numero = @Numero";

                var montoAbonado = await db.QueryFirstAsync<decimal>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });

                var saldo = documentoVenta.Total - montoAbonado;

                query = @"  UPDATE Venta SET Ven_Fecha = @FechaEmision, Ven_Venci = @FechaVencimiento, Cli_Codigo = @ClienteId, Cli_TipoDoc = @ClienteTipoDocumentoIdentidadId, 
                            Ven_RucDni = @ClienteNumeroDocumentoIdentidad, Id_DireccionLlegada = @ClienteDireccionId, Ven_DireccionLleg = @ClienteDireccion, Per_Codigo = @PersonalId, 
                            Ven_Condicion = @Letra, Ven_Referencia = @LetraId, Ven_TCambio = @TipoCambio, Ven_TVenta = @TipoVentaId, Ven_TPago = @TipoCobroId, Ven_TipoComp = @NumeroOperacion, 
                            Ven_Telefono = @CuentaCorrienteId, Ven_CertifInscrip = @DocumentoReferenciaId, Ven_Abonar = @Abonar, Ven_PlacaRodaje = @MotivoNotaDescripcion, 
                            Mot_Codigo = @MotivoNotaId, Ven_ValorVenta = @SubTotal, Ven_Sustento = @MotivoSustento, Ven_GuiaRemision = @GuiaRemision, Ven_OrdenCompra = @NumeroPedido, 
                            Ven_Observ = @Observacion, Ven_Anticipo = @IsAnticipo, Ven_OpeGratuitas = @IsOperacionGratuita, Ven_IncluyeIgv = @IncluyeIGV, Ven_AfectarStock = @AfectarStock, 
                            Ven_Inafecto = @TotalOperacionesInafectas, Ven_TotalOpeGratuitas = @TotalOperacionesGratuitas, Ven_SubTotal = @Subtotal, Ven_TotalAnticipo = @TotalAnticipos, 
                            Ven_TotalNeto = @TotalNeto, Ven_MontoIgv = @MontoIGV, Ven_Retencion = @MontoRetencion, Ven_Detraccion = @MontoDetraccion, Ven_MontoImpBolsa = @MontoImpuestoBolsa, 
                            Ven_Total = @Total, Ven_PorcIgv = @PorcentajeIGV, Ven_PorcReten = @PorcentajeRetencion, Ven_PorcDetrac = @PorcentajeDetraccion, Ven_ImpBolsa = @FactorImpuestoBolsa,
                            Ven_IngEgrStock = @IngresoEgresoStock, Ven_Hora = @HoraEmision, Ven_LicenciaCond = @DocumentoReferenciaTipo, Usu_Codigo = @UsuarioId, Ven_Abonado = @Abonado, 
                            Ven_Saldo = @Saldo, Ven_FechaRecepcion = @FechaEmision, Ctrl_Cilindros = @ControlarCilindros, Ven_NetoMercaderia = @NetoMercaderia, Ven_NetoBolsa = @NetoBolsa,
                            Ven_Costo = @CostoTotal, Ven_Utilidad = @UtilidadTotal, Ven_NroCuotas = @NumeroCuotas, Ven_Cancelado = @IsCancelado, Ven_FechaMod = GETDATE(), Ven_Facturado = 'N',
                            Ven_FechaEmision = @FechaDocumentoReferencia WHERE Conf_Codigo = @EmpresaId AND TDoc_Codigo = @TipoDocumentoId AND Ven_Serie = @Serie AND Ven_Numero = @Numero";

                var splitDocumentoReferenciaId = string.IsNullOrWhiteSpace(documentoVenta.DocumentoReferenciaId) ? null : SplitId(documentoVenta.DocumentoReferenciaId);

                await db.ExecuteAsync(query, new
                {
                    documentoVenta.FechaEmision,
                    documentoVenta.FechaVencimiento,
                    documentoVenta.ClienteId,
                    documentoVenta.ClienteTipoDocumentoIdentidadId,
                    documentoVenta.ClienteNumeroDocumentoIdentidad,
                    documentoVenta.ClienteDireccionId,
                    documentoVenta.ClienteDireccion,
                    documentoVenta.PersonalId,
                    documentoVenta.Letra,
                    documentoVenta.LetraId,
                    documentoVenta.TipoCambio,
                    documentoVenta.TipoVentaId,
                    documentoVenta.TipoCobroId,
                    documentoVenta.NumeroOperacion,
                    documentoVenta.CuentaCorrienteId,
                    documentoVenta.DocumentoReferenciaId,
                    Abonar = documentoVenta.Abonar ? "S" : "N",
                    documentoVenta.MotivoNotaId,
                    documentoVenta.MotivoNotaDescripcion,
                    documentoVenta.MotivoSustento,
                    documentoVenta.GuiaRemision,
                    documentoVenta.NumeroPedido,
                    documentoVenta.Observacion,
                    IsAnticipo = documentoVenta.IsAnticipo ? "S" : "N",
                    IsOperacionGratuita = documentoVenta.IsOperacionGratuita ? "S" : "N",
                    IncluyeIGV = documentoVenta.IncluyeIGV ? "S" : "N",
                    AfectarStock = documentoVenta.AfectarStock ? "S" : "N",
                    documentoVenta.TotalOperacionesInafectas,
                    documentoVenta.TotalOperacionesGratuitas,
                    documentoVenta.SubTotal,
                    documentoVenta.TotalAnticipos,
                    documentoVenta.TotalNeto,
                    documentoVenta.MontoIGV,
                    documentoVenta.MontoRetencion,
                    documentoVenta.MontoDetraccion,
                    documentoVenta.MontoImpuestoBolsa,
                    documentoVenta.Total,
                    documentoVenta.PorcentajeIGV,
                    documentoVenta.PorcentajeRetencion,
                    documentoVenta.PorcentajeDetraccion,
                    documentoVenta.FactorImpuestoBolsa,
                    documentoVenta.IngresoEgresoStock,
                    documentoVenta.HoraEmision,
                    DocumentoReferenciaTipo = splitDocumentoReferenciaId is null ? null : Comun.GetTipoDocumentoAbreviatura(splitDocumentoReferenciaId.TipoDocumentoId),
                    documentoVenta.UsuarioId,
                    Abonado = montoAbonado,
                    Saldo = saldo,
                    IsCancelado = saldo > 0 ? "N" : "S",
                    ControlarCilindros = documentoVenta.ControlarCilindros ? "S" : "N",
                    documentoVenta.NetoMercaderia,
                    documentoVenta.NetoBolsa,
                    documentoVenta.CostoTotal,
                    documentoVenta.UtilidadTotal,
                    documentoVenta.NumeroCuotas,
                    documentoVenta.FechaDocumentoReferencia,
                    documentoVenta.EmpresaId,
                    documentoVenta.TipoDocumentoId,
                    documentoVenta.Serie,
                    documentoVenta.Numero
                });
            }
        }

        public async Task Eliminar(string id)
        {
            using (var db = GetConnection())
            {
                await db.ExecuteAsync("SP_UpdateCantPendVenta", new
                {
                    codigo = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 18 },
                    tipo = new DbString { Value = "-", IsAnsi = true, IsFixedLength = false, Length = 1 }
                }, commandType: CommandType.StoredProcedure, commandTimeout: 600);

                await db.ExecuteAsync("SP_VentasEliminar", new
                {
                    codigo = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 18 }
                }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            }
        }

        public async Task Anular(string id)
        {
            using (var db = GetConnection())
            {
                await db.ExecuteAsync("SP_UpdateCantPendVenta", new
                {
                    codigo = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 18 },
                    tipo = new DbString { Value = "-", IsAnsi = true, IsFixedLength = false, Length = 1 }
                }, commandType: CommandType.StoredProcedure, commandTimeout: 600);

                await db.ExecuteAsync("SP_VentasAnular", new
                {
                    codigo = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 18 }
                }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oDocumentoVenta> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"	SELECT
									V.Conf_Codigo AS EmpresaId,
									V.TDoc_Codigo AS TipoDocumentoId,
									V.Ven_Serie AS Serie,
									V.Ven_Numero AS Numero,
									V.Ven_Fecha AS FechaEmision,
									V.Ven_Venci AS FechaVencimiento,
									V.Ven_NroComp AS Cotizacion,
									V.Ven_DocFactur AS CotizacionId,
									V.Cli_Codigo AS ClienteId,
									RTRIM(C.Cli_RazonSocial) AS ClienteNombre,
									V.Ven_RucDni AS ClienteNumeroDocumentoIdentidad,
									V.Id_DireccionLlegada AS ClienteDireccionId,
									V.Ven_DireccionLleg AS ClienteDireccion,
									V.Per_Codigo AS PersonalId,
									V.Ven_Condicion AS Letra,
									V.Ven_Referencia AS LetraId,
									V.Ven_Moneda AS MonedaId,
									V.Ven_TCambio AS TipoCambio,
									V.Ven_TVenta AS TipoVentaId,
									V.Ven_TPago AS TipoCobroId,
									V.Ven_TipoComp AS NumeroOperacion,
									V.Ven_Telefono AS CuentaCorrienteId,
									V.Ven_CertifInscrip AS DocumentoReferenciaId,
									V.Ven_FechaEmision AS FechaDocumentoReferencia,
									CAST(CASE WHEN V.Ven_Abonar = 'S' THEN 1 ELSE 0 END AS BIT) AS Abonar,
									V.Mot_Codigo AS MotivoNotaId,
									V.Ven_PlacaRodaje AS MotivoNotaDescripcion,
									V.Ven_Sustento AS MotivoSustento,
									V.Ven_GuiaRemision AS GuiaRemision,
									V.Ven_OrdenCompra AS NumeroPedido,
                                    V.Ven_CotTiemEntrega AS Orden,
									V.Ven_Observ AS Observacion,
									CAST(CASE WHEN V.Ven_OpeGratuitas = 'S' THEN 1 ELSE 0 END AS BIT) AS IsOperacionGratuita,
									CAST(CASE WHEN V.Ven_IncluyeIgv = 'S' THEN 1 ELSE 0 END AS BIT) AS IncluyeIGV,
									CAST(CASE WHEN V.Ven_AfectarStock = 'S' THEN 1 ELSE 0 END AS BIT) AS AfectarStock,
									V.Ven_Inafecto AS TotalOperacionesInafectas,
									V.Ven_TotalOpeGratuitas AS TotalOperacionesGratuitas,
									V.Ven_SubTotal AS Subtotal,
									V.Ven_TotalNeto AS TotalNeto,
									V.Ven_MontoIgv AS MontoIGV,
									V.Ven_Retencion AS MontoRetencion,
									V.Ven_Detraccion AS MontoDetraccion,
									V.Ven_MontoImpBolsa AS MontoImpuestoBolsa,
									V.Ven_Total AS Total,
									V.Ven_PorcIgv AS PorcentajeIGV,
									V.Ven_PorcReten AS PorcentajeRetencion,
									V.Ven_PorcDetrac AS PorcentajeDetraccion,
									CAST(CASE WHEN V.Ven_Anulado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsAnulado
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
                return await db.QueryFirstOrDefaultAsync<oDocumentoVenta>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<oPagina<vDocumentoVenta>> Listar(string[] tiposDocumento, DateTime fechaInicio, DateTime fechaFin, string clienteNombre, bool? isEnviado, string personalId, oPaginacion paginacion)
        {
            string query = $@"	SELECT 
									Codigo AS Id,
									Fecha AS FechaEmision,
									Documento AS NumeroDocumento,
									Razon_Social AS ClienteNombre,
									Ruc_Dni AS ClienteNumero,
									Moneda AS MonedaId,
									Total,
									CAST(CASE WHEN Cancelado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsCancelado,
									CAST(CASE WHEN AfectarStock = 'S' THEN 1 ELSE 0 END AS BIT) AS AfectarStock,
									CAST(CASE WHEN Anulado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsAnulado,
									CAST(CASE WHEN Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado,
									pedido AS NotaPedido,
									Ven_GuiaRemision AS GuiaRemision,
                                    Personal AS Personal,
									CAST(Sunat AS BIT) AS IsEnviado
								FROM 
									v_lst_Venta
								WHERE 
									TipoDoc IN ({JoinToQuery(tiposDocumento)})
									AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
									AND Razon_Social LIKE '%' + @clienteNombre + '%'
									{(string.IsNullOrWhiteSpace(personalId) ? string.Empty : "AND Per_Codigo = @personalId")}
									{(isEnviado is null ? string.Empty : "AND Sunat = @isEnviado")}
								ORDER BY
									Fecha DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vDocumentoVenta> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    clienteNombre = new DbString { Value = clienteNombre, IsAnsi = true, IsFixedLength = false, Length = 250 },
                    personalId = new DbString { Value = personalId, IsAnsi = true, IsFixedLength = true, Length = 8 },
                    isEnviado = isEnviado is null || !isEnviado.Value ? 0 : 1
                }))
                {
                    pagina = new oPagina<vDocumentoVenta>
                    {
                        Data = await result.ReadAsync<vDocumentoVenta>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<oPagina<vDocumentoVentaSimplificado>> ListarSimplificado(string numeroDocumento, oPaginacion paginacion)
        {
            string query = $@"   SELECT
	                                Codigo AS Id,
	                                Fecha AS FechaEmision,
	                                Ruc_Dni AS ClienteNumero,
	                                Razon_Social AS ClienteNombre,
	                                Documento AS NumeroDocumento,
	                                Moneda AS MonedaId,
	                                Total
                                FROM
	                                v_lst_venta
                                WHERE
	                                TipoDoc IN ('01', '03')
	                                AND Anulado = 'N'
	                                AND Ven_Facturado = 'N'
	                                AND Documento LIKE '%' + @numeroDocumento + '%'
                                ORDER BY
                                    Documento DESC
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vDocumentoVentaSimplificado> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { numeroDocumento = new DbString { Value = numeroDocumento, IsAnsi = true, IsFixedLength = false, Length = 24 } }))
                {
                    pagina = new oPagina<vDocumentoVentaSimplificado>
                    {
                        Data = await result.ReadAsync<vDocumentoVentaSimplificado>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<oPagina<vDocumentoVentaAnticipo>> ListarAnticipos(DateTime fechaInicio, DateTime fechaFin, string clienteId, string numeroDocumento, oPaginacion paginacion)
        {
            string query = $@"   SELECT 
	                                Codigo AS Id,
	                                Fecha AS FechaEmision,
	                                Documento AS NumeroDocumento,
	                                Moneda AS MonedaId,
	                                TCambio AS TipoCambio,
	                                SubTotal,
	                                Total
                                FROM 
	                                v_lst_Venta
                                WHERE 
	                                TipoDoc IN ('01', '03')
	                                AND Cli_Codigo = @clienteId
	                                AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
	                                AND Anulado = 'N'
	                                AND Anticipo = 'S'
	                                AND Documento LIKE '%' + @numeroDocumento + '%'
                                ORDER BY
									Fecha DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vDocumentoVentaAnticipo> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    numeroDocumento = new DbString { Value = numeroDocumento, IsAnsi = true, IsFixedLength = false, Length = 24 }
                }))
                {
                    pagina = new oPagina<vDocumentoVentaAnticipo>
                    {
                        Data = await result.ReadAsync<vDocumentoVentaAnticipo>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<(string DocumentoReferenciaId, int? AbonoId)> GetDatosDocumentoReferencia(string id)
        {
            var splitId = SplitId(id);
            string query = "SELECT Ven_CertifInscrip, Ven_AboItem FROM Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<(string documentoReferenciaId, int? abonoId)>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task ActualizarCampoDocumentoReferenciaAbonoId(string id, int abonoId)
        {
            var splitId = SplitId(id);
            string query = @"UPDATE Venta SET Ven_AboItem = @abonoId WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    abonoId,
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<string> GetNuevoNumero(string empresaId, string tipoDocumentoId, string serie)
        {
            using (var db = GetConnection())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@empresa", empresaId, dbType: DbType.AnsiString, size: 2);
                parametros.Add("@sucursal", "01", dbType: DbType.AnsiString, size: 2);
                parametros.Add("@tdoc", tipoDocumentoId, dbType: DbType.AnsiString, size: 2);
                parametros.Add("@serie", serie, dbType: DbType.AnsiString, size: 4);
                parametros.Add("@numero", dbType: DbType.AnsiString, size: 10, direction: ParameterDirection.Output);

                await db.ExecuteAsync("SP_CorrelativoVenta", parametros, commandType: CommandType.StoredProcedure);

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

        public async Task<(bool IsBloqueado, string Mensaje)> IsBloqueado(string id)
        {
            string mensaje = "El registro está bloqueado";

            var splitId = SplitId(id);

            string query = @"   SELECT
                                    CAST(CASE WHEN Ven_Anulado = 'S' THEN 1 ELSE 0 END AS BIT),
                                    CAST(CASE WHEN Ven_Facturado = 'S' THEN 1 ELSE 0 END AS BIT),
                                    CAST(CASE WHEN Ven_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT),
                                    Ven_Fecha
                                FROM 
                                    Venta
                                WHERE 
                                    Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                var (isAnulado, isFacturado, isBloqueado, fechaEmision) =
                    await db.QueryFirstOrDefaultAsync<(bool IsAnulado, bool IsFacturado, bool IsBloqueado, DateTime FechaEmision)>(query, new
                    {
                        empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                    });

                if (isAnulado)
                {
                    mensaje = "El registro está anulado.";
                }
                if (isFacturado)
                {
                    mensaje += " porque tiene una guía de venta amarrada.";
                }
                else if (isBloqueado)
                {
                    mensaje += ".";
                }
                else
                {
                    query = @"  SELECT COUNT(Conf_Codigo) FROM Abono_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

                    var cantidadAbonos = await db.QueryFirstAsync<int>(query, new
                    {
                        empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                    });

                    if (cantidadAbonos == 0)
                    {
                        return (false, string.Empty);
                    }
                    else if (cantidadAbonos > 1)
                    {
                        mensaje += " ya que tiene más de un abono.";
                    }
                    else if (cantidadAbonos == 1)
                    {
                        query = @"  SELECT COUNT(Conf_Codigo) FROM Abono_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie 
                                    AND Ven_Numero = @numero AND Abo_Fecha = @fechaEmision AND Abo_Bloquedo = 'N'";

                        cantidadAbonos = await db.QueryFirstAsync<int>(query, new
                        {
                            empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                            tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                            serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                            numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                            fechaEmision
                        });

                        if (cantidadAbonos == 1)
                        {
                            return (false, string.Empty);
                        }
                        else
                        {
                            mensaje += " ya que tiene un abono en una fecha distinta a la de la venta.";
                        }
                    }
                }
            }

            return (true, mensaje);
        }

        public async Task<bool> IsEnviadoSunat(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT CAST(CASE WHEN simpleubl_cpe_id IS NULL THEN 0 ELSE 1 END AS BIT) FROM Venta 
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

        public async Task<decimal> GetSaldoDocumento(string id)
        {
            var splitId = SplitId(id);
            string query = "SELECT ISNULL(Ven_Saldo, 0) FROM Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<decimal?>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                }) ?? 0;
            }
        }

        public async Task ActualizarCantidadEntrada(string id, string articuloId, decimal cantidad, Operacion operacion)
        {
            var splitId = SplitId(id);
            var splitArticuloId = new oSplitArticuloId(articuloId);

            using (var db = GetConnection())
            {
                string query = @$"  UPDATE Detalle_Venta SET DVen_CantEnt = DVen_CantEnt {(operacion == Operacion.Aumentar ? "+" : "-")} @cantidad
                                    WHERE (Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero)
                                    AND (Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId)";

                await db.ExecuteAsync(query, new
                {
                    cantidad,
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    lineaId = new DbString { Value = splitArticuloId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitArticuloId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    articuloId = new DbString { Value = splitArticuloId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                });
            }
        }

        public async Task DeshacerVenta(string id)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = "SELECT Ven_TipoComp FROM Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

                var documentoRelacionadoId = await db.QueryFirstOrDefaultAsync<string>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });

                if (!string.IsNullOrWhiteSpace(documentoRelacionadoId))
                {
                    var detalles = await new dDocumentoVentaDetalle(_connectionString).ListarPorDocumentoVenta(id);

                    foreach (var detalle in detalles)
                    {
                        await ActualizarCantidadEntrada(documentoRelacionadoId, detalle.Id, detalle.Cantidad, Operacion.Aumentar);
                    }
                }
            }
        }

        public async Task Enviar(oEnviarDocumentoVenta enviarDocumentoVenta)
        {
            string query = "UPDATE Venta SET Ven_Transaccion = @enviar WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                string strEnviar = enviarDocumentoVenta.Enviar ? "S" : "N";

                foreach (var id in enviarDocumentoVenta.Ids)
                {
                    var splitId = SplitId(id);

                    await db.ExecuteAsync(query, new
                    {
                        enviar = new DbString { Value = strEnviar, IsAnsi = true, IsFixedLength = true, Length = 1 },
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
