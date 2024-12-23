using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;
using MCWebAPI.Modelos.Otros;

namespace BarcoAzul.Api.Repositorio.Compra
{
    public class dGuiaCompra : dComun
    {
        public const string TipoDocumentoId = "09";

        public dGuiaCompra(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oGuiaCompra guiaCompra)
        {
            string query = @"   INSERT INTO Compra (Conf_Codigo, Prov_Codigo, TDoc_Codigo, Com_Serie, Com_Numero, Cli_Codigo, Com_Fecha, Com_partViaNom, Com_DireccionPart, Com_CtaCbleNom, 
                                Com_CtaCtble, Com_partDepart, Com_partProvincia, Com_partDistrito, Com_llegViaNom, Com_DireccionLleg, Com_CCostoNom, 
                                Com_CCosto, Com_llegDepart, Com_llegProvincia, Com_llegDistrito, Com_RucDni, Com_Telefono, 
                                Tra_Codigo, Com_OtroMotivo, Com_CertifInscrip, Com_licenciaCond, Com_PlacaRodaje, TipO_Codigo, 
                                Com_GuiaRemision, Com_IngEgrStock, Com_Observ, Com_OrdCompra, Com_Moneda, Com_AfectarStock, Suc_Codigo, Com_TCompra, Com_TPago, Com_Hora, Com_Venci,
                                Per_Codigo, Com_TCambio, Com_ConIgv, Com_IncluyeIgv, Com_Retenc, Com_Percep, Com_Anulado, Com_Cancelado, Com_Bloqueado, Com_TTraslado, Com_FechaReg,
                                Usu_Codigo, Com_BloqUsu, Com_BloqSist, Com_BloqProc, Com_Documento, Com_FechaContable, Com_PorcIgv, Com_SubTotal, Com_PorcDscto, Com_Descuento,
                                Com_ValorVenta, Com_TotalNeto, Com_MontoIgv, Com_Otros, Com_PorcReten, Com_Retencion, Com_PorcPercep, Com_Percepcion, Com_Inafecto, Com_Total,
                                Com_Abonado, Com_Saldo, Com_SubTotalSol, Com_DescuentoSol, Com_ValorVentaSol, Com_TotalNetoSol, Com_MontoIgvSol, Com_OtrosSol, Com_RetencionSol,
                                Com_PercepcionSol, Com_InafectoSol, Com_TotalSol, Com_SubTotalDol, Com_DescuentoDol, Com_ValorVentaDol, Com_TotalNetoDol, Com_MontoIgvDol, Com_OtrosDol,
                                Com_RetencionDol, Com_PercepcionDol, Com_InafectoDol, Com_TotalDol, Com_TipoComp)
                                VALUES (@EmpresaId, @ProveedorId, @TipoDocumentoId, @Serie, @Numero, @ClienteId, @FechaEmision, @DireccionPartida, @DepartamentoPartidaId, @ProvinciaPartidaId, 
                                @DistritoPartidaId, @DepartamentoPartidaNombre, @ProvinciaPartidaNombre, @DistritoPartidaNombre, @DireccionLlegada, @DepartamentoLlegadaId, @ProvinciaLlegadaId, 
                                @DistritoLlegadaId, @DepartamentoLlegadaNombre, @ProvinciaLlegadaNombre, @DistritoLlegadaNombre, @ProveedorRUC, @ProveedorNumeroDocumentoIdentidad, 
                                @TransportistaId, @TransportistaNumeroDocumentoIdentidad, @TransportistaCertificadoInscripcion, @TransportistaLicenciaConducir, @MarcaPlaca, @MotivoTrasladoId, 
                                @MotivoTrasladoSustento, @IngresoEgresoStock, @Observacion, @DocumentoReferencia, @MonedaId, @AfectarStock, '01', 'CO', 'EF', @HoraEmision, @FechaEmision,
                                @PersonalId, @TipoCambio, 'S', 'S', 'N', 'N', 'N', 'N', 'S', '01', GETDATE(),
                                @UsuarioId, 'N', 'N', 'N', @NumeroDocumento, @FechaEmision, @PorcentajeIGV, @subTotal, 0, 0,
                                @subTotal, @subTotal, @montoIGV, 0, 0, 0, 0, 0, 0, @total,
                                0, @total, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, 0, 0, 0, 0, 0,
                                0, 0, 0, 0, @DocumentoReferenciaId)";

