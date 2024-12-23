using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otr;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Repositorio.Compra
{
    public class dOrdenCompra : dComun
    {
        public const string TipoDocumentoId = "OC";

        public dOrdenCompra(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oOrdenCompra ordenCompra)
        {
            string query = @"	INSERT INTO Compra (Conf_Codigo, Prov_Codigo, TDoc_Codigo, Com_Serie, Com_Numero, Cli_Codigo, Com_Fecha, Com_FechaContable,
								Com_Venci, Com_RucDni, Com_DireccionPart, Per_Codigo, Per_Codigo2, Per_Codigo3, cod_contacto, Com_TCompra, Com_Moneda, Com_TCambio,
								Com_TPago, Com_NroComp, Com_Telefono, Com_LugEntrega, Com_ProvCtaCte, Com_ProvCtaCte2, Com_Observ, Com_SubTotal,
								Com_PorcIgv, Com_MontoIgv, Com_ValorVenta, Com_TotalNeto, Com_PorcReten, Com_Retencion, Com_PorcPercep, Com_Percepcion, Com_Total,
								Com_IncluyeIgv, Com_Hora, Usu_Codigo, Com_FechaReg, Com_Abonado, Com_Saldo, Com_Cancelado, Com_Anulado, Suc_Codigo,
                                Com_AfectarStock, Com_IngEgrStock, Com_PorcDscto, Com_Descuento, Com_Otros, Com_Inafecto, Com_ConIgv, Com_Retenc, Com_Percep,
                                Com_Bloqueado, Com_BloqUsu, Com_BloqSist, Com_BloqProc, Com_CierreZ, Com_CierreX, TipO_Codigo, Com_AboItem, Com_Abonar, Com_Facturado)
								VALUES (@EmpresaId, @ProveedorId, @TipoDocumentoId, @Serie, @Numero, @ClienteId, @FechaEmision, @FechaContable,
								@FechaVencimiento, @ProveedorNumeroDocumentoIdentidad, @ProveedorDireccion, @Responsable1Id, @Responsable2Id, @Responsable3Id, @ProveedorContactoId, @TipoCompraId, @MonedaId, @TipoCambio,
								@TipoPagoId, @NumeroOperacion, @CuentaCorrienteId, @LugarEntrega, @ProveedorCuentaCorriente1Id, @ProveedorCuentaCorriente2Id, @Observacion, @SubTotal,
								@PorcentajeIGV, @MontoIGV, @SubTotal, @TotalNeto, @PorcentajeRetencion, @MontoRetencion, @PorcentajePercepcion, @MontoPercepcion, @Total,
								@IncluyeIGV, @HoraEmision, @UsuarioId, GETDATE(), @Total, 0, 'N', 'N', '01',
                                'N', '+', 0, 0, 0, 0, 'S', 'N', 'N', 
                                'N', 'N', 'N', 'N', 'N', 'N', '02', 0, 'S', 'N')";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    ordenCompra.EmpresaId,
                    ordenCompra.ProveedorId,
                    ordenCompra.TipoDocumentoId,
                    ordenCompra.Serie,
                    ordenCompra.Numero,
                    ordenCompra.ClienteId,
                    ordenCompra.FechaEmision,
                    ordenCompra.FechaContable,
                    ordenCompra.FechaVencimiento,
                    ordenCompra.ProveedorNumeroDocumentoIdentidad,
                    ordenCompra.ProveedorDireccion,
                    ordenCompra.Responsable1Id,
                    ordenCompra.Responsable2Id,
                    ordenCompra.Responsable3Id,
                    ordenCompra.ProveedorContactoId,
                    ordenCompra.TipoCompraId,
                    ordenCompra.MonedaId,
                    ordenCompra.TipoCambio,
                    ordenCompra.TipoPagoId,
                    ordenCompra.NumeroOperacion,
                    ordenCompra.CuentaCorrienteId,
                    ordenCompra.LugarEntrega,
                    ordenCompra.ProveedorCuentaCorriente1Id,
                    ordenCompra.ProveedorCuentaCorriente2Id,
                    ordenCompra.Observacion,
                    ordenCompra.SubTotal,
                    ordenCompra.PorcentajeIGV,
                    ordenCompra.MontoIGV,
                    ordenCompra.TotalNeto,
                    ordenCompra.PorcentajeRetencion,
                    ordenCompra.MontoRetencion,
                    ordenCompra.PorcentajePercepcion,
                    ordenCompra.MontoPercepcion,
                    ordenCompra.Total,
                    IncluyeIGV = ordenCompra.IncluyeIGV ? "S" : "N",
                    ordenCompra.HoraEmision,
                    ordenCompra.UsuarioId
                });
            }
        }

        public async Task Modificar(oOrdenCompra ordenCompra)
        {
            string query = @"   UPDATE Compra SET Com_Fecha = @FechaEmision, Com_FechaContable = @FechaContable, Com_Venci = @FechaVencimiento, 
                                Com_RucDni = @ProveedorNumeroDocumentoIdentidad, Com_DireccionPart = @ProveedorDireccion, Per_Codigo = @Responsable1Id, 
                                Per_Codigo2 = @Responsable2Id, Per_Codigo3 = @Responsable3Id, cod_contacto = @ProveedorContactoId, Com_TCompra = @TipoCompraId, 
                                Com_TCambio = @TipoCambio, Com_TPago = @TipoPagoId, Com_NroComp = @NumeroOperacion, Com_Telefono = @CuentaCorrienteId, 
                                Com_LugEntrega = @LugarEntrega, Com_ProvCtaCte = @ProveedorCuentaCorriente1Id, Com_ProvCtaCte2 = @ProveedorCuentaCorriente2Id, 
                                Com_Observ = @Observacion, Com_SubTotal = @SubTotal, Com_PorcIgv = @PorcentajeIGV, Com_MontoIgv = @MontoIGV, Com_ValorVenta = @SubTotal, 
                                Com_TotalNeto = @TotalNeto, Com_PorcReten = @PorcentajeRetencion, Com_Retencion = @MontoRetencion, Com_PorcPercep = @PorcentajePercepcion,
                                Com_Percepcion = @MontoPercepcion, Com_Total = @Total, Com_IncluyeIgv = @IncluyeIGV, Usu_Codigo = @UsuarioId, Com_FechaMod = GETDATE(),
                                Com_Abonado = @Total WHERE Conf_Codigo = @EmpresaId AND Prov_Codigo = @ProveedorId AND TDoc_Codigo = @TipoDocumentoId
                                AND Com_Serie = @Serie AND Com_Numero = @Numero AND Cli_Codigo = @ClienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    ordenCompra.FechaEmision,
                    ordenCompra.FechaContable,
                    ordenCompra.FechaVencimiento,
                    ordenCompra.ProveedorNumeroDocumentoIdentidad,
                    ordenCompra.ProveedorDireccion,
                    ordenCompra.Responsable1Id,
                    ordenCompra.Responsable2Id,
                    ordenCompra.Responsable3Id,
                    ordenCompra.ProveedorContactoId,
                    ordenCompra.TipoCompraId,
                    ordenCompra.TipoCambio,
                    ordenCompra.TipoPagoId,
                    ordenCompra.NumeroOperacion,
                    ordenCompra.CuentaCorrienteId,
                    ordenCompra.LugarEntrega,
                    ordenCompra.ProveedorCuentaCorriente1Id,
                    ordenCompra.ProveedorCuentaCorriente2Id,
                    ordenCompra.Observacion,
                    ordenCompra.SubTotal,
                    ordenCompra.PorcentajeIGV,
                    ordenCompra.MontoIGV,
                    ordenCompra.TotalNeto,
                    ordenCompra.PorcentajeRetencion,
                    ordenCompra.MontoRetencion,
                    ordenCompra.PorcentajePercepcion,
                    ordenCompra.MontoPercepcion,
                    ordenCompra.Total,
                    IncluyeIGV = ordenCompra.IncluyeIGV ? "S" : "N",
                    ordenCompra.HoraEmision,
                    ordenCompra.UsuarioId,
                    ordenCompra.EmpresaId,
                    ordenCompra.ProveedorId,
                    ordenCompra.TipoDocumentoId,
                    ordenCompra.Serie,
                    ordenCompra.Numero,
                    ordenCompra.ClienteId
                });
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = @"DELETE Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
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
        #endregion

        #region Otros Métodos
        public async Task<oOrdenCompra> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"	SELECT
									C.Conf_Codigo AS EmpresaId,
									C.Prov_Codigo AS ProveedorId,
									C.TDoc_Codigo AS TipoDocumentoId,
									C.Com_Serie AS Serie,
									C.Com_Numero AS Numero,
									C.Cli_Codigo AS ClienteId,
									C.Com_Fecha AS FechaEmision,
									C.Com_FechaContable AS FechaContable,
									C.Com_Venci AS FechaVencimiento,
									RTRIM(P.Pro_RazonSocial) AS ProveedorNombre,
									C.Com_RucDni AS ProveedorNumeroDocumentoIdentidad,
									C.Com_DireccionPart AS ProveedorDireccion,
									C.Per_Codigo AS Responsable1Id,
									C.Per_Codigo2 AS Responsable2Id,
									C.Per_Codigo3 AS Responsable3Id,
									C.cod_contacto AS ProveedorContactoId,
									C.Com_TCompra AS TipoCompraId,
									C.Com_Moneda AS MonedaId,
									C.Com_TCambio AS TipoCambio,
									C.Com_TPago AS TipoPagoId,
									C.Com_NroComp AS NumeroOperacion,
									C.Com_Telefono AS CuentaCorrienteId,
									C.Com_LugEntrega AS LugarEntrega,
									C.Com_ProvCtaCte AS ProveedorCuentaCorriente1Id,
									C.Com_ProvCtaCte2 AS ProveedorCuentaCorriente2Id,
									C.Com_Observ AS Observacion,
									C.Com_SubTotal AS SubTotal,
									C.Com_PorcIgv AS PorcentajeIGV,
									C.Com_MontoIgv AS MontoIGV,
									C.Com_TotalNeto AS TotalNeto,
									C.Com_PorcReten AS PorcentajeRetencion,
									C.Com_Retencion AS MontoRetencion,
									C.Com_PorcPercep AS PorcentajePercepcion,
									C.Com_Percepcion AS MontoPercepcion,
									C.Com_Total AS Total,
									CAST(CASE WHEN C.Com_IncluyeIgv = 'S' THEN 1 ELSE 0 END AS BIT) AS IncluyeIGV
								FROM
									Compra C
                                    INNER JOIN Proveedor P ON C.Prov_Codigo = P.Prov_Codigo
								WHERE
									C.Conf_Codigo = @empresaId
									AND C.Prov_Codigo = @proveedorId
									AND C.TDoc_Codigo = @tipoDocumentoId
									AND C.Com_Serie = @serie
									AND C.Com_Numero = @numero
									AND C.Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oOrdenCompra>(query, new
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

        public async Task<oPagina<vOrdenCompra>> Listar(DateTime fechaInicio, DateTime fechaFin, string proveedorNombre, string estado, oPaginacion paginacion)
        {
            string query = $@"	SELECT
									Codigo AS Id,
									Fecha AS FechaContable,
									Documento AS NumeroDocumento,
									Proveedor As ProveedorNombre,
									Ruc As ProveedorNumero,
									Moneda AS MonedaId,
									Total,
									Com_NroComp AS DocumentoRelacionado,
									(CASE WHEN com_facturado = 'S' THEN 'ENTREGADO' ELSE 'PENDIENTE' END) AS Estado
								FROM 
									v_lst_compra
								WHERE 
									(Fecha BETWEEN @fechaInicio AND @fechaFin)
									AND TipoDoc = 'OC'
									AND Proveedor LIKE '%' + @proveedorNombre + '%'
									{(string.IsNullOrWhiteSpace(estado) ? string.Empty : "AND Com_Facturado = @estado")}
								ORDER BY
									Fecha DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vOrdenCompra> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    proveedorNombre = new DbString { Value = proveedorNombre, IsAnsi = true, IsFixedLength = false, Length = 100 },
                    estado = new DbString { Value = estado, IsAnsi = true, IsFixedLength = false, Length = 1 }
                }))
                {
                    pagina = new oPagina<vOrdenCompra>
                    {
                        Data = await result.ReadAsync<vOrdenCompra>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<oPagina<vOrdenCompraPendiente>> ListarPendientes(DateTime fechaInicio, DateTime fechaFin, string proveedorId, oPaginacion paginacion)
        {
            string query = $@"	SELECT
									Codigo AS Id,
									Fecha AS FechaContable,
									Documento AS NumeroDocumento,
									Proveedor As ProveedorNombre,
									Moneda AS MonedaId,
									Total
								FROM 
									v_lst_compra
								WHERE 
									(Fecha BETWEEN @fechaInicio AND @fechaFin)
									AND TipoDoc = '{TipoDocumentoId}'
									AND Prov_Codigo = @proveedorId
                                    AND Com_Facturado = 'N'
								ORDER BY
									Fecha DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vOrdenCompraPendiente> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    proveedorId = new DbString { Value = proveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                }))
                {
                    pagina = new oPagina<vOrdenCompraPendiente>
                    {
                        Data = await result.ReadAsync<vOrdenCompraPendiente>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT COUNT(Conf_Codigo) FROM Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

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

            string query = @"   SELECT CAST(CASE WHEN Com_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) FROM Compra
                                WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

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

        public async Task<bool> IsFacturado(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT CAST(CASE WHEN Com_Facturado = 'S' THEN 1 ELSE 0 END AS BIT) FROM Compra
                                WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

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

        public async Task<string> GetNuevoNumero(string empresaId, string serie)
        {
            return await GetNuevoId("SELECT MAX(Com_Numero) FROM Compra WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie", new
            {
                empresaId = new DbString { Value = empresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                tipoDocumentoId = new DbString { Value = TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                serie = new DbString { Value = serie, IsAnsi = true, IsFixedLength = true, Length = 4 }
            }, "000000000#");
        }

        public async Task ActualizarCantidadPendiente(string documentoCompraId, Operacion operacion)
        {
            using (var db = GetConnection())
            {
                await db.ExecuteAsync("Sp_UpdateCantPendOrdenCompra", new
                {
                    Codigo = new DbString { Value = documentoCompraId, IsAnsi = true, IsFixedLength = false, Length = 24 },
                    Tipo = new DbString { Value = operacion == Operacion.Aumentar ? "+" : "-", IsAnsi = true, IsFixedLength = false, Length = 1 }
                }, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 600);
            }
        }

        public async Task ActualizarCantidadEntrada(string id, string articuloId, decimal cantidad, Operacion operacion)
        {
            var splitId = SplitId(id);
            var splitArticuloId = new oSplitArticuloId(articuloId);

            using (var db = GetConnection())
            {
                string query = $@"  UPDATE Detalle_Compra SET DCom_CantidadEntr = DCom_CantidadEntr {(operacion == Operacion.Aumentar ? "+" : "-")} @cantidad
                                    WHERE (Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId)
                                    AND (Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId)";

                await db.ExecuteAsync(query, new
                {
                    cantidad,
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    lineaId = new DbString { Value = splitArticuloId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitArticuloId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    articuloId = new DbString { Value = splitArticuloId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                });

                query = @"  SELECT ISNULL(DCom_Cantidad, 0), ISNULL(DCom_CantidadEntr, 0) FROM Detalle_Compra
                            WHERE (Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId)
                            AND (Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId)";

                var (cantidadDetalle, cantidadEntregada) = await db.QueryFirstOrDefaultAsync<(decimal Cantidad, decimal CantidadEntregada)>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    lineaId = new DbString { Value = splitArticuloId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitArticuloId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    articuloId = new DbString { Value = splitArticuloId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                });

                bool isEntregado = cantidadEntregada >= cantidadDetalle && cantidadEntregada > 0;

                query = @"  UPDATE Detalle_Compra SET DCom_Estado = @isEntregado 
                            WHERE (Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId)
                            AND (Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId)";

                await db.ExecuteAsync(query, new
                {
                    isEntregado = new DbString { Value = isEntregado ? "S" : "N", IsAnsi = true, IsFixedLength = false, Length = 1 },
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    lineaId = new DbString { Value = splitArticuloId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitArticuloId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    articuloId = new DbString { Value = splitArticuloId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                });
            }
        }

        public async Task ActualizarEstadoFacturado(string id, bool isFacturado)
        {
            var splitId = SplitId(id);

            string query = @"   UPDATE Compra SET Com_Facturado = @isFacturado WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId 
                                AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    isFacturado = new DbString { Value = isFacturado ? "S" : "N", IsAnsi = true, IsFixedLength = true, Length = 1 },
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });
            }
        }

        public static oSplitDocumentoCompraId SplitId(string id) => new(id);
        #endregion
    }
}
