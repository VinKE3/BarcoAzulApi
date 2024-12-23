using BarcoAzul.Api.Modelos.Entidades;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Compra
{
    public class dOrdenCompraDetalle : dComun
    {
        public dOrdenCompraDetalle(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oOrdenCompraDetalle> detalles)
        {
            string query = @"   INSERT INTO Detalle_Compra(Conf_Codigo, Prov_Codigo, TDoc_Codigo, Com_Serie, Com_Numero, Com_Item, Suc_Codigo, DCom_AfectarStock,
                                DCom_CtrlStock, DCom_Fecha, Lin_Codigo, SubL_Codigo, Art_Codigo, DCom_Descripcion, Uni_Codigo, DCom_Moneda, DCom_Cantidad, DCom_Precio,
                                DCom_PorcDscto, DCom_Descuento, DCom_PrecioNeto, DCom_PorcIgv, DCom_MontoIgv, DCom_Inafecto, DCom_Importe, DCom_TotalPeso, DCom_CstoMinTra,
                                DCom_Flat01, DCom_Falt02, Mar_Codigo, DCom_Turno, DCom_CodPtoCompra, DCom_CierreZ, DCom_CierreX, DArt_Codigo, Cli_Codigo, DCom_PercepCompra, 
                                DCom_Presentacion, DCom_CantidadEntr)
                                VALUES
                                (@EmpresaId, @ProveedorId, @TipoDocumentoId, @Serie, @Numero, @DetalleId, '01', 'N',
                                '+', @FechaEmision, @LineaId, @SubLineaId, @ArticuloId, @Descripcion, @UnidadMedidaId, @MonedaId, @Cantidad, @PrecioUnitario,
                                0, 0, @PrecioNeto, 0, 0, 0, @Importe, 0, 0,
                                0, 0, @MarcaId, '', '', 'N', 'N', @CodigoBarras, @ClienteId, 'N',
                                @Presentacion, @CantidadPendiente)";

            using (var db = GetConnection())
            {
                foreach (var detalle in detalles)
                {
                    await db.ExecuteAsync(query, new
                    {
                        detalle.EmpresaId,
                        detalle.ProveedorId,
                        detalle.TipoDocumentoId,
                        detalle.Serie,
                        detalle.Numero,
                        detalle.DetalleId,
                        detalle.FechaEmision,
                        detalle.LineaId,
                        detalle.SubLineaId,
                        detalle.ArticuloId,
                        detalle.Descripcion,
                        detalle.UnidadMedidaId,
                        detalle.MonedaId,
                        detalle.Cantidad,
                        detalle.PrecioUnitario,
                        detalle.PrecioNeto,
                        detalle.Importe,
                        detalle.MarcaId,
                        detalle.CodigoBarras,
                        detalle.ClienteId,
                        detalle.Presentacion,
                        detalle.CantidadPendiente
                    });
                }
            }
        }

        public async Task Modificar(IEnumerable<oOrdenCompraDetalle> detalles)
        {
            string ordenCompraId = detalles.First().OrdenCompraId;

            await EliminarDeOrdenCompra(ordenCompraId);
            await Registrar(detalles);
        }

        public async Task EliminarDeOrdenCompra(string ordenCompraId)
        {
            var splitId = dOrdenCompra.SplitId(ordenCompraId);

            string query = @"   DELETE Detalle_Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
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
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oOrdenCompraDetalle>> ListarPorOrdenCompra(string ordenCompraId)
        {
            var splitId = dOrdenCompra.SplitId(ordenCompraId);

            string query = @"   SELECT
                                    DC.Com_Item AS DetalleId,
                                    DC.Lin_Codigo AS LineaId,
                                    DC.SubL_Codigo AS SubLineaId,
                                    DC.Art_Codigo AS ArticuloId,
                                    DC.Uni_Codigo AS UnidadMedidaId,
                                    DC.Mar_Codigo AS MarcaId,
                                    DC.DCom_Descripcion AS Descripcion,
                                    DC.DArt_Codigo AS CodigoBarras,
                                    DC.DCom_Cantidad AS Cantidad,
                                    DC.DCom_Precio AS PrecioUnitario,
                                    DC.DCom_Precio AS SubTotal,
                                    DC.DCom_MontoIgv AS MontoIGV,
                                    DC.DCom_Importe AS Importe,
                                    DC.DCom_Presentacion AS Presentacion,
                                    U.Uni_Nombre AS UnidadMedidaDescripcion,
                                    DC.DCom_CantidadEntr AS CantidadPendiente
                                FROM 
                                    Detalle_Compra DC
                                    INNER JOIN Unidad_Medida U ON DC.Uni_Codigo = U.Uni_Codigo
                                WHERE
                                    Conf_Codigo = @empresaId
									AND Prov_Codigo = @proveedorId
									AND TDoc_Codigo = @tipoDocumentoId
									AND Com_Serie = @serie
									AND Com_Numero = @numero
									AND Cli_Codigo = @clienteId
                                ORDER BY
                                    DetalleId";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oOrdenCompraDetalle>(query, new
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
    }
}
