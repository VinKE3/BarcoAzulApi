using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Venta
{
    public class dDocumentoVentaAnticipo : dComun
    {
        public dDocumentoVentaAnticipo(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oDocumentoVentaAnticipo> anticipos)
        {
            string query = @"   INSERT INTO Venta_Anticipo (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, CodVenta, Item, Fecha, Tdoc, Serie, 
                                Numero, Moneda, Total, TCambio, FechaReg, FechaMod, Usu_Crea, Usu_Modifica)
                                VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @DocumentoRelacionadoId, @AnticipoId, @FechaEmision, @AnticipoTipoDocumentoId, @AnticipoSerie, 
                                @AnticipoNumero, @MonedaId, @SubTotal, @TipoCambio, GETDATE(), NULL, @UsuarioId, NULL)";

            using (var db = GetConnection())
            {
                foreach (var anticipo in anticipos)
                {
                    var splitId = dDocumentoVenta.SplitId(anticipo.DocumentoRelacionadoId);

                    await db.ExecuteAsync(query, new
                    {
                        anticipo.EmpresaId,
                        anticipo.TipoDocumentoId,
                        anticipo.Serie,
                        anticipo.Numero,
                        anticipo.DocumentoRelacionadoId,
                        anticipo.AnticipoId,
                        anticipo.FechaEmision,
                        AnticipoTipoDocumentoId = splitId.TipoDocumentoId,
                        AnticipoSerie = splitId.Serie,
                        AnticipoNumero = splitId.Numero,
                        anticipo.MonedaId,
                        anticipo.SubTotal,
                        anticipo.TipoCambio,
                        anticipo.UsuarioId
                    });
                }
            }
        }

        public async Task Modificar(string documentoVentaId, IEnumerable<oDocumentoVentaAnticipo> detalles)
        {
            await EliminarDeDocumentoVenta(documentoVentaId);

            if (detalles is not null && detalles.Any())
                await Registrar(detalles);
        }

        public async Task EliminarDeDocumentoVenta(string documentoVentaId)
        {
            var splitId = dDocumentoVenta.SplitId(documentoVentaId);
            string query = @"DELETE Venta_Anticipo WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

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
        public async Task<IEnumerable<oDocumentoVentaAnticipo>> ListarPorDocumentoVenta(string documentoVentaId)
        {
            string query = @"   SELECT
	                                Item AS AnticipoId,
	                                CodVenta AS DocumentoRelacionadoId,
	                                Fecha AS FechaEmision,
	                                Moneda AS MonedaId,
	                                TCambio AS TipoCambio,
	                                Total AS SubTotal
                                FROM
	                                Venta_Anticipo
                                WHERE
	                                Conf_Codigo = @empresaId 
                                    AND TDoc_Codigo = @tipoDocumentoId 
                                    AND Ven_Serie = @serie 
                                    AND Ven_Numero = @numero";

            var splitId = dDocumentoVenta.SplitId(documentoVentaId);

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oDocumentoVentaAnticipo>(query, new
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
