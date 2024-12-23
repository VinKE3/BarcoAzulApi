using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Finanzas
{
    public class dAbonoVenta : dComun
    {
        public dAbonoVenta(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oAbonoVenta abonoVenta)
        {
            string query = @"   INSERT INTO Abono_Venta (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Abo_Item, Abo_Fecha, Abo_Concepto, Abo_Moneda, Abo_TCambio, 
                                Abo_Monto, Abo_MontoDol, Abo_MontoSol, Abo_Bloquedo, Abo_Documento, Abo_FechaReg, Abo_FechaMod, Usu_Codigo, Abo_Turno, Abo_CodPtoVenta, 
                                Abo_CierreZ, Abo_CierreX, Abo_TPago, Abo_Hora, CC_Codigo, abo_recibonro, Abo_Planilla)
                                VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @AbonoId, @Fecha, @Concepto, @MonedaId, @TipoCambio,
                                @Monto, @MontoUSD, @MontoPEN, @IsBloqueado, @DocumentoVentaId, GETDATE(), NULL, @UsuarioId, 'N', '',
                                'N', 'N', @TipoCobroId, @Hora, @CuentaCorrienteId, @NumeroOperacion, NULL)";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    abonoVenta.EmpresaId,
                    abonoVenta.TipoDocumentoId,
                    abonoVenta.Serie,
                    abonoVenta.Numero,
                    abonoVenta.AbonoId,
                    abonoVenta.Fecha,
                    abonoVenta.Concepto,
                    abonoVenta.MonedaId,
                    abonoVenta.TipoCambio,
                    abonoVenta.Monto,
                    abonoVenta.MontoUSD,
                    abonoVenta.MontoPEN,
                    IsBloqueado = abonoVenta.IsBloqueado ? "S" : "N",
                    abonoVenta.DocumentoVentaId,
                    abonoVenta.UsuarioId,
                    abonoVenta.TipoCobroId,
                    abonoVenta.Hora,
                    abonoVenta.CuentaCorrienteId,
                    abonoVenta.NumeroOperacion
                });
            }
        }

        public async Task Eliminar(string id, int? abonoId = null)
        {
            var splitId = SplitId(id);
            string query = @"DELETE Abono_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            if (abonoId is not null)
                query += " AND Abo_Item = @abonoId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                    abonoId
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<int> GetNuevoId(string documentoVentaId)
        {
            var splitId = SplitId(documentoVentaId);
            string query = "SELECT ISNULL(MAX(Abo_Item), 0) FROM Abono_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                var nuevoId = await db.QueryFirstOrDefaultAsync<int?>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
                return nuevoId.HasValue ? nuevoId.Value + 1 : 1;
            }
        }

        public async Task<IEnumerable<oAbonoVenta>> ListarPorDocumentoVenta(string documentoVentaId)
        {
            var splitId = SplitId(documentoVentaId);

            string query = @"   SELECT
                                    Conf_Codigo AS EmpresaId,
                                    TDoc_Codigo AS TipoDocumentoId,
                                    Ven_Serie AS Serie,
                                    Ven_Numero AS Numero,
	                                Abo_Item AS AbonoId,
	                                Abo_Fecha AS Fecha,
	                                Abo_Concepto AS Concepto,
	                                Abo_Moneda AS MonedaId,
	                                Abo_TCambio AS TipoCambio,
	                                Abo_Monto AS Monto,
	                                Abo_MontoSol AS MontoPEN,
	                                Abo_MontoDol AS MontoUSD,
	                                Abo_TPago AS TipoCobroId, 
	                                CC_Codigo AS CuentaCorrienteId,
	                                abo_recibonro AS NumeroOperacion
                                FROM 
	                                Abono_Venta 
                                WHERE 
	                                Conf_Codigo = @empresaId
	                                AND TDoc_Codigo = @tipoDocumentoId
	                                AND Ven_Serie = @serie
	                                AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oAbonoVenta>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        public async Task<bool> TieneAbonos(string documentoVentaId)
        {
            var splitId = SplitId(documentoVentaId);
            string query = "SELECT COUNT(Conf_Codigo) FROM Abono_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                var abonos = await db.QueryFirstAsync<int>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });

                return abonos > 0;
            }
        }

        private static oSplitDocumentoVentaId SplitId(string id) => new(id);
        #endregion
    }
}