            decimal subTotal = guiaCompra.Detalles.Sum(x => x.SubTotal);
            decimal montoIGV = guiaCompra.Detalles.Sum(x => x.MontoIGV);
            decimal total = guiaCompra.Detalles.Sum(x => x.Importe);

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    guiaCompra.EmpresaId,
                    guiaCompra.ProveedorId,
                    guiaCompra.TipoDocumentoId,
                    guiaCompra.Serie,
                    guiaCompra.Numero,
                    guiaCompra.ClienteId,
                    guiaCompra.FechaEmision,
                    guiaCompra.DireccionPartida,
                    guiaCompra.DepartamentoPartidaId,
                    guiaCompra.ProvinciaPartidaId,
                    guiaCompra.DistritoPartidaId,
                    guiaCompra.DepartamentoPartidaNombre,
                    guiaCompra.ProvinciaPartidaNombre,
                    guiaCompra.DistritoPartidaNombre,
                    guiaCompra.DireccionLlegada,
                    guiaCompra.DepartamentoLlegadaId,
                    guiaCompra.ProvinciaLlegadaId,
                    guiaCompra.DistritoLlegadaId,
                    guiaCompra.DepartamentoLlegadaNombre,
                    guiaCompra.ProvinciaLlegadaNombre,
                    guiaCompra.DistritoLlegadaNombre,
                    guiaCompra.ProveedorRUC,
                    guiaCompra.ProveedorNumeroDocumentoIdentidad,
                    guiaCompra.TransportistaId,
                    guiaCompra.TransportistaNumeroDocumentoIdentidad,
                    guiaCompra.TransportistaCertificadoInscripcion,
                    guiaCompra.TransportistaLicenciaConducir,
                    guiaCompra.MarcaPlaca,
                    guiaCompra.MotivoTrasladoId,
                    guiaCompra.MotivoTrasladoSustento,
                    guiaCompra.IngresoEgresoStock,
                    guiaCompra.Observacion,
                    guiaCompra.DocumentoReferencia,
                    guiaCompra.MonedaId,
                    AfectarStock = guiaCompra.AfectarStock ? "S" : "N",
                    guiaCompra.HoraEmision,
                    guiaCompra.PersonalId,
                    guiaCompra.TipoCambio,
                    guiaCompra.UsuarioId,
                    guiaCompra.NumeroDocumento,
                    guiaCompra.PorcentajeIGV,
                    subTotal,
                    montoIGV,
                    total,
                    guiaCompra.DocumentoReferenciaId
                });
            }
        }

        public async Task Modificar(oGuiaCompra guiaCompra)
        {
            string query = @"   UPDATE Compra SET Com_Fecha = @FechaEmision, Com_partViaNom = @DireccionPartida, Com_DireccionPart = @DepartamentoPartidaId,
                                Com_CtaCbleNom = @ProvinciaPartidaId, Com_CtaCtble = @DistritoPartidaId, Com_partDepart = @DepartamentoPartidaNombre,
                                Com_partProvincia = @ProvinciaPartidaNombre, Com_partDistrito = @DistritoPartidaNombre, Com_llegViaNom = @DireccionLlegada,
                                Com_DireccionLleg = @DepartamentoLlegadaId, Com_CCostoNom = @ProvinciaLlegadaId, Com_CCosto = @DistritoLlegadaId,
                                Com_llegDepart = @DepartamentoLlegadaNombre, Com_llegProvincia = @ProvinciaLlegadaNombre, Com_llegDistrito = @DistritoLlegadaNombre,
                                Com_RucDni = @ProveedorRUC, Com_Telefono = @ProveedorNumeroDocumentoIdentidad, Tra_Codigo = @TransportistaId,
                                Com_OtroMotivo = @TransportistaNumeroDocumentoIdentidad, Com_CertifInscrip = @TransportistaCertificadoInscripcion,
                                Com_licenciaCond = @TransportistaLicenciaConducir, Com_PlacaRodaje = @MarcaPlaca, TipO_Codigo = @MotivoTrasladoId,
                                Com_GuiaRemision = @MotivoTrasladoSustento, Com_IngEgrStock = @IngresoEgresoStock, Com_Observ = @Observacion,
                                Com_OrdCompra = @DocumentoReferencia, Com_Moneda = @MonedaId, Com_AfectarStock = @AfectarStock, Com_Hora = @HoraEmision,
                                Com_Venci = @FechaEmision, Com_TCambio = @TipoCambio, Usu_Codigo = @UsuarioId, Com_FechaContable = @FechaEmision,
                                Com_Cancelado = 'N', Com_Abonado = 0, Com_Saldo = @total, Com_FechaMod = GETDATE(), Com_Bloqueado = 'S', Com_SubTotal = @subTotal,
                                Com_ValorVenta = @subTotal, Com_TotalNeto = @subTotal, Com_MontoIgv = @montoIGV, Com_Total = @total, Com_PorcIgv = @PorcentajeIGV,
                                Com_TipoComp = @DocumentoReferenciaId WHERE Conf_Codigo = @EmpresaId AND Prov_Codigo = @ProveedorId AND TDoc_Codigo = @TipoDocumentoId 
                                AND Com_Serie = @Serie AND Com_Numero = @Numero AND Cli_Codigo = @ClienteId";

            decimal subTotal = guiaCompra.Detalles.Sum(x => x.SubTotal);
            decimal montoIGV = guiaCompra.Detalles.Sum(x => x.MontoIGV);
            decimal total = guiaCompra.Detalles.Sum(x => x.Importe);

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    guiaCompra.FechaEmision,
                    guiaCompra.DireccionPartida,
                    guiaCompra.DepartamentoPartidaId,
                    guiaCompra.ProvinciaPartidaId,
                    guiaCompra.DistritoPartidaId,
                    guiaCompra.DepartamentoPartidaNombre,
                    guiaCompra.ProvinciaPartidaNombre,
                    guiaCompra.DistritoPartidaNombre,
                    guiaCompra.DireccionLlegada,
                    guiaCompra.DepartamentoLlegadaId,
                    guiaCompra.ProvinciaLlegadaId,
                    guiaCompra.DistritoLlegadaId,
                    guiaCompra.DepartamentoLlegadaNombre,
                    guiaCompra.ProvinciaLlegadaNombre,
                    guiaCompra.DistritoLlegadaNombre,
                    guiaCompra.ProveedorRUC,
                    guiaCompra.ProveedorNumeroDocumentoIdentidad,
                    guiaCompra.TransportistaId,
                    guiaCompra.TransportistaNumeroDocumentoIdentidad,
                    guiaCompra.TransportistaCertificadoInscripcion,
                    guiaCompra.TransportistaLicenciaConducir,
                    guiaCompra.MarcaPlaca,
                    guiaCompra.MotivoTrasladoId,
                    guiaCompra.MotivoTrasladoSustento,
                    guiaCompra.IngresoEgresoStock,
                    guiaCompra.Observacion,
                    guiaCompra.DocumentoReferencia,
                    guiaCompra.MonedaId,
                    AfectarStock = guiaCompra.AfectarStock ? "S" : "N",
                    guiaCompra.HoraEmision,
                    guiaCompra.PersonalId,
                    guiaCompra.TipoCambio,
                    guiaCompra.UsuarioId,
                    guiaCompra.NumeroDocumento,
                    guiaCompra.PorcentajeIGV,
                    guiaCompra.DocumentoReferenciaId,
                    subTotal,
                    montoIGV,
                    total,
                    guiaCompra.EmpresaId,
                    guiaCompra.ProveedorId,
                    guiaCompra.TipoDocumentoId,
                    guiaCompra.Serie,
                    guiaCompra.Numero,
                    guiaCompra.ClienteId,
                });
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);

            string query = @"   DELETE Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie 
                                AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

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
        public async Task<oGuiaCompra> GetPorId(string id)
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
									C.Com_partViaNom AS DireccionPartida,
									C.Com_DireccionPart AS DepartamentoPartidaId,
									C.Com_CtaCbleNom AS ProvinciaPartidaId,
									C.Com_CtaCtble AS DistritoPartidaId,
									C.Com_partDepart AS DepartamentoPartidaNombre,
									C.Com_partProvincia AS ProvinciaPartidaNombre,
									C.Com_partDistrito AS DistritoPartidaNombre,
									C.Com_llegViaNom AS DireccionLlegada,
									C.Com_DireccionLleg AS DepartamentoLlegadaId,
									C.Com_CCostoNom AS ProvinciaLlegadaId,
									C.Com_CCosto AS DistritoLlegadaId,
									C.Com_llegDepart AS DepartamentoLlegadaNombre,
									C.Com_llegProvincia AS ProvinciaLlegadaNombre,
									C.Com_llegDistrito AS DistritoLlegadaNombre,
									C.Com_RucDni AS ProveedorRUC,
									RTRIM(P.Pro_RazonSocial) AS ProveedorNombre,
									C.Com_Telefono AS ProveedorNumeroDocumentoIdentidad,
									C.Tra_Codigo AS TransportistaId,
									C.Com_OtroMotivo AS TransportistaNumeroDocumentoIdentidad,
									C.Com_CertifInscrip AS TransportistaCertificadoInscripcion,
									C.Com_licenciaCond AS TransportistaLicenciaConducir,
									C.Com_PlacaRodaje AS MarcaPlaca,
									C.TipO_Codigo AS MotivoTrasladoId,
									C.Com_GuiaRemision AS MotivoTrasladoSustento,
									C.Com_IngEgrStock AS IngresoEgresoStock,
									C.Com_Observ AS Observacion,
									C.Com_OrdCompra AS DocumentoReferencia,
									C.Com_TipoComp AS DocumentoReferenciaId,
									C.Com_Moneda AS MonedaId,
									C.Com_TCambio AS TipoCambio,
									CAST(CASE WHEN C.Com_AfectarStock = 'S' THEN 1 ELSE 0 END AS BIT) AS AfectarStock
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
                return await db.QueryFirstOrDefaultAsync<oGuiaCompra>(query, new
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

        public async Task<oPagina<vGuiaCompra>> Listar(DateTime fechaInicio, DateTime fechaFin, string proveedorNombre, oPaginacion paginacion)
        {
            string query = $@"   SELECT
	                                Codigo AS Id,
	                                Fecha AS FechaEmision,
	                                Emision AS FechaTraslado,
	                                Documento AS NumeroDocumento,
	                                Proveedor AS ProveedorNombre,
	                                CAST(CASE WHEN Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado,
	                                CAST(CASE WHEN AfectarStock = 'S' THEN 1 ELSE 0 END AS BIT) AS AfectarStock,
	                                Marca_Placa AS MarcaPlaca
                                FROM
	                                v_lst_compra
                                WHERE
	                                TipoDoc = '{TipoDocumentoId}'
	                                AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
                                    AND Proveedor LIKE '%' + @proveedorNombre + '%'
                                ORDER BY
                                    Fecha DESC,
                                    Documento DESC
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vGuiaCompra> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    proveedorNombre = new DbString { Value = proveedorNombre, IsAnsi = true, IsFixedLength = false, Length = 100 }
                }))
                {
                    pagina = new oPagina<vGuiaCompra>
                    {
                        Data = await result.ReadAsync<vGuiaCompra>(),
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

        public async Task ActualizarEstadoDocumentoCompraRelacionado(string id)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = "SELECT Com_TipoComp FROM Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

                var documentoCompraId = await db.QueryFirstOrDefaultAsync<string>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });

                if (!string.IsNullOrWhiteSpace(documentoCompraId))
                {
                    splitId = dDocumentoCompra.SplitId(documentoCompraId);
                    query = "UPDATE Compra SET Com_Facturado = 'N', Com_Cancelado = 'N', Com_GuiaRemision = '' WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

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

        public async Task ActualizarDocumentoCompraRelacionadoComoFacturado(string documentoReferenciaId)
        {
            var splitId = SplitId(documentoReferenciaId);

            string query = @"   SELECT 
                                    SUM(DCom_CantidadEntr)
                                FROM 
                                    Detalle_Compra
                                WHERE 
                                    Conf_Codigo = @empresaId 
                                    AND Prov_Codigo = @proveedorId 
                                    AND TDoc_Codigo = @tipoDocumentoId 
                                    AND Com_Serie = @serie 
                                    AND Com_Numero = @numero 
                                    AND Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                var pendiente = await db.QueryFirstOrDefaultAsync<int?>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                }) ?? 0;

                if (pendiente == 0)
                {
                    query = @"  UPDATE 
                                    Compra 
                                SET 
                                    Com_Facturado = 'S'  
                                WHERE 
                                    Conf_Codigo = @empresaId
                                    AND Prov_Codigo = @proveedorId
                                    AND TDoc_Codigo = @tipoDocumentoId
                                    AND Com_Serie = @serie
                                    AND Com_Numero = @numero
                                    AND Cli_Codigo = @clienteId";

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

        public async Task DeshacerCompra(string id)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = "SELECT Com_TipoComp FROM Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

                var documentoCompraId = await db.QueryFirstOrDefaultAsync<string>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
                });

                if (!string.IsNullOrWhiteSpace(documentoCompraId))
                {
                    dOrdenCompra dOrdenCompra = new(_connectionString);
                    var detalles = await new dGuiaCompraDetalle(_connectionString).ListarPorGuiaCompra(id);

                    foreach (var detalle in detalles)
                    {
                        await dOrdenCompra.ActualizarCantidadEntrada(documentoCompraId, detalle.Id, detalle.Cantidad, Operacion.Aumentar);
                    }
                }
            }
        }

        public static oSplitDocumentoCompraId SplitId(string id) => new(id);
        #endregion
    }
}
