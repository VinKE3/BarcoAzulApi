using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Finanzas
{
    public class dAbonoCompra : dComun
    {
        public dAbonoCompra(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oAbonoCompra abonoCompra)
        {
            string query = @"   INSERT INTO Abono_Compra (Conf_Codigo, Prov_Codigo, TDoc_Codigo, Com_Serie, Com_Numero, Abo_Item, Abo_Fecha,
                                Abo_Concepto, Abo_Moneda, Abo_TCambio, Abo_Monto, Abo_MontoDol, Abo_MontoSol, Abp_Bloqueado, Abo_Documento,
                                Abo_FechaReg, Abo_FechaMod, Usu_Codigo, Abo_Turno, Abo_CodPtoCompra, Abo_CierreZ, Abo_CierreX, Abo_TPago,
                                Abo_Hora, CC_Codigo, abo_observ, abo_recibonro) 
                                VALUES (@EmpresaId, @ProveedorId, @TipoDocumentoId, @Serie, @Numero, @AbonoId, @Fecha,
                                @Concepto, @MonedaId, @TipoCambio, @Monto, @MontoUSD, @MontoPEN, @IsBloqueado, @DocumentoCompraId, 
                                GETDATE(), NULL, @UsuarioId, '', '', 'N', 'N', @TipoPagoId,
                                @Hora, @CuentaCorrienteId, NULL, @NumeroOperacion)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    abonoCompra.EmpresaId,
                    abonoCompra.ProveedorId,
                    abonoCompra.TipoDocumentoId,
                    abonoCompra.Serie,
                    abonoCompra.Numero,
                    abonoCompra.AbonoId,
                    abonoCompra.Fecha,
                    abonoCompra.Concepto,
                    abonoCompra.MonedaId,
                    abonoCompra.TipoCambio,
                    abonoCompra.Monto,
                    abonoCompra.MontoUSD,
                    abonoCompra.MontoPEN,
                    IsBloqueado = abonoCompra.IsBloqueado ? "S" : "N",
                    abonoCompra.DocumentoCompraId,
                    abonoCompra.UsuarioId,
                    abonoCompra.TipoPagoId,
                    abonoCompra.Hora,
                    abonoCompra.CuentaCorrienteId,
                    abonoCompra.NumeroOperacion
                });
            }
        }

        public async Task Eliminar(string id, int? abonoId = null)
        {
            var splitId = SplitId(id);
            string query = @"DELETE Abono_Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero";

            if (abonoId is not null)
                query += " AND Abo_Item = @abonoId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    abonoId
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oAbonoCompra> GetPorId(string compraId, int abonoId)
        {
            var splitId = SplitId(compraId);

            string query = @"   SELECT
                                    Conf_Codigo AS EmpresaId,
                                    Prov_Codigo AS ProveedorId,
                                    TDoc_Codigo AS TipoDocumentoId,
                                    Com_Serie AS Serie,
                                    Com_Numero AS Numero,
                                    Abo_Item AS AbonoId,
                                    Abo_Fecha AS Fecha, 
                                    Abo_TPago AS TipoPagoId, 
                                    Abo_Concepto AS Concepto, 
                                    Abo_Moneda AS MonedaId, 
                                    Abo_TCambio AS TipoCambio, 
                                    Abo_Monto AS Monto,
                                    Abo_MontoSol AS MontoPEN,
                                    Abo_MontoDol AS MontoUSD,
                                    CC_Codigo AS CuentaCorrienteId,
                                    abo_recibonro AS NumeroOperacion,
                                    Abo_Documento AS DocumentoCompraId,
                                    CAST(CASE WHEN Abp_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado
                                FROM 
                                    Abono_Compra
                                WHERE 
                                    Conf_Codigo = @empresaId 
                                    AND Prov_Codigo = @proveedorId 
                                    AND TDoc_Codigo = @tipoDocumentoId 
                                    AND Com_Serie = @serie 
                                    AND Com_Numero = @numero
                                    AND Abo_Item = @abonoId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oAbonoCompra>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    abonoId
                });
            }
        }

        public async Task<IEnumerable<oAbonoCompra>> ListarPorDocumentoCompra(string documentoCompraId)
        {
            var splitId = SplitId(documentoCompraId);

            string query = @"   SELECT
                                    Conf_Codigo AS EmpresaId,
                                    Prov_Codigo AS ProveedorId,
                                    TDoc_Codigo AS TipoDocumentoId,
                                    Com_Serie AS Serie,
                                    Com_Numero AS Numero,
                                    Abo_Item AS AbonoId,
                                    Abo_Fecha AS Fecha, 
                                    Abo_TPago AS TipoPagoId, 
                                    Abo_Concepto AS Concepto, 
                                    Abo_Moneda AS MonedaId, 
                                    Abo_TCambio AS TipoCambio, 
                                    Abo_Monto AS Monto,
                                    Abo_MontoSol AS MontoPEN,
                                    Abo_MontoDol AS MontoUSD,
                                    CC_Codigo AS CuentaCorrienteId,
                                    abo_recibonro AS NumeroOperacion,
                                    Abo_Documento AS DocumentoCompraId,
                                    CAST(CASE WHEN Abp_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsBloqueado
                                FROM 
                                    Abono_Compra
                                WHERE 
                                    Conf_Codigo = @empresaId 
                                    AND Prov_Codigo = @proveedorId 
                                    AND TDoc_Codigo = @tipoDocumentoId 
                                    AND Com_Serie = @serie 
                                    AND Com_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oAbonoCompra>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<int> GetNuevoId(string documentoCompraId)
        {
            var splitId = SplitId(documentoCompraId);
            string query = @"   SELECT ISNULL(MAX(Abo_Item), 0) FROM Abono_Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Com_Serie = @serie AND Com_Numero = @numero";

            using (var db = GetConnection())
            {
                var nuevoId = await db.QueryFirstOrDefaultAsync<int?>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
                return nuevoId.HasValue ? nuevoId.Value + 1 : 1;
            }
        }

        public async Task<bool> Existe(string compraId, int abonoId)
        {
            var splitId = SplitId(compraId);

            string query = @"   SELECT COUNT(Conf_Codigo) FROM Abono_Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Com_Serie = @serie AND Com_Numero = @numero AND Abo_Item = @abonoId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    abonoId
                });
                return existe > 0;
            }
        }

        public async Task<bool> IsBloqueado(string compraId, int abonoId)
        {
            var splitId = SplitId(compraId);

            string query = @"   SELECT CAST(CASE WHEN Abp_Bloqueado = 'S' THEN 1 ELSE 0 END AS BIT) FROM Abono_Compra
                                WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Com_Serie = @serie AND Com_Numero = @numero AND Abo_Item = @abonoId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<bool>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    abonoId
                });
            }
        }

        private static oSplitDocumentoCompraId SplitId(string id) => new(id);
        #endregion
    }
}
