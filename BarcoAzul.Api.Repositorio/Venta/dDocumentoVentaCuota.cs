using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Venta
{
    public class dDocumentoVentaCuota : dComun
    {
        public dDocumentoVentaCuota(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oDocumentoVentaCuota> cuotas)
        {
            string query = @"   INSERT INTO Cuota_Venta (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, CVen_Item, CVen_FechaPago, CVen_Monto)
                                VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @CuotaId, @FechaPago, @Monto)";

            using (var db = GetConnection())
            {
                foreach (var cuota in cuotas)
                {
                    await db.ExecuteAsync(query, cuota);
                }
            }
        }

        public async Task Modificar(string documentoVentaId, IEnumerable<oDocumentoVentaCuota> detalles)
        {
            await EliminarDeDocumentoVenta(documentoVentaId);

            if (detalles is not null && detalles.Any())
                await Registrar(detalles);
        }

        public async Task EliminarDeDocumentoVenta(string documentoVentaId)
        {
            var splitId = dDocumentoVenta.SplitId(documentoVentaId);
            string query = @"DELETE Cuota_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

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
        public async Task<IEnumerable<oDocumentoVentaCuota>> ListarPorDocumentoVenta(string documentoVentaId)
        {
            var splitId = dDocumentoVenta.SplitId(documentoVentaId);

            string query = @"   SELECT
                                    CVen_Item AS CuotaId,
                                    CVen_FechaPago AS FechaPago,
                                    CVen_Monto AS Monto
                                FROM 
                                    Cuota_Venta
                                WHERE 
                                    Conf_Codigo = @empresaId
									AND TDoc_Codigo = @tipoDocumentoId
									AND Ven_Serie = @serie
									AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oDocumentoVentaCuota>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }
        #endregion
    }
}
