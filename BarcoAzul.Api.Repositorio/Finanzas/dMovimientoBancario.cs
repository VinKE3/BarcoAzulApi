using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;
using BarcoAzul.Api.Utilidades.Extensiones;
using System.Data.SqlClient;

namespace BarcoAzul.Api.Repositorio.Finanzas
{
    public class dMovimientoBancario : dComun
    {
        public const string TipoDocumentoId = "13";

        public dMovimientoBancario(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oMovimientoBancario movimientoBancario)
        {
            string query = @"  INSERT INTO MovCtaCte01 (Mov_Codigo, Conf_Codigo, CC_Codigo, Mov_Fecha, Mov_TipoOpe, Mov_TipoMov, Mov_Numero, Mov_concepto, Mov_Moneda, Mov_Tcambio,
                                Mov_Monto, Mov_PorcItf, Mov_Itf, Mov_Total, Mov_IdProvCli, Mov_Nombres, Mov_IdCpraVta, Mov_Detraccion, 
                                Mov_CierreCaja, Mov_CtaCtaDest, Mov_CtaDestino, Mov_concepto2)
                                VALUES (@Id, @EmpresaId, @CuentaCorrienteId, @FechaEmision, @TipoMovimientoId, @TipoOperacionId, @NumeroOperacion, @Concepto, @MonedaId, @TipoCambio,
                                @Monto, @PorcentajeITF, @MontoITF, @Total, @ClienteProveedorId, @ClienteProveedorNombre, @DocumentoVentaCompraId, @TieneDetraccion, 
                                @IsCierreCaja, @TieneCuentaDestino, @CuentaDestinoId, @DocumentoReferencia)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    movimientoBancario.Id,
                    movimientoBancario.EmpresaId,
                    movimientoBancario.CuentaCorrienteId,
                    movimientoBancario.FechaEmision,
                    movimientoBancario.TipoMovimientoId,
                    movimientoBancario.TipoOperacionId,
                    movimientoBancario.NumeroOperacion,
                    movimientoBancario.Concepto,
                    movimientoBancario.MonedaId,
                    movimientoBancario.TipoCambio,
                    movimientoBancario.Monto,
                    movimientoBancario.PorcentajeITF,
                    movimientoBancario.MontoITF,
                    movimientoBancario.Total,
                    movimientoBancario.ClienteProveedorId,
                    movimientoBancario.ClienteProveedorNombre,
                    movimientoBancario.DocumentoVentaCompraId,
                    TieneDetraccion = movimientoBancario.TieneDetraccion ? "S" : "N",
                    IsCierreCaja = movimientoBancario.IsCierreCaja ? "S" : "N",
                    TieneCuentaDestino = movimientoBancario.TieneCuentaDestino ? "S" : "N",
                    movimientoBancario.CuentaDestinoId,
                    movimientoBancario.DocumentoReferencia,
                });

                await RegistroInterno(db, movimientoBancario);
            }
        }

        public async Task Modificar(oMovimientoBancario movimientoBancario)
        {
            string query = @"   UPDATE MovCtaCte01
                                SET Mov_Fecha = @FechaEmision, Mov_TipoMov = @TipoOperacionId, Mov_Numero = @NumeroOperacion, Mov_Concepto = @Concepto, Mov_TCambio = @TipoCambio,
                                Mov_Monto = @Monto, Mov_PorcItf = @PorcentajeITF, Mov_Itf = @MontoITF, Mov_Total = @Total,  Mov_IdProvCli = @ClienteProveedorId,
                                Mov_Nombres = @ClienteProveedorNombre, Mov_IdCpraVta = @DocumentoVentaCompraId, Mov_Detraccion = @TieneDetraccion, Mov_CierreCaja = @IsCierreCaja, 
                                Mov_CtaCtaDest = @TieneCuentaDestino, Mov_CtaDestino = @CuentaDestinoId, Mov_concepto2 = @DocumentoReferencia
                                WHERE Mov_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    movimientoBancario.FechaEmision,
                    movimientoBancario.TipoOperacionId,
                    movimientoBancario.NumeroOperacion,
                    movimientoBancario.Concepto,
                    movimientoBancario.TipoCambio,
                    movimientoBancario.Monto,
                    movimientoBancario.PorcentajeITF,
                    movimientoBancario.MontoITF,
                    movimientoBancario.Total,
                    movimientoBancario.ClienteProveedorId,
                    movimientoBancario.ClienteProveedorNombre,
                    movimientoBancario.DocumentoVentaCompraId,
                    TieneDetraccion = movimientoBancario.TieneDetraccion ? "S" : "N",
                    IsCierreCaja = movimientoBancario.IsCierreCaja ? "S" : "N",
                    TieneCuentaDestino = movimientoBancario.TieneCuentaDestino ? "S" : "N",
                    movimientoBancario.CuentaDestinoId,
                    movimientoBancario.DocumentoReferencia,
                    movimientoBancario.Id
                });

                await RegistroInterno(db, movimientoBancario);
            }
        }

        public async Task Eliminar(string id)
        {
            string query1 = @"DELETE MovCtaCte01 WHERE Mov_Codigo = @id";
            string query2 = @"DELETE MovCtaCte01 WHERE Mov_IdOrigen = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query1, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 10 } });
                await db.ExecuteAsync(query2, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 10 } });
            }
        }

        private async Task RegistroInterno(SqlConnection db, oMovimientoBancario movimientoBancario)
        {
            if (movimientoBancario.Detalles is null || movimientoBancario.Detalles.Count == 0)
                return;

            string query = movimientoBancario.TipoMovimientoId == "IN" ?
                           @"  INSERT INTO Venta (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Suc_Codigo, Ven_Fecha, Cli_Codigo, Ven_Moneda, Ven_TCambio) VALUES
                                (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, '01', @FechaEmision, @ClienteId, @MonedaId, @TipoCambio)" :
                           @"  INSERT INTO Compra (Conf_Codigo, TDoc_Codigo, Prov_Codigo, Com_Serie, Com_Numero, Suc_Codigo, Com_Fecha, Cli_Codigo, Com_Moneda, Com_TCambio) VALUES
                                (@EmpresaId, @TipoDocumentoId, @ProveedorId, @Serie, @Numero, '01', @FechaEmision, @ClienteId, @MonedaId, @TipoCambio)";

            await db.ExecuteAsync(query, movimientoBancario.TipoMovimientoId == "IN" ?
                   (new
                   {
                       movimientoBancario.EmpresaId,
                       movimientoBancario.TipoDocumentoId,
                       Serie = movimientoBancario.Id.Right(4),
                       Numero = movimientoBancario.Id,
                       movimientoBancario.FechaEmision,
                       movimientoBancario.ClienteId,
                       movimientoBancario.MonedaId,
                       movimientoBancario.TipoCambio
                   }) :
                   (new
                   {
                       movimientoBancario.EmpresaId,
                       movimientoBancario.TipoDocumentoId,
                       movimientoBancario.ProveedorId,
                       Serie = movimientoBancario.Id.Right(4),
                       Numero = movimientoBancario.Id,
                       movimientoBancario.FechaEmision,
                       movimientoBancario.ClienteId,
                       movimientoBancario.MonedaId,
                       movimientoBancario.TipoCambio
                   }));

            movimientoBancario.DocumentoVentaCompraId = movimientoBancario.TipoMovimientoId == "IN"
                ? $"{movimientoBancario.EmpresaId}{movimientoBancario.TipoDocumentoId}{movimientoBancario.Id.Right(4)}{movimientoBancario.Id}"
                : $"{movimientoBancario.EmpresaId}{movimientoBancario.ProveedorId}{TipoDocumentoId}{movimientoBancario.Id.Right(4)}{movimientoBancario.Id}";

            query = "UPDATE MovCtaCte01 SET Mov_IdCpraVta = @DocumentoVentaCompraId WHERE Mov_Codigo = @Id";

            await db.ExecuteAsync(query, new
            {
                movimientoBancario.DocumentoVentaCompraId,
                movimientoBancario.Id
            });
        }
        #endregion

        #region Otros Métodos
        public async Task<oMovimientoBancario> GetPorId(string id)
        {
            string query = @"   SELECT
	                                Mov_Codigo AS Id,
	                                Conf_Codigo AS EmpresaId,
	                                CC_Codigo AS CuentaCorrienteId,
	                                Mov_Fecha AS FechaEmision,
	                                Mov_TipoOpe AS TipomovimientoId,
	                                Mov_TipoMov AS TipoOperacionId,
	                                Mov_Numero AS NumeroOperacion,
	                                Mov_concepto AS Concepto,
	                                Mov_Moneda AS MonedaId,
	                                Mov_Tcambio AS TipoCambio,
	                                Mov_Monto AS Monto,
	                                Mov_PorcItf AS PorcentajeITF,
	                                Mov_Itf AS MontoITF,
	                                Mov_Total AS Total,
	                                Mov_IdProvCli AS ClienteProveedorId,
	                                Mov_Nombres As ClienteProveedorNombre,
	                                Mov_IdCpraVta AS DocumentoVentaCompraId,
	                                CAST(CASE WHEN Mov_Detraccion = 'S' THEN 1 ELSE 0 END AS BIT) AS TieneDetraccion,
	                                CAST(CASE WHEN Mov_CierreCaja = 'S' THEN 1 ELSE 0 END AS BIT) AS IsCierreCaja,
	                                CAST(CASE WHEN Mov_CtaCtaDest = 'S' THEN 1 ELSE 0 END AS BIT) AS TieneCuentaDestino,
	                                Mov_CtaDestino AS CuentaDestinoId
                                FROM 
	                                MovCtaCte01
                                WHERE
	                                Mov_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oMovimientoBancario>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 10 } });
            }
        }

        public async Task<oPagina<vMovimientoBancario>> Listar(DateTime fechaInicio, DateTime fechaFin, string cuentaCorrienteId, string tipoMovimientoId, string concepto, oPaginacion paginacion)
        {
            string query = $@"	SELECT 
									MCC.Mov_Codigo AS Id,
									CC.CC_Numero AS CuentaBancaria,
									MCC.Mov_Fecha AS FechaEmision,
									MCC.Mov_TipoOpe AS TipoMovimientoId,
									MCC.Mov_TipoMov AS TipoOperacionId,
									MCC.Mov_Numero AS NumeroOperacion,
									MCC.Mov_Nombres AS ClienteProveedorNombre,
									MCC.Mov_concepto AS Concepto,
									MCC.Mov_Monto AS Monto,
									MCC.Mov_Itf AS ITF,
									MCC.Mov_Total AS Total,
									CAST(CASE WHEN MCC.Mov_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) as IsBloqueado
								FROM
									MovCtaCte01 MCC
									INNER JOIN Cuenta_Corriente CC ON MCC.Conf_Codigo = CC.Conf_Codigo AND MCC.CC_Codigo = CC.CC_Codigo
								WHERE 
									(MCC.Mov_Fecha BETWEEN @fechaInicio AND @fechaFin)
									{(string.IsNullOrWhiteSpace(tipoMovimientoId) ? string.Empty : "AND MCC.Mov_TipoOpe = @tipoMovimientoId")}
									{(string.IsNullOrWhiteSpace(cuentaCorrienteId) ? string.Empty : "AND MCC.CC_Codigo = @cuentacorrienteId")}
									AND MCC.Mov_concepto LIKE '%' + @concepto + '%'
								ORDER BY
									CC.CC_Numero,
									FechaEmision DESC,
									Id DESC
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vMovimientoBancario> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    fechaInicio,
                    fechaFin,
                    tipoMovimientoId = new DbString { Value = tipoMovimientoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    cuentaCorrienteId = new DbString { Value = cuentaCorrienteId, IsAnsi = true, IsFixedLength = false, Length = 2 },
                    concepto = new DbString { Value = concepto, IsAnsi = true, IsFixedLength = false, Length = 250 }
                }))
                {
                    pagina = new oPagina<vMovimientoBancario>
                    {
                        Data = await result.ReadAsync<vMovimientoBancario>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            string query = "SELECT COUNT(Mov_Codigo) FROM MovCtaCte01 WHERE Mov_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 10 } });
                return existe > 0;
            }
        }

        public async Task<bool> IsBloqueado(string id)
        {
            string query = @"SELECT CAST(CASE WHEN Mov_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) FROM MovCtaCte01 WHERE Mov_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<bool>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 10 } });
            }
        }

        public async Task<bool> IsModificable(string id)
        {
            string query = "SELECT Mov_IdCpraVta FROM MovCtaCte01 WHERE Mov_Codigo = @id";

            using (var db = GetConnection())
            {
                string registroRelacionado = await db.QueryFirstOrDefaultAsync<string>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 10 } });
                return string.IsNullOrWhiteSpace(registroRelacionado);
            }
        }

        public async Task<(bool TieneOtroOrigen, string CuentaCorrienteInfo)> TieneOtroOrigen(string id)
        {
            string query = "SELECT Mov_TReg FROM MovCtaCte01 WHERE Mov_Codigo = @id";

            using (var db = GetConnection())
            {
                string tipoRegistro = await db.QueryFirstOrDefaultAsync<string>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 10 } });

                if (tipoRegistro == "G")
                {
                    query = @"  SELECT
	                                CC.CC_Numero + ' | ' + EB.Ban_Nombre
                                FROM
	                                Cuenta_Corriente CC
	                                INNER JOIN Entidad_Bancaria EB ON CC.Ban_Codigo = EB.Ban_Codigo
	                                INNER JOIN MovCtaCte01 MCC ON CC.Conf_Codigo = MCC.Conf_Codigo AND CC.CC_Codigo = MCC.CC_Codigo
                                WHERE
	                                MCC.Mov_Codigo = (SELECT Mov_IdOrigen FROM MovCtaCte01 WHERE Mov_Codigo = @id)";

                    string cuentaCorrienteInfo = await db.QueryFirstOrDefaultAsync<string>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 10 } });

                    return (true, cuentaCorrienteInfo);
                }

                return (false, null);
            }
        }


        public async Task EjecutarStoredProcedureDestinoTransferencia(string id)
        {
            using (var db = GetConnection())
            {
                await db.ExecuteAsync("Sp_DestinoTranferencia", new { IdOrigen = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 10 } }, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 600);
            }
        }

        public async Task<string> GetNuevoNumero() => await GetNuevoId("SELECT MAX(Mov_Codigo) FROM MovCtaCte01", null, "000000000#");
        #endregion
    }
}
