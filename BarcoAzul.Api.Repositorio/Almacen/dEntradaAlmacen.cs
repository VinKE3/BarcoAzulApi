using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Almacen
{
    public class dEntradaAlmacen : dComun
    {
        public const string TipoDocumentoId = "EN";

        public dEntradaAlmacen(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oEntradaAlmacen entradaAlmacen)
        {
            string query = @"   INSERT INTO Compra (Conf_Codigo, Prov_Codigo, TDoc_Codigo, Com_Serie, Com_Numero, Cli_Codigo, Com_RucDni, Com_DireccionPart, Per_Codigo, 
                                Com_Fecha, Com_Moneda, Com_TCambio, Com_Observ, Suc_Codigo, Com_AfectarStock, Com_IngEgrStock, Com_TCompra, Com_TPago, Com_Hora,
                                Com_Venci, Com_PorcIgv, Com_SubTotal, Com_PorcDscto, Com_Descuento, Com_ValorVenta, Com_TotalNeto, Com_MontoIgv, Com_Otros, Com_PorcReten,
                                Com_Retencion, Com_PorcPercep, Com_Percepcion, Com_Inafecto, Com_Total, Com_Abonado, Com_Saldo, Com_ConIgv, Com_IncluyeIgv, Com_Retenc, Com_Percep,
                                Com_Anulado, Com_Cancelado, Com_Bloqueado, Com_FechaReg, Usu_Codigo, Com_SubTotalSol, Com_DescuentoSol, Com_ValorVentaSol, Com_TotalNetoSol,
                                Com_MontoIgvSol, Com_OtrosSol, Com_RetencionSol, Com_PercepcionSol, Com_InafectoSol, Com_TotalSol, Com_SubTotalDol, Com_DescuentoDol, Com_ValorVentaDol,
                                Com_TotalNetoDol, Com_MontoIgvDol, Com_OtrosDol, Com_RetencionDol, Com_PercepcionDol, Com_InafectoDol, Com_TotalDol, Com_BloqUsu, Com_BloqSist, Com_BloqProc,
                                Com_Documento, Com_FechaContable)
                                VALUES (@EmpresaId, @ProveedorId, @TipoDocumentoId, @Serie, @Numero, @ClienteId, @ProveedorNumeroDocumentoIdentidad, @ProveedorDireccion, @PersonalId, 
                                @FechaEmision, @MonedaId, @TipoCambio, @Observacion, '01', 'S', '+', 'CO', 'EF', @HoraEmision,
                                @FechaEmision, 0, @total, 0, 0, @total, @total, 0, 0, 0,
                                0, 0, 0, 0, @total, 0, @total, 'N', 'N', 'N', 'N',
                                'N', 'N', 'N', GETDATE(), @UsuarioId, @totalPEN, 0, @totalPEN, @totalPEN,
                                0, 0, 0, 0, 0, @totalPEN, @totalUSD, 0, @totalUSD,
                                @totalUSD, 0, 0, 0, 0, 0, @totalUSD, 'N', 'N', 'N',
                                @NumeroDocumento, @FechaEmision)";

            var total = entradaAlmacen.Detalles.Sum(x => x.Importe);
            decimal totalPEN = 0, totalUSD = 0;

            if (entradaAlmacen.MonedaId == "S")
            {
                totalPEN = total;
                totalUSD = decimal.Round(total / entradaAlmacen.TipoCambio, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                totalUSD = total;
                totalPEN = decimal.Round(total * entradaAlmacen.TipoCambio, 2, MidpointRounding.AwayFromZero);
            }

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    entradaAlmacen.EmpresaId,
                    entradaAlmacen.ProveedorId,
                    entradaAlmacen.TipoDocumentoId,
                    entradaAlmacen.Serie,
                    entradaAlmacen.Numero,
                    entradaAlmacen.ClienteId,
                    entradaAlmacen.ProveedorNumeroDocumentoIdentidad,
                    entradaAlmacen.ProveedorDireccion,
                    entradaAlmacen.PersonalId,
                    entradaAlmacen.FechaEmision,
                    entradaAlmacen.MonedaId,
                    entradaAlmacen.TipoCambio,
                    entradaAlmacen.NumeroOP,
                    entradaAlmacen.Observacion,
                    entradaAlmacen.HoraEmision,
                    total,
                    entradaAlmacen.UsuarioId,
                    totalPEN,
                    totalUSD,
                    entradaAlmacen.NumeroDocumento
                });
            }
        }

        public async Task Modificar(oEntradaAlmacen entradaAlmacen)
        {
            string query = @"   UPDATE Compra SET Com_RucDni = @ProveedorNumeroDocumentoIdentidad, Com_DireccionPart = @ProveedorDireccion, Per_Codigo = @PersonalId, Com_Fecha = @FechaEmision, 
                                Com_Moneda = @MonedaId, Com_TCambio = @TipoCambio, Com_Observ = @Observacion, Com_Venci = @FechaEmision, Com_SubTotal = @total,
                                Com_ValorVenta = @total, Com_TotalNeto = @total, Com_Total = @total, Com_Saldo = @total, Com_FechaMod = GETDATE(), Usu_Codigo = @UsuarioId, 
                                Com_SubTotalSol = @totalPEN, Com_ValorVentaSol = @totalPEN, Com_TotalNetoSol = @totalPEN, Com_TotalSol = @totalPEN, Com_SubTotalDol = @totalUSD, 
                                Com_ValorVentaDol = @totalUSD, Com_TotalNetoDol = @totalUSD, Com_TotalDol = @totalUSD, Com_FechaContable = @FechaEmision, Com_Hora = @HoraEmision,
                                Com_Abonado = 0 WHERE Conf_Codigo = @EmpresaId AND Prov_Codigo = @ProveedorId AND TDoc_Codigo = @TipoDocumentoId AND Com_Serie = @Serie 
                                AND Com_Numero = @Numero AND Cli_Codigo = @ClienteId";

            var total = entradaAlmacen.Detalles.Sum(x => x.Importe);
            decimal totalPEN = 0, totalUSD = 0;

            if (entradaAlmacen.MonedaId == "S")
            {
                totalPEN = total;
                totalUSD = decimal.Round(total / entradaAlmacen.TipoCambio, 2, MidpointRounding.AwayFromZero);
            }
            else
            {
                totalUSD = total;
                totalPEN = decimal.Round(total * entradaAlmacen.TipoCambio, 2, MidpointRounding.AwayFromZero);
            }

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    entradaAlmacen.ProveedorNumeroDocumentoIdentidad,
                    entradaAlmacen.ProveedorDireccion,
                    entradaAlmacen.PersonalId,
                    entradaAlmacen.FechaEmision,
                    entradaAlmacen.MonedaId,
                    entradaAlmacen.TipoCambio,
                    entradaAlmacen.NumeroOP,
                    entradaAlmacen.Observacion,
                    total,
                    entradaAlmacen.UsuarioId,
                    totalPEN,
                    totalUSD,
                    entradaAlmacen.HoraEmision,
                    entradaAlmacen.EmpresaId,
                    entradaAlmacen.ProveedorId,
                    entradaAlmacen.TipoDocumentoId,
                    entradaAlmacen.Serie,
                    entradaAlmacen.Numero,
                    entradaAlmacen.ClienteId
                });
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);

            string query = @"   DELETE Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

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

        public async Task Anular(string id)
        {
            var splitId = SplitId(id);

            string query = @"   UPDATE Compra SET Com_Anulado = 'S', Com_AfectarStock = 'N' WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId 
                                AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

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
        public async Task<oEntradaAlmacen> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT 
	                                C.Conf_Codigo AS EmpresaId,
	                                C.Prov_Codigo AS ProveedorId,
	                                C.TDoc_Codigo AS TipoDocumentoId,
	                                C.Com_Serie AS Serie,
	                                C.Com_Numero AS Numero,
	                                C.Cli_Codigo AS ClienteId,
	                                C.Com_RucDni AS ProveedorNumeroDocumentoIdentidad,
	                                RTRIM(P.Pro_RazonSocial) AS ProveedorNombre,
	                                C.Com_DireccionPart AS ProveedorDireccion,
	                                C.Per_Codigo AS PersonalId,
	                                C.Com_Fecha AS FechaEmision,
	                                C.Com_Moneda AS MonedaId,
	                                C.Com_TCambio AS TipoCambio,
	                                C.Com_Observ AS Observacion,
                                    C.TipO_Codigo AS MotivoId
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
                return await db.QueryFirstOrDefaultAsync<oEntradaAlmacen>(query, new
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

        public async Task<oPagina<vEntradaAlmacen>> Listar(DateTime fechaInicio, DateTime fechaFin, string observacion, oPaginacion paginacion)
        {
            string query = $@"	SELECT 
									Codigo AS Id,
									Fecha AS FechaEmision,
									Hora AS HoraEmision,
									TDocAbr + '-' + Serie + '-' + RIGHT(Numero, 8) AS NumeroDocumento,
									Com_Observ AS Observacion,
                                    Personal AS Personal,
									Moneda AS MonedaId,
                                    Com_DireccionPart AS Concepto,
                                    Total AS Total,
									CAST(CASE WHEN Cancelado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsCancelado,
									CAST(CASE WHEN Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado,
									CAST(CASE WHEN Anulado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsAnulado
								FROM 
									v_lst_compra
								WHERE 
									TipoDoc = '{TipoDocumentoId}'
									AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
									AND Com_Observ LIKE '%' + @observacion + '%'
								ORDER BY
									Fecha DESC, 
									NumeroDocumento DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vEntradaAlmacen> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    observacion = new DbString { Value = observacion, IsAnsi = true, IsFixedLength = false, Length = 250 }
                }))
                {
                    pagina = new oPagina<vEntradaAlmacen>
                    {
                        Data = await result.ReadAsync<vEntradaAlmacen>(),
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
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = @"   SELECT
                                        CAST(CASE WHEN Com_Cancelado = 'S' THEN 1 ELSE 0 END AS BIT),
                                        CAST(CASE WHEN Com_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT),
                                        CAST(CASE WHEN Com_Anulado = 'S' THEN 1 ELSE 0 END AS BIT)
                                    FROM 
                                        Compra
                                    WHERE 
                                        Conf_Codigo = @empresaId 
                                        AND Prov_Codigo = @proveedorId 
                                        AND TDoc_Codigo = @tipoDocumentoId 
                                        AND Com_Serie = @serie 
                                        AND Com_Numero = @numero 
                                        AND Cli_Codigo = @clienteId";

                var (isCerrado, isBloqueado, isAnulado) = await db.QueryFirstAsync<(bool IsCerrado, bool IsBloqueado, bool IsAnulado)>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    clienteId = new DbString { Value = splitId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 }
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

        public async Task<string> GetNuevoNumero(string empresaId, string serie)
        {
            return await GetNuevoId("SELECT MAX(Com_Numero) FROM Compra WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @TipoDocumentoId AND Com_Serie = @serie AND ISNUMERIC(Com_Numero) = 1", new
            {
                empresaId = new DbString { Value = empresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                TipoDocumentoId = new DbString { Value = TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                serie = new DbString { Value = serie, IsAnsi = true, IsFixedLength = true, Length = 4 }
            }, "000000000#");
        }

        public async Task ActualizarEstadoCancelado(string id, bool isCancelado)
        {
            var splitId = SplitId(id);

            string query = @"   UPDATE Compra SET Com_Cancelado = @isCancelado WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId 
                                AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    isCancelado = new DbString { Value = isCancelado ? "S" : "N", IsAnsi = true, IsFixedLength = true, Length = 1 },
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
