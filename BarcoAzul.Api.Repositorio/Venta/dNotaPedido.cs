using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;
using System.Data;

namespace BarcoAzul.Api.Repositorio.Venta
{
    public class dNotaPedido : dComun
    {
        public const string TipoDocumentoId = "NP";

        public dNotaPedido(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oNotaPedido notaPedido)
        {
            string query = @"	INSERT INTO Venta (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Ven_Fecha, Ven_Venci, Cli_Codigo, Ven_RazonSocial, Ven_RucDni, 
								Id_DireccionLlegada, Ven_DireccionLleg, Ven_CliTelefono, Dep_Codigo, Pro_Codigo, Dis_Codigo, Ven_CodContacto, Ven_nomcont, Ven_telefonocont, 
								Ven_emailcont, Car_Codigo, Ven_Cargo, Ven_celularcont, Per_Codigo, Ven_Moneda, Ven_TCambio, Ven_TVenta, Ven_TPago,
								Ven_TipoComp, Ven_CotAbonoBan01, Ven_CotValidez, Ven_Observ, Ven_SubTotal, Ven_MontoIgv, Ven_TotalNeto, Ven_Retencion, Ven_Percepcion, Ven_Total, 
								Ven_PorcIgv, Ven_PorcReten, Ven_PorcPercep, Ven_IncluyeIgv, Suc_Codigo, Ven_AfectarStock, Ven_IngEgrStock, Ven_Hora, Ven_PorcDscto, Ven_Descuento,
								Ven_ValorVenta, Ven_Otros, Ven_Inafecto, Ven_Abonado, Ven_Saldo, Ven_Anulado, Ven_Cancelado, Ven_ConIgv, Ven_Retenc, Ven_Percep, Ven_FechaReg,
								Usu_Codigo, Ven_BloqUsu, Ven_BloqSist, Ven_BloqProc, Ven_Bloqueado, Ven_Documento, Ven_CierreZ, Ven_CierreX, Ven_Facturado, Ven_Guia,
								Ven_FechaRecepcion, TipO_Codigo, Ven_AboItem, Ven_Abonar, Ven_Detraccion, Ven_PorcDetrac, Ven_EditCliente, Ven_Registrado, Ven_EditCargo, Ven_Transaccion,
								Ven_Costo, Ven_Utilidad, Ven_Anticipo)
								VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @FechaEmision, @FechaVencimiento, @ClienteId, @ClienteNombre, @ClienteNumeroDocumentoIdentidad, 
								@ClienteDireccionId, @ClienteDireccion, @ClienteTelefono, @DepartamentoId, @ProvinciaId, @DistritoId, @ContactoId, @ContactoNombre, @ContactoTelefono, 
								@ContactoCorreoElectronico, @ContactoCargoId, @ContactoCargoDescripcion, @ContactoCelular, @PersonalId, @MonedaId, @TipoCambio, @TipoVentaId, @TipoCobroId, 
								@NumeroOperacion, @CuentaCorrienteDescripcion, @Validez, @Observacion, @Subtotal, @MontoIGV, @TotalNeto, @MontoRetencion, @MontoPercepcion, @Total, 
								@PorcentajeIGV, @PorcentajeRetencion, @PorcentajePercepcion, @IncluyeIGV, '01', 'N', '-', @HoraEmision, 0, 0,
								@SubTotal, 0, 0, 0, @Total, 'N', 'N', 'S', 'N', 'N', GETDATE(),
								@UsuarioId, 'N', 'N', 'N', 'N', @NumeroDocumento, 'N', 'N', 'N', 'N',
								@FechaEmision, '01', 0, 'S', 0, 0, 'N', 'S', 'N', 'N',
								@CostoTotal, @UtilidadTotal, 'N')";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    notaPedido.EmpresaId,
                    notaPedido.TipoDocumentoId,
                    notaPedido.Serie,
                    notaPedido.Numero,
                    notaPedido.FechaEmision,
                    notaPedido.FechaVencimiento,
                    notaPedido.ClienteId,
                    notaPedido.ClienteNombre,
                    notaPedido.ClienteNumeroDocumentoIdentidad,
                    notaPedido.ClienteDireccionId,
                    notaPedido.ClienteDireccion,
                    notaPedido.ClienteTelefono,
                    notaPedido.DepartamentoId,
                    notaPedido.ProvinciaId,
                    notaPedido.DistritoId,
                    notaPedido.ContactoId,
                    notaPedido.ContactoNombre,
                    notaPedido.ContactoTelefono,
                    notaPedido.ContactoCorreoElectronico,
                    notaPedido.ContactoCargoId,
                    notaPedido.ContactoCargoDescripcion,
                    notaPedido.ContactoCelular,
                    notaPedido.PersonalId,
                    notaPedido.MonedaId,
                    notaPedido.TipoCambio,
                    notaPedido.TipoVentaId,
                    notaPedido.TipoCobroId,
                    notaPedido.NumeroOperacion,
                    notaPedido.CuentaCorrienteDescripcion,
                    notaPedido.Validez,
                    notaPedido.Observacion,
                    notaPedido.SubTotal,
                    notaPedido.MontoIGV,
                    notaPedido.TotalNeto,
                    notaPedido.MontoRetencion,
                    notaPedido.MontoPercepcion,
                    notaPedido.Total,
                    notaPedido.PorcentajeIGV,
                    notaPedido.PorcentajeRetencion,
                    notaPedido.PorcentajePercepcion,
                    IncluyeIGV = notaPedido.IncluyeIGV ? "S" : "N",
                    notaPedido.HoraEmision,
                    notaPedido.UsuarioId,
                    notaPedido.NumeroDocumento,
                    notaPedido.CostoTotal,
                    notaPedido.UtilidadTotal
                });
            }
        }

        public async Task Modificar(oNotaPedido notaPedido)
        {
            string query = @"   UPDATE Venta SET Ven_Fecha = @FechaEmision, Ven_Venci = @FechaVencimiento, Cli_Codigo = @ClienteId, Ven_RazonSocial = @ClienteNombre, 
                                Ven_RucDni = @ClienteNumeroDocumentoIdentidad, Id_DireccionLlegada = @ClienteDireccionId, Ven_DireccionLleg = @ClienteDireccion, 
                                Ven_CliTelefono = @ClienteTelefono, Dep_Codigo = @DepartamentoId, Pro_Codigo = @ProvinciaId, Dis_Codigo = @DistritoId, Ven_CodContacto = @ContactoId, 
                                Ven_nomcont = @ContactoNombre, Ven_telefonocont = @ContactoTelefono, Ven_emailcont = @ContactoCorreoElectronico, Car_Codigo = @ContactoCargoId, 
                                Ven_Cargo = @ContactoCargoDescripcion, Ven_celularcont = @ContactoCelular, Per_Codigo = @PersonalId, Ven_TCambio = @TipoCambio, Ven_TVenta = @TipoVentaId, 
                                Ven_TPago = @TipoCobroId, Ven_TipoComp = @NumeroOperacion, Ven_CotAbonoBan01 = @CuentaCorrienteDescripcion, Ven_CotValidez = @Validez, Ven_Observ = @Observacion, 
                                Ven_SubTotal = @Subtotal, Ven_MontoIgv = @MontoIGV, Ven_TotalNeto = @TotalNeto, Ven_Retencion = @MontoRetencion, Ven_Percepcion = @MontoPercepcion, 
                                Ven_Total = @Total, Ven_PorcIgv = @PorcentajeIGV, Ven_PorcReten = @PorcentajeRetencion, Ven_PorcPercep = @PorcentajePercepcion, Ven_IncluyeIgv = @IncluyeIGV,
                                Ven_Hora = @HoraEmision, Ven_ValorVenta = @SubTotal, Ven_Saldo = @Total, Usu_Codigo = @UsuarioId, Ven_FechaRecepcion = @FechaEmision, Ven_Costo = @CostoTotal, 
                                Ven_Utilidad = @UtilidadTotal, Ven_FechaMod = GETDATE() WHERE Conf_Codigo = @EmpresaId AND TDoc_Codigo = @TipoDocumentoId AND Ven_Serie = @Serie 
                                AND Ven_Numero = @Numero";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    notaPedido.FechaEmision,
                    notaPedido.FechaVencimiento,
                    notaPedido.ClienteId,
                    notaPedido.ClienteNombre,
                    notaPedido.ClienteNumeroDocumentoIdentidad,
                    notaPedido.ClienteDireccionId,
                    notaPedido.ClienteDireccion,
                    notaPedido.ClienteTelefono,
                    notaPedido.DepartamentoId,
                    notaPedido.ProvinciaId,
                    notaPedido.DistritoId,
                    notaPedido.ContactoId,
                    notaPedido.ContactoNombre,
                    notaPedido.ContactoTelefono,
                    notaPedido.ContactoCorreoElectronico,
                    notaPedido.ContactoCargoId,
                    notaPedido.ContactoCargoDescripcion,
                    notaPedido.ContactoCelular,
                    notaPedido.PersonalId,
                    notaPedido.TipoCambio,
                    notaPedido.TipoVentaId,
                    notaPedido.TipoCobroId,
                    notaPedido.NumeroOperacion,
                    notaPedido.CuentaCorrienteDescripcion,
                    notaPedido.Validez,
                    notaPedido.Observacion,
                    notaPedido.SubTotal,
                    notaPedido.MontoIGV,
                    notaPedido.TotalNeto,
                    notaPedido.MontoRetencion,
                    notaPedido.MontoPercepcion,
                    notaPedido.Total,
                    notaPedido.PorcentajeIGV,
                    notaPedido.PorcentajeRetencion,
                    notaPedido.PorcentajePercepcion,
                    IncluyeIGV = notaPedido.IncluyeIGV ? "S" : "N",
                    notaPedido.HoraEmision,
                    notaPedido.UsuarioId,
                    notaPedido.CostoTotal,
                    notaPedido.UtilidadTotal,
                    notaPedido.EmpresaId,
                    notaPedido.TipoDocumentoId,
                    notaPedido.Serie,
                    notaPedido.Numero
                });
            }
        }

        public async Task Eliminar(string id)
        {
            using (var db = GetConnection())
            {
                await db.ExecuteAsync("Sp_VentasEliminar", new { Codigo = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 18 } }, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 600);
            }
        }

        public async Task Anular(string id)
        {
            using (var db = GetConnection())
            {
                await db.ExecuteAsync("Sp_UpdateCantPendVenta", new
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
        public async Task<oNotaPedido> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"	SELECT
									V.Conf_Codigo AS EmpresaId,
									V.TDoc_Codigo AS TipoDocumentoId,
									V.Ven_Serie AS Serie,
									V.Ven_Numero AS Numero,
									V.Ven_Fecha AS FechaEmision,
									V.Ven_Venci AS FechaVencimiento,
									V.Cli_Codigo AS ClienteId,
									V.Ven_RazonSocial AS ClienteNombre,
									C.Cli_TipoDoc AS ClienteTipoDocumentoIdentidadId,
									V.Ven_RucDni AS ClienteNumeroDocumentoIdentidad,
									V.Id_DireccionLlegada AS ClienteDireccionId,
									V.Ven_DireccionLleg AS ClienteDireccion,
									V.Ven_CliTelefono AS ClienteTelefono,
									V.Dep_Codigo AS DepartamentoId,
									V.Pro_Codigo AS ProvinciaId,
									V.Dis_Codigo AS DistritoId,
									V.Ven_CodContacto AS ContactoId,
									V.Ven_nomcont AS ContactoNombre,
									V.Ven_telefonocont AS ContactoTelefono,
									V.Ven_emailcont AS ContactoCorreoElectronico,
									V.Car_Codigo AS ContactoCargoId,
									V.Ven_Cargo AS ContactoCargoDescripcion,
									V.Ven_celularcont AS ContactoCelular,
									V.Per_Codigo AS PersonalId,
									V.Ven_Moneda AS MonedaId,
									V.Ven_TCambio AS TipoCambio,
									V.Ven_TVenta AS TipoVentaId,
									V.Ven_TPago AS TipoCobroId,
									V.Ven_TipoComp AS NumeroOperacion,
									V.Ven_CotAbonoBan01 AS CuentaCorrienteDescripcion,
									V.Ven_CotValidez AS Validez,
									V.Ven_Observ AS Observacion,
									V.Ven_SubTotal AS Subtotal,
									V.Ven_MontoIgv AS MontoIGV,
									V.Ven_TotalNeto AS TotalNeto,
									V.Ven_Retencion AS MontoRetencion,
									V.Ven_Percepcion AS MontoPercepcion,
									V.Ven_Total AS Total,
									V.Ven_PorcIgv AS PorcentajeIGV,
									V.Ven_PorcReten AS PorcentajeRetencion,
									V.Ven_PorcPercep AS PorcentajePercepcion,
									CAST(CASE WHEN V.Ven_IncluyeIgv = 'S' THEN 1 ELSE 0 END AS BIT) AS IncluyeIGV
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
                return await db.QueryFirstOrDefaultAsync<oNotaPedido>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<oPagina<vNotaPedido>> Listar(DateTime fechaInicio, DateTime fechaFin, string clienteNombre, string personalId, oPaginacion paginacion)
        {
            string query = $@"	SELECT 
									Codigo AS Id,
									Fecha AS FechaEmision,
									Documento AS NumeroDocumento,
									Razon_Social AS ClienteNombre,
									Ruc_Dni AS ClienteNumero,
									Moneda AS MonedaId,
									Total,
									CAST(CASE WHEN Anulado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsAnulado,
									CAST(CASE WHEN Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado,
									CAST(CASE WHEN Ven_Facturado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsFacturado,
									Ven_DocFactur AS DocumentoReferencia,
									Personal AS PersonalNombre
								FROM
									v_lst_Venta
								WHERE
									TipoDoc = '{TipoDocumentoId}'
									AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
									{(string.IsNullOrWhiteSpace(personalId) ? string.Empty : "AND Per_Codigo = @personalId")}
									AND Razon_Social LIKE '%' + @clienteNombre + '%'
								ORDER BY
									Fecha DESC,
									Documento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vNotaPedido> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    personalId = new DbString { Value = personalId, IsAnsi = true, IsFixedLength = true, Length = 8 },
                    clienteNombre = new DbString { Value = clienteNombre, IsAnsi = true, IsFixedLength = false, Length = 250 }
                }))
                {
                    pagina = new oPagina<vNotaPedido>
                    {
                        Data = await result.ReadAsync<vNotaPedido>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
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

        public async Task<bool> IsFacturado(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT CAST(CASE WHEN Ven_Facturado = 'S' THEN 1 ELSE 0 END AS BIT) FROM Venta
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

        public async Task ActualizarDocumentoVentaRelacionado(string id, string documentoVentaId, string numeroDocumentoVentaId)
        {
            var splitId = SplitId(id);

            string query = @"   UPDATE Venta SET Ven_Facturado = 'S', Ven_DocFactur = @documentoVentaId, Ven_NroComp = @numeroDocumentoVentaId
                                WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    documentoVentaId = new DbString { Value = documentoVentaId, IsAnsi = true, IsFixedLength = false, Length = 20 },
                    numeroDocumentoVentaId = new DbString { Value = numeroDocumentoVentaId, IsAnsi = true, IsFixedLength = false, Length = 50 },
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
