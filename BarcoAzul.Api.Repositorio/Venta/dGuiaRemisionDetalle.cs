﻿using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Venta
{
    public class dGuiaRemisionDetalle : dComun
    {
        public dGuiaRemisionDetalle(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oGuiaRemisionDetalle> detalles)
        {
            string query = @"   INSERT INTO Detalle_Venta (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, DVen_Item, DVen_Fecha, Suc_Codigo, DVen_AfectarStock, Lin_Codigo,
                                SubL_Codigo, Art_Codigo, DVen_Descripcion, Uni_Codigo, DVen_Moneda, DVen_Cantidad, DVen_Precio, DVen_PorcDscto,
                                DVen_Descuento, DVen_PrecioNeto, DVen_PorcIgv, DVen_MontoIgv, DVen_Inafecto, DVen_Importe, DVen_Flat01, DVen_Flat02,
                                Mar_Codigo, Dven_CtrlStock, DVen_Turno, DVen_CodPtoVenta, DVen_CierreZ, DVen_CierreX,
                                DArt_Codigo, DVen_MontoICBPER, DVen_CantEnt, DVen_CstoMinTra, DVen_TotalPeso)
                                VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @DetalleId, @FechaEmision, '01', @AfectarStock, @LineaId,
                                @SubLineaId, @ArticuloId, @Descripcion, @UnidadMedidaId, @MonedaId, @Cantidad, @PrecioUnitario, 0,
                                0, @SubTotal, @PorcentajeIGV, @MontoIGV, 0, @Importe, 0, 0,
                                @MarcaId, '-', NULL, NULL, 'N', 'N',
                                @CodigoBarras, 0, @CantidadPendiente, @PesoUnitario, @TotalPeso)";

            using (var db = GetConnection())
            {
                foreach (var detalle in detalles)
                {
                    await db.ExecuteAsync(query, new
                    {
                        detalle.EmpresaId,
                        detalle.TipoDocumentoId,
                        detalle.Serie,
                        detalle.Numero,
                        detalle.DetalleId,
                        detalle.FechaEmision,
                        AfectarStock = detalle.AfectarStock ? "S" : "N",
                        detalle.LineaId,
                        detalle.SubLineaId,
                        detalle.ArticuloId,
                        detalle.Descripcion,
                        detalle.UnidadMedidaId,
                        detalle.MonedaId,
                        detalle.Cantidad,
                        detalle.PrecioUnitario,
                        detalle.PorcentajeIGV,
                        detalle.MontoIGV,
                        detalle.SubTotal,
                        detalle.Importe,
                        detalle.MarcaId,
                        detalle.CodigoBarras,
                        detalle.Costo,
                        detalle.PrecioCompra,
                        detalle.PesoUnitario,
                        detalle.TotalPeso,
                        detalle.CantidadPendiente
                    });
                }
            }
        }

        public async Task Modificar(IEnumerable<oGuiaRemisionDetalle> detalles)
        {
            string guiaRemisionId = detalles.First().GuiaRemisionId;

            await EliminarDeGuiaRemision(guiaRemisionId);
            await Registrar(detalles);
        }

        public async Task EliminarDeGuiaRemision(string guiaRemisionId)
        {
            var splitId = dGuiaRemision.SplitId(guiaRemisionId);
            string query = @"DELETE Detalle_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

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

        public async Task AnularDeGuiaRemision(string guiaRemisionId)
        {
            var splitId = dGuiaRemision.SplitId(guiaRemisionId);

            string query = @"   UPDATE Detalle_Venta SET DVen_AfectarStock = 'N'
                                WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

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
        public async Task<IEnumerable<oGuiaRemisionDetalle>> ListarPorGuiaRemision(string guiaRemisionId)
        {
            var splitId = dGuiaRemision.SplitId(guiaRemisionId);

            string query = @"   SELECT
	                                DV.DVen_Item AS DetalleId,
	                                DV.Lin_Codigo AS LineaId,
	                                DV.SubL_Codigo AS SubLineaId,
	                                DV.Art_Codigo AS ArticuloId,
	                                DV.Uni_Codigo AS UnidadMedidaId,
	                                DV.Mar_Codigo AS MarcaId,
	                                DV.DVen_Descripcion AS Descripcion,
	                                DV.DArt_Codigo AS CodigoBarras,
	                                DV.DVen_Cantidad AS Cantidad,
	                                DV.DVen_Precio AS PrecioUnitario,
	                                DV.Dven_PrecioNeto AS SubTotal,
	                                DV.DVen_MontoIgv AS MontoIgv,
	                                DV.DVen_Importe AS Importe,
                                    DV.DVen_CstoMinTra AS PesoUnitario,
                                    DV.DVen_TotalPeso AS TotalPeso,
	                                U.Uni_Nombre AS UnidadMedidaDescripcion,
	                                DV.DVen_CantEnt AS CantidadPendiente
                                FROM
                                    Detalle_Venta DV
                                    INNER JOIN Unidad_Medida U ON DV.Uni_Codigo = U.Uni_Codigo
                                WHERE 
                                    Conf_Codigo = @empresaId 
                                    AND TDoc_Codigo = @tipoDocumentoId
                                    AND Ven_Serie = @serie 
                                    AND Ven_Numero = @numero
                                ORDER BY
									DetalleId";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oGuiaRemisionDetalle>(query, new
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
