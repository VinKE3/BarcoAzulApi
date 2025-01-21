using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Utilidades.Extensiones;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Finanzas
{
    public class dMovimientoBancarioDetalle : dComun
    {
        public dMovimientoBancarioDetalle(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(string tipoMovimientoId, IEnumerable<oMovimientoBancarioDetalle> detalles)
        {
            if (detalles is null || !detalles.Any())
                return;

            string query = tipoMovimientoId == "IN" ?
                           @"  INSERT INTO DetLetraRetencion (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Det_Item, Det_VentaCod, Det_VentaComp, Det_AboCodigo, Det_Emision, 
                                Det_Concepto, Det_Moneda, Det_MontoPago, Det_TCambio, Det_PorcPercp, Det_AboPercep, Det_AfectarDeuda, Det_VentaTDoc, Det_VentaSerie, Det_VentaNumero, Det_DocRetencion)
                                VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @DetalleId, @DocumentoVentaCompraId, @DocumentoVentaCompraNumeroDocumento, @AbonoId, @DocumentoVentaCompraFechaEmision, 
                                @Concepto, @MonedaId, 0, @TipoCambio, 0, @Abono, 'S', @DocumentoVentaCompraTipoDocumentoId, @DocumentoVentaCompraSerie, @DocumentoVentaCompraNumero, 'N')" :
                           @"  INSERT INTO DetLetraPercepcion (Conf_Codigo, Prov_Codigo, TDoc_Codigo, Com_Serie, Com_Numero, Det_Item, Det_CompaCod, Det_CompraComp, Det_AboCodigo, Det_Emision, 
                                Det_Concepto, Det_Moneda, Det_MontoPago, Det_TCambio, Det_PorcPercep, Det_AboPercep, Det_AfectarDeuda, Det_CompraTDoc, Det_CompraSerie, Det_CompraNumero)
                                VALUES (@EmpresaId, @ProveedorId, @TipoDocumentoId, @Serie, @Numero, @DetalleId, @DocumentoVentaCompraId, @DocumentoVentaCompraNumeroDocumento, @AbonoId, @DocumentoVentaCompraFechaEmision, 
                                @Concepto, @MonedaId, 0, @TipoCambio, 0, @Abono, 'S', @DocumentoVentaCompraTipoDocumentoId, @DocumentoVentaCompraSerie, @DocumentoVentaCompraNumero)";

            using (var db = GetConnection())
            {
                foreach (var detalle in detalles)
                {
                    detalle.AbonoId = tipoMovimientoId == "IN"
                        ? await new dAbonoVenta(_connectionString).GetNuevoId(detalle.DocumentoVentaCompraId)
                        : await new dAbonoCompra(_connectionString).GetNuevoId(detalle.DocumentoVentaCompraId);

                    await db.ExecuteAsync(query, tipoMovimientoId == "IN" ?
                        (new
                        {
                            detalle.EmpresaId,
                            detalle.TipoDocumentoId,
                            detalle.Serie,
                            detalle.Numero,
                            detalle.DetalleId,
                            detalle.DocumentoVentaCompraId,
                            detalle.DocumentoVentaCompraNumeroDocumento,
                            detalle.AbonoId,
                            detalle.DocumentoVentaCompraFechaEmision,
                            detalle.Concepto,
                            detalle.MonedaId,
                            detalle.TipoCambio,
                            detalle.Abono,
                            detalle.DocumentoVentaCompraTipoDocumentoId,
                            detalle.DocumentoVentaCompraSerie,
                            detalle.DocumentoVentaCompraNumero
                        }) :
                        (new
                        {
                            detalle.EmpresaId,
                            detalle.ProveedorId,
                            detalle.TipoDocumentoId,
                            detalle.Serie,
                            detalle.Numero,
                            detalle.DetalleId,
                            DocumentoVentaCompraId = detalle.DocumentoVentaCompraId.Left(24),
                            detalle.DocumentoVentaCompraNumeroDocumento,
                            detalle.AbonoId,
                            detalle.DocumentoVentaCompraFechaEmision,
                            detalle.Concepto,
                            detalle.MonedaId,
                            detalle.TipoCambio,
                            detalle.Abono,
                            detalle.DocumentoVentaCompraTipoDocumentoId,
                            detalle.DocumentoVentaCompraSerie,
                            detalle.DocumentoVentaCompraNumero,
                            detalle.DocumentoRelacionado
                        }));
                }
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oMovimientoBancarioDetalle>> ListarPorMovimientoBancario(string tipoMovimientoId, string movimientoBancarioId)
        {
            string query = tipoMovimientoId == "IN"
                            ? @"SELECT
	                                Det_Item AS DetalleId,
	                                Det_VentaCod AS DocumentoVentaCompraId,
	                                Det_Emision AS DocumentoVentaCompraFechaEmision,
	                                Det_Concepto AS Concepto,
	                                Det_AboPercep AS Abono
                                FROM 
	                                DetLetraRetencion
                                WHERE
	                                Conf_Codigo + TDoc_Codigo + Ven_Serie + Ven_Numero = @movimientoBancarioId"
                            : @"SELECT
	                                Det_Item AS DetalleId,
	                                Det_CompaCod AS DocumentoVentaCompraId,
	                                Det_Emision AS DocumentoVentaCompraFechaEmision,
	                                Det_Concepto AS Concepto,
	                                Det_AboPercep AS Abono
                                FROM 
	                                DetLetraPercepcion
                                WHERE
	                                Conf_Codigo + Prov_Codigo + TDoc_Codigo + Com_Serie + Com_Numero = @movimientoBancarioId";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oMovimientoBancarioDetalle>(query, new { movimientoBancarioId });
            }
        }
        #endregion
    }
}
