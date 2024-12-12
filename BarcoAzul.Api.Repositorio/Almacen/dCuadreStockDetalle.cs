using BarcoAzul.Api.Modelos.Entidades;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Almacen
{
    public class dCuadreStockDetalle : dComun
    {
        public dCuadreStockDetalle(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oCuadreStockDetalle> detalles)
        {
            string query = @"   INSERT INTO Cuadrar_Stock (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Lin_Codigo, SubL_Codigo, Art_Codigo, Det_Int, Det_StockFinal, Det_Inventario, Det_Precio, 
                                Det_CantFalta, Det_TotalFalta, Det_CantSobra, Det_TotalSobra, Det_CantSaldo, Det_TotalSaldo, Mar_Codigo, Det_Marca, Det_Linea, Det_SubLinea, 
                                Det_Descripcion, Det_CodBarra, Det_Unidad, Uni_Codigo, TipE_Codigo)
                                VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @LineaId, @SubLineaId, @ArticuloId, @DetalleId, @StockFinal, @Inventario, @PrecioUnitario,
                                @CantidadFalta, @TotalFalta, @CantidadSobra, @TotalSobra, @CantidadSaldo, @TotalSaldo, @MarcaId, @MarcaNombre, @LineaDescripcion, @SubLineaDescripcion,
                                @Descripcion, @CodigoBarras, @UnidadMedidaDescripcion, @UnidadMedidaId, @TipoExistenciaId)";

            using (var db = GetConnection())
            {
                foreach (var detalle in detalles)
                {
                    await db.ExecuteAsync(query, detalle);
                }
            }
        }

        public async Task Modificar(IEnumerable<oCuadreStockDetalle> detalles)
        {
            string cuadreStockId = detalles.First().CuadreStockId;

            //Eliminarmos los detalles existentes para insertar los nuevos
            await EliminarDeCuadreStock(cuadreStockId);
            await Registrar(detalles);
        }

        public async Task EliminarDeCuadreStock(string cuadreStockId)
        {
            var splitId = dCuadreStock.SplitId(cuadreStockId);
            string query = @"DELETE Cuadrar_Stock WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

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
        public async Task<IEnumerable<oCuadreStockDetalle>> ListarPorCuadreStock(string cuadreStockId)
        {
            var splitId = dCuadreStock.SplitId(cuadreStockId);

            string query = @"   SELECT
                                    Lin_Codigo AS LineaId,
                                    SubL_Codigo AS SubLineaId,
                                    Art_Codigo AS ArticuloId,   
                                    Det_Int AS DetalleId,
                                    Det_StockFinal AS StockFinal, 
                                    Det_Inventario AS Inventario,
                                    Det_Precio AS PrecioUnitario,
                                    Det_CantFalta AS CantidadFalta,
                                    Det_TotalFalta AS TotalFalta,
                                    Det_CantSobra AS CantidadSobra,
                                    Det_TotalSobra AS TotalSobra,
                                    Det_CantSaldo AS CantidadSaldo,
                                    Det_TotalSaldo AS TotalSaldo, 
                                    Mar_Codigo AS MarcaId,
                                    Det_Marca AS MarcaNombre,   
                                    Det_Linea AS LineaDescripcion,
                                    Det_SubLinea AS SubLineaDescripcion,
                                    Det_Descripcion AS Descripcion,
                                    Det_CodBarra AS CodigoBarras,
                                    Det_Unidad AS UnidadMedidaDescripcion,
                                    TipE_Codigo AS TipoExistenciaId,
                                    Uni_Codigo AS UnidadMedidaId
                                FROM 
	                                Cuadrar_Stock
                                WHERE 
	                                Conf_Codigo = @empresaId 
                                    AND TDoc_Codigo = @tipoDocumentoId 
                                    AND Ven_Serie = @serie 
                                    AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oCuadreStockDetalle>(query, new
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
