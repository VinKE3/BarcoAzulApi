using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;
using MCWebAPI.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Compra
{
    public class dDocumentoCompra : dComun
    {
        public dDocumentoCompra(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oDocumentoCompra documentoCompra)
        {
            string query = @"   INSERT INTO Compra (Conf_Codigo, Prov_Codigo, TDoc_Codigo, Com_Serie, Com_Numero, Cli_Codigo, Com_Fecha, Com_FechaContable, Com_Venci,
                                Com_RucDni, Com_DireccionPart, Com_TCompra, Com_Moneda, Com_TCambio, Com_TPago, Com_NroComp, Com_Telefono, Com_TipoDocMod, 
                                Com_Abonar, Mot_Codigo, Com_OtroMotivo, Com_GuiaRemision, Com_Observ, Com_SubTotal, Com_PorcIgv, Com_MontoIgv, Com_ValorVenta, Com_TotalNeto, Com_Total, Com_IncluyeIgv,
                                Com_AfectarStock, Com_AfectarPrecio, Usu_Codigo, Com_FechaReg, Com_Hora, Suc_Codigo, Com_IngEgrStock, Per_Codigo, Com_PorcDscto, Com_Descuento, Com_Otros, Com_Inafecto,
                                Com_PorcReten, Com_Retencion, Com_PorcPercep, Com_Percepcion, Com_Abonado, Com_Saldo, Com_ConIgv, Com_Retenc, Com_Percep, Com_Anulado, Com_Cancelado,
                                Com_Bloqueado, Com_BloqUsu, Com_BloqSist, Com_BloqProc, Com_Documento, Com_CierreZ, Com_CierreX, Com_LlegViaNom, TipO_Codigo, Com_AboItem,
                                Com_Facturado)
                                VALUES (@EmpresaId, @ProveedorId, @TipoDocumentoId, @Serie, @Numero, @ClienteId, @FechaEmision, @FechaContable, @FechaVencimiento,
                                @ProveedorNumeroDocumentoIdentidad, @ProveedorDireccion, @TipoCompraId, @MonedaId, @TipoCambio, @TipoPagoId, @NumeroOperacion, @CuentaCorrienteId, @DocumentoReferenciaId, 
                                @Abonar, @MotivoNotaId, @MotivoSustento, @GuiaRemision, @Observacion, @SubTotal, @PorcentajeIGV, @MontoIGV, @SubTotal, @TotalNeto, @Total, @IncluyeIGV,
                                @AfectarStock, @AfectarPrecio, @UsuarioId, GETDATE(), @HoraEmision, '01', @IngresoEgresoStock, @PersonalId, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, @Total, @ConIGV, 'S', 'N', 'N', 'N', 
                                'S', 'N', 'N', 'N', @NumeroDocumento, 'N', 'N', @NumeroDocumento, @TipoOperacionId, 0, 
                                'N')";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    documentoCompra.EmpresaId,
                    documentoCompra.ProveedorId,
                    documentoCompra.TipoDocumentoId,
                    documentoCompra.Serie,
                    documentoCompra.Numero,
                    documentoCompra.ClienteId,
                    documentoCompra.FechaEmision,
                    documentoCompra.FechaContable,
                    documentoCompra.FechaVencimiento,
                    documentoCompra.ProveedorNumeroDocumentoIdentidad,
                    documentoCompra.ProveedorDireccion,
                    documentoCompra.TipoCompraId,
                    documentoCompra.MonedaId,
                    documentoCompra.TipoCambio,
                    documentoCompra.TipoPagoId,
                    documentoCompra.NumeroOperacion,
                    documentoCompra.CuentaCorrienteId,
                    documentoCompra.DocumentoReferenciaId,
                    Abonar = documentoCompra.Abonar ? "S" : "N",
                    documentoCompra.MotivoNotaId,
                    documentoCompra.MotivoSustento,
                    documentoCompra.GuiaRemision,
                    documentoCompra.Observacion,
                    documentoCompra.SubTotal,
                    documentoCompra.PorcentajeIGV,
                    documentoCompra.MontoIGV,
                    documentoCompra.TotalNeto,
                    documentoCompra.Total,
                    IncluyeIGV = documentoCompra.IncluyeIGV ? "S" : "N",
                    AfectarStock = documentoCompra.AfectarStock ? "S" : "N",
                    AfectarPrecio = documentoCompra.AfectarPrecio ? "S" : "N",
                    documentoCompra.UsuarioId,
                    documentoCompra.HoraEmision,
                    IngresoEgresoStock = documentoCompra.TipoDocumentoId == "07" ? "-" : "+",
                    documentoCompra.PersonalId,
                    ConIGV = documentoCompra.TipoDocumentoId == "03" ? "N" : "S",
                    documentoCompra.NumeroDocumento,
                    TipoOperacionId = documentoCompra.TipoDocumentoId == "07" || documentoCompra.TipoDocumentoId == "08" ? "99" : "02",
                    documentoCompra.NumeroOrdenesCompraRelacionadas
                });
            }
        }

        public async Task Modificar(oDocumentoCompra documentoCompra)
        {
            var splitId = SplitId(documentoCompra.Id);

            using (var db = GetConnection())
            {
                string query = @"   SELECT Com_Abonado FROM Compra WHERE Conf_Codigo = @EmpresaId AND Prov_Codigo = @ProveedorId AND TDoc_Codigo = @TipoDocumentoId 
                                    AND Com_Serie = @Serie AND Com_Numero = @Numero AND Cli_Codigo = @ClienteId";

                var montoAbonado = await db.QueryFirstAsync<decimal>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });

                var saldo = documentoCompra.Total - montoAbonado;

                query = @"  UPDATE Compra SET Com_Fecha = @FechaEmision, Com_FechaContable = @FechaContable, Com_Venci = @FechaVencimiento,
                            Com_DireccionPart = @ProveedorDireccion, Com_TCompra = @TipoCompraId, Com_TCambio = @TipoCambio, Com_TPago = @TipoPagoId, Com_NroComp = @NumeroOperacion,
                            Com_Telefono = @CuentaCorrienteId, Com_TipoDocMod = @DocumentoReferenciaId, Com_Abonar = @Abonar, Mot_Codigo = @MotivoNotaId, Com_OtroMotivo = @MotivoSustento,
                            Com_GuiaRemision = @GuiaRemision, Com_Observ = @Observacion, Com_SubTotal = @SubTotal, Com_PorcIgv = @PorcentajeIGV, Com_MontoIgv = @MontoIGV,
                            Com_ValorVenta = @SubTotal, Com_TotalNeto = @TotalNeto, Com_Total = @Total, Com_IncluyeIgv = @IncluyeIGV, Com_AfectarStock = @AfectarStock, Com_AfectarPrecio = @AfectarPrecio,
                            Usu_Codigo = @UsuarioId, Com_FechaMod = GETDATE(), Com_Abonado = @Abonado, Com_Saldo = @Saldo, Com_Cancelado = @IsCancelado, Com_Bloqueado = 'S'
                            WHERE Conf_Codigo = @EmpresaId AND Prov_Codigo = @ProveedorId AND TDoc_Codigo = @TipoDocumentoId
                            AND Com_Serie = @Serie AND Com_Numero = @Numero AND Cli_Codigo = @ClienteId";

                await db.ExecuteAsync(query, new
                {
                    documentoCompra.FechaEmision,
                    documentoCompra.FechaContable,
                    documentoCompra.FechaVencimiento,
                    documentoCompra.ProveedorDireccion,
                    documentoCompra.TipoCompraId,
                    documentoCompra.TipoCambio,
                    documentoCompra.TipoPagoId,
                    documentoCompra.NumeroOperacion,
                    documentoCompra.CuentaCorrienteId,
                    documentoCompra.DocumentoReferenciaId,
                    Abonar = documentoCompra.Abonar ? "S" : "N",
                    documentoCompra.MotivoNotaId,
                    documentoCompra.MotivoSustento,
                    documentoCompra.GuiaRemision,
                    documentoCompra.Observacion,
                    documentoCompra.SubTotal,
                    documentoCompra.PorcentajeIGV,
                    documentoCompra.MontoIGV,
                    documentoCompra.TotalNeto,
                    documentoCompra.Total,
                    IncluyeIGV = documentoCompra.IncluyeIGV ? "S" : "N",
                    AfectarStock = documentoCompra.AfectarStock ? "S" : "N",
                    AfectarPrecio = documentoCompra.AfectarPrecio ? "S" : "N",
                    documentoCompra.UsuarioId,
                    Abonado = montoAbonado,
                    Saldo = saldo,
                    IsCancelado = saldo > 0 ? "N" : "S",
                    documentoCompra.NumeroOrdenesCompraRelacionadas,
                    documentoCompra.EmpresaId,
                    documentoCompra.ProveedorId,
                    documentoCompra.TipoDocumentoId,
                    documentoCompra.Serie,
                    documentoCompra.Numero,
                    documentoCompra.ClienteId
                });
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = "DELETE Compra_Mercaderia WHERE Cod_DocReferencia = @id";

                await db.ExecuteAsync(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 24 } });

                query = @"DELETE Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

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
        public async Task<oDocumentoCompra> GetPorId(string id)
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
									C.Com_TCompra AS TipoCompraId,
									C.Com_Moneda AS MonedaId,
									C.Com_TCambio AS TipoCambio,
									C.Com_TPago AS TipoPagoId,
									C.Com_NroComp AS NumeroOperacion,
									C.Com_Telefono AS CuentaCorrienteId,
									C.Com_TipoDocMod AS DocumentoReferenciaId,
									CAST(CASE WHEN C.Com_Abonar = 'S' THEN 1 ELSE 0 END AS BIT) AS Abonar,
									C.Mot_Codigo AS MotivoNotaId,
									C.Com_OtroMotivo AS MotivoSustento,
									C.Com_GuiaRemision AS GuiaRemision,
									C.Com_Observ AS Observacion,
									C.Com_SubTotal AS SubTotal,
									C.Com_PorcIgv AS PorcentajeIGV,
									C.Com_MontoIgv AS MontoIGV,
									C.Com_TotalNeto AS TotalNeto,
									C.Com_Total AS Total,
                                    C.Com_Abonado AS Abonado,
                                    C.Com_Saldo AS Saldo,
									CAST(CASE WHEN C.Com_IncluyeIgv = 'S' THEN 1 ELSE 0 END AS BIT) AS IncluyeIGV,
									CAST(CASE WHEN C.Com_AfectarStock = 'S' THEN 1 ELSE 0 END AS BIT) AS AfectarStock,
                                    CAST(CASE WHEN C.Com_AfectarPrecio = 'S' THEN 1 ELSE 0 END AS BIT) AS AfectarPrecio
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
                return await db.QueryFirstOrDefaultAsync<oDocumentoCompra>(query, new
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

        public async Task<oPagina<vDocumentoCompra>> Listar(string[] tiposDocumento, DateTime fechaInicio, DateTime fechaFin, string proveedorNombre, oPaginacion paginacion)
        {
            string query = $@"	SELECT 
									Codigo AS Id,
									Fecha AS FechaContable,
									Emision AS FechaEmision,
									Documento AS NumeroDocumento,
									Proveedor AS ProveedorNombre,
									Ruc AS ProveedorNumero,
									Moneda AS MonedaId,
									Total,
									CAST(CASE WHEN Cancelado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsCancelado,
									CAST(CASE WHEN Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado,
									GuiaRemision
								FROM 
									v_lst_compra
								WHERE 
									(Fecha BETWEEN @fechaInicio AND @fechaFin)
									AND TipoDoc IN ({JoinToQuery(tiposDocumento)})
									AND Proveedor LIKE '%' + @proveedorNombre + '%'
								ORDER BY
									Fecha DESC,
									Proveedor DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vDocumentoCompra> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    proveedorNombre = new DbString { Value = proveedorNombre, IsAnsi = true, IsFixedLength = false, Length = 100 }
                }))
                {
                    pagina = new oPagina<vDocumentoCompra>
                    {
                        Data = await result.ReadAsync<vDocumentoCompra>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<oPagina<vDocumentoCompraPendiente>> ListarPendientes(DateTime fechaInicio, DateTime fechaFin, string proveedorId, oPaginacion paginacion)
        {
            string query = $@"   SELECT 
	                                Codigo AS Id,
	                                Fecha AS FechaContable,
	                                Documento AS NumeroDocumento,
	                                Proveedor AS ProveedorNombre,
                                    CodigoPendiente AS CodigoPendiente,
	                                Moneda AS MonedaId,
	                                Total AS Total
                                FROM 
	                                v_lst_compra
                                WHERE 
	                                TipoDoc = '01'
	                                AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
	                                AND Anulado = 'N'
	                                AND Prov_codigo = @proveedorId
                                ORDER BY
									Fecha DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vDocumentoCompraPendiente> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    proveedorId = new DbString { Value = proveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                }))
                {
                    pagina = new oPagina<vDocumentoCompraPendiente>
                    {
                        Data = await result.ReadAsync<vDocumentoCompraPendiente>(),
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

        public async Task<(bool IsBloqueado, string Mensaje)> IsBloqueado(string id)
        {
            string mensaje = "El registro está bloqueado";

            var splitId = SplitId(id);

            string query = @"   SELECT 
                                    CAST(CASE WHEN Com_Facturado = 'S' THEN 1 ELSE 0 END AS BIT),
                                    CAST(CASE WHEN Com_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT),
                                    Com_FechaContable
                                FROM 
                                    Compra
                                WHERE 
                                    Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                    AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                (bool isFacturado, bool isBloqueado, DateTime fechaContable) = await db.QueryFirstOrDefaultAsync<(bool IsFacturado, bool IsBloqueado, DateTime FechaContable)>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                });

                if (isFacturado)
                {
                    mensaje += " ya que ha sido amarrado a una factura de compra.";
                }
                else if (isBloqueado)
                {
                    mensaje += ".";
                }
                else
                {
                    query = @"  SELECT COUNT(Conf_Codigo) FROM Abono_Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId 
                                AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero";

                    var cantidadAbonos = await db.QueryFirstAsync<int>(query, new
                    {
                        empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
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
                        query = @"SELECT COUNT(Conf_Codigo) FROM Abono_Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId 
                                AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Abo_Fecha = @fechaContable AND Abp_Bloqueado = 'N'";

                        cantidadAbonos = await db.QueryFirstAsync<int>(query, new
                        {
                            empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                            proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                            tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                            serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                            numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                            fechaContable
                        });

                        if (cantidadAbonos == 1)
                        {
                            return (false, string.Empty);
                        }
                        else
                        {
                            mensaje += " ya que tiene un abono en una fecha distinta a la de la compra.";
                        }
                    }
                }
            }

            return (true, mensaje);
        }

        public async Task<(string DocumentoReferenciaId, int? AbonoId)> GetDatosDocumentoReferencia(string id)
        {
            var splitId = SplitId(id);
            string query = "SELECT Com_TipoDocMod, Com_AboItem FROM Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<(string documentoReferenciaId, int? abonoId)>(query, new
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

        public async Task ActualizarEstadoOrdenCompraRelacionado(string id)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = "SELECT Com_TipoComp FROM Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

                var ordenCompraId = await db.QueryFirstOrDefaultAsync<string>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });

                if (!string.IsNullOrWhiteSpace(ordenCompraId))
                {
                    splitId = dOrdenCompra.SplitId(ordenCompraId);
                    query = "UPDATE Compra SET Com_Facturado = 'N' WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

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
        }

        public async Task ActualizarCampoDocumentoReferenciaAbonoId(string id, int abonoId)
        {
            var splitId = SplitId(id);

            string query = @"   UPDATE Compra SET Com_AboItem = @abonoId WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    abonoId,
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });
            }
        }

        public async Task ActualizarCampoTipoCompraId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   UPDATE Compra SET Com_TCompra = 'CR', Com_Cancelado = 'N' WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId 
                                AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId AND Com_TCompra = 'CO' AND Com_Saldo > 0";

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

        public async Task DeshacerCompra(string id)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = "SELECT Com_TipoComp FROM Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

                var ordenCompraId = await db.QueryFirstOrDefaultAsync<string>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });

                if (!string.IsNullOrWhiteSpace(ordenCompraId))
                {
                    dOrdenCompra dOrdenCompra = new(_connectionString);
                    var detalles = await new dDocumentoCompraDetalle(_connectionString).ListarPorDocumentoCompra(id);

                    foreach (var detalle in detalles)
                    {
                        await dOrdenCompra.ActualizarCantidadEntrada(ordenCompraId, detalle.Id, detalle.Cantidad, Operacion.Aumentar);
                    }
                }
            }
        }

        public static oSplitDocumentoCompraId SplitId(string id) => new(id);
        #endregion
    }
}
