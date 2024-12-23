using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Almacen
{
    public class dDetalleOperacion : dComun
    {
        public dDetalleOperacion(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oDetalleOperacion detalle) => await Registrar(new oDetalleOperacion[] { detalle });

        public async Task Registrar(IEnumerable<oDetalleOperacion> detalles)
        {
            string query = @"   INSERT INTO Detalle_Op (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, DVen_Item, SubL_Codigo, Lin_Codigo, Art_Codigo, DVen_Descripcion, Uni_Codigo, DVen_Cantidad)
                                VALUES(@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @DetalleId, @SubLineaId, @LineaId, @ArticuloId, @Descripcion, @UnidadMedidaId, @Cantidad)";

            using (var db = GetConnection())
            {
                foreach (var detalle in detalles)
                {
                    await db.ExecuteAsync(query, detalle);
                }
            }
        }

        public async Task Modificar(IEnumerable<oDetalleOperacion> detalles)
        {
            var ventaId = detalles.First().Id;

            await EliminarDeVenta(ventaId);
            await Registrar(detalles);
        }

        public async Task EliminarDeVenta(string ventaId)
        {
            var splitId = SplitId(ventaId);
            string query = @"DELETE Detalle_Op WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

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
        public async Task<IEnumerable<oDetalleOperacion>> ListarPorVenta(string ventaId)
        {
            var splitId = SplitId(ventaId);

            string query = @"   SELECT 
	                                Conf_Codigo AS EmpresaId,
	                                TDoc_Codigo AS TipoDocumentoId,
	                                Ven_Serie AS Serie,
	                                Ven_Numero AS Numero,
	                                DVen_Item AS DetalleId,
	                                Lin_Codigo AS LineaId,
	                                SubL_Codigo AS SubLineaId,
	                                Art_Codigo AS ArticuloId,
	                                DVen_Descripcion AS Descripcion,
	                                Uni_Codigo AS UnidadMedidaId,
	                                DVen_Cantidad AS Cantidad
                                FROM 
	                                Detalle_Op
                                WHERE
                                    Conf_Codigo = @empresaId
                                    AND TDoc_Codigo = @tipoDocumentoId
                                    AND Ven_Serie = @serie
                                    AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oDetalleOperacion>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        private static oSplitDocumentoVentaId SplitId(string id) => new(id);
        #endregion
    }
}
