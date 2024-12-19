using Dapper;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Utilidades;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dArticulo : dComun
    {
        public dArticulo(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(oArticulo articulo)
        {
            string query = @"INSERT INTO Articulo(Lin_Codigo, SubL_Codigo, Art_Codigo, Art_CodBarra, Art_Descripcion, Mar_Codigo, Uni_Codigo, 
                            Art_CtrlStock, Art_Stock01, Art_Stock02, Art_StockMin, Art_StockMax, Art_Moneda, Art_PCompra, Art_Utilidad01, 
                            Art_Utilidad02, Art_Utilidad03, Art_Utilidad04, Art_PVenta01, Art_PVenta02, Art_PVenta03, Art_PVenta04, Art_IncluyeIgv,
                            Art_TipoArt, Art_Imagen, Art_FechaReg, Usu_Codigo, Art_ActPCompra, Art_ConIgv, Art_UltCompra, Art_Derivado, Art_DerEquival, 
                            Art_Activo, Art_Peso, TipE_Codigo, Art_Detraccion, Art_PercepCompra, Art_Observ, Art_Exportado)
                            VALUES 
                            (@LineaId, @SubLineaId, @ArticuloId, @CodigoBarras, @Descripcion, @MarcaId, @UnidadMedidaId, 
                            @ControlarStock, 0, 0, @StockMinimo, 0, @MonedaId, @PrecioCompra, @PorcentajeUtilidad1, 
                            @PorcentajeUtilidad2, @PorcentajeUtilidad3, @PorcentajeUtilidad4, @PrecioVenta1, @PrecioVenta2, @PrecioVenta3, @PrecioVenta4, @PrecioIncluyeIGV,
                            'ME', '', GETDATE(), @UsuarioId, @ActualizarPrecioCompra, 'S', 0, 'N', 0,
                            @IsActivo, @Peso, @TipoExistenciaId, 'N', 'N', @Observacion, 'N')";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    articulo.LineaId,
                    articulo.SubLineaId,
                    articulo.ArticuloId,
                    articulo.CodigoBarras,
                    articulo.Descripcion,
                    articulo.MarcaId,
                    articulo.UnidadMedidaId,
                    ControlarStock = articulo.ControlarStock ? "S" : "N",
                    articulo.StockMinimo,
                    articulo.MonedaId,
                    articulo.PrecioCompra,
                    articulo.PorcentajeUtilidad1,
                    articulo.PorcentajeUtilidad2,
                    articulo.PorcentajeUtilidad3,
                    articulo.PorcentajeUtilidad4,
                    articulo.PrecioVenta1,
                    articulo.PrecioVenta2,
                    articulo.PrecioVenta3,
                    articulo.PrecioVenta4,
                    articulo.UsuarioId,
                    ActualizarPrecioCompra = articulo.ActualizarPrecioCompra ? "S" : "N",
                    IsActivo = articulo.IsActivo ? "S" : "N",
                    articulo.Peso,
                    articulo.TipoExistenciaId,
                    articulo.Observacion,
                    articulo.PrecioCompraDescuento,
                    PrecioIncluyeIGV = articulo.PrecioIncluyeIGV ? "S" : "N",
                    PercepcionCompra = articulo.PercepcionCompra ? "S" : "N",
                    Detraccion = articulo.Detraccion ? "S" : "N",
                });
            }
        }

        public async Task Modificar(oArticulo articulo)
        {
            string query = @"   UPDATE Articulo SET Art_CodBarra = @CodigoBarras, Art_Descripcion = @Descripcion, Mar_Codigo = @MarcaId, Uni_Codigo = @UnidadMedidaId,
                                Art_CtrlStock = @ControlarStock, Art_Moneda = @MonedaId, Art_PCompra = @PrecioCompra, Art_Utilidad01 = @PorcentajeUtilidad1, Art_Utilidad02 = @PorcentajeUtilidad2,
                                Art_Utilidad03 = @PorcentajeUtilidad3, Art_Utilidad04 = @PorcentajeUtilidad4, Art_PVenta01 = @PrecioVenta1, Art_PVenta02 = @PrecioVenta2, Art_PVenta03 = @PrecioVenta3,
                                Art_PVenta04 = @PrecioVenta4, Art_Observ = @Observacion, Art_FechaMod = GETDATE(), Usu_Codigo = @UsuarioId, Art_ActPCompra = @ActualizarPrecioCompra,
                                Art_Activo = @IsActivo, Art_Peso = @Peso, TipE_Codigo = @TipoExistenciaId, Art_StockMin = @StockMinimo, 
                                Art_IncluyeIgv = @PrecioIncluyeIGV WHERE Lin_Codigo = @LineaId AND SubL_Codigo = @SubLineaId AND Art_Codigo = @ArticuloId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    articulo.CodigoBarras,
                    articulo.Descripcion,
                    articulo.MarcaId,
                    articulo.UnidadMedidaId,
                    ControlarStock = articulo.ControlarStock ? "S" : "N",
                    articulo.MonedaId,
                    articulo.PrecioCompra,
                    articulo.PorcentajeUtilidad1,
                    articulo.PorcentajeUtilidad2,
                    articulo.PorcentajeUtilidad3,
                    articulo.PorcentajeUtilidad4,
                    articulo.PrecioVenta1,
                    articulo.PrecioVenta2,
                    articulo.PrecioVenta3,
                    articulo.PrecioVenta4,
                    articulo.Observacion,
                    articulo.UsuarioId,
                    ActualizarPrecioCompra = articulo.ActualizarPrecioCompra ? "S" : "N",
                    IsActivo = articulo.IsActivo ? "S" : "N",
                    articulo.Peso,
                    articulo.TipoExistenciaId,
                    articulo.StockMinimo,
                    articulo.PrecioCompraDescuento,
                    PrecioIncluyeIGV = articulo.PrecioIncluyeIGV ? "S" : "N",
                    PercepcionCompra = articulo.PercepcionCompra ? "S" : "N",
                    Detraccion = articulo.Detraccion ? "S" : "N",
                    articulo.LineaId,
                    articulo.SubLineaId,
                    articulo.ArticuloId
                });
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);

            using (var db = GetConnection())
            {
                string query = "DELETE Cuadrar_Stock WHERE Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId";

                await db.ExecuteAsync(query, new
                {
                    lineaId = new DbString { Value = splitId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    articuloId = new DbString { Value = splitId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                });

                query = "DELETE Articulo WHERE Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId";

                await db.ExecuteAsync(query, new
                {
                    lineaId = new DbString { Value = splitId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    articuloId = new DbString { Value = splitId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oArticulo> GetPorId(string id)
        {
            var splitId = SplitId(id);

            string query = @"   SELECT
                                    A.Lin_Codigo AS LineaId,
                                    A.SubL_Codigo AS SubLineaId,
                                    A.Art_Codigo AS ArticuloId,
                                    A.TipE_Codigo AS TipoExistenciaId,
                                    A.Uni_Codigo AS UnidadMedidaId,
                                    A.Mar_Codigo AS MarcaId,
                                    A.Art_Descripcion AS Descripcion,
                                    A.Art_Observ AS Observacion,
                                    A.Art_CodBarra AS CodigoBarras,
                                    A.Art_Peso AS Peso,
                                    A.Art_Moneda AS MonedaId,
                                    A.Art_Exportado AS Exportado,
                                    A.Art_PCompra AS PrecioCompra,
                                    A.Art_PVenta01 AS PrecioVenta1,
                                    A.Art_PVenta02 AS PrecioVenta2,
                                    A.Art_PVenta03 AS PrecioVenta3,
                                    A.Art_PVenta04 AS PrecioVenta4,
                                    A.Art_Utilidad01 AS PorcentajeUtilidad1,
                                    A.Art_Utilidad02 AS PorcentajeUtilidad2,
                                    A.Art_Utilidad03 AS PorcentajeUtilidad3,
                                    A.Art_Utilidad04 AS PorcentajeUtilidad4,
                                    A.Art_Stock01 As Stock,
	                                A.Art_StockMin AS StockMinimo,
	                                CAST(CASE WHEN A.Art_IncluyeIgv = 'S' THEN 1 ELSE 0 END AS BIT) AS PrecioIncluyeIGV,
	                                CAST(CASE WHEN A.Art_PercepCompra = 'S' THEN 1 ELSE 0 END AS BIT) AS PercepcionCompra,
                                    CAST(CASE WHEN A.Art_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo,
                                    CAST(CASE WHEN A.Art_CtrlStock = 'S' THEN 1 ELSE 0 END AS BIT) AS ControlarStock,
                                    CAST(CASE WHEN A.Art_ActPCompra = 'S' THEN 1 ELSE 0 END AS BIT) AS ActualizarPrecioCompra,
                                    CAST(CASE WHEN A.Art_Detraccion = 'S' THEN 1 ELSE 0 END AS BIT) AS Detraccion,
                                    UM.Uni_Nombre AS UnidadMedidaDescripcion
                                FROM 
                                    Articulo A
                                    INNER JOIN Unidad_Medida UM ON A.Uni_Codigo = UM.Uni_Codigo
                                WHERE 
                                    A.Lin_Codigo = @lineaId 
                                    AND A.SubL_Codigo = @subLineaId 
                                    AND A.Art_Codigo = @articuloId";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oArticulo>(query, new
                {
                    lineaId = new DbString { Value = splitId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    articuloId = new DbString { Value = splitId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                });
            }
        }

        public async Task<oPagina<vArticulo>> Listar(string codigoBarras, string descripcion, bool? isActivo, oPaginacion paginacion)
        {
            string query = $@"	SELECT
									Codigo AS Id,
									Codigo_Barra AS CodigoBarras,
									Marca AS MarcaNombre,
									Descripcion AS Descripcion,
									Unidad_Medida AS UnidadMedidaAbreviatura,
									Stock,
									Precio_Compra AS PrecioCompra,
									Precio_Venta AS PrecioVenta,
									Moneda AS MonedaId,
									CAST(CASE WHEN Contro_Stock = 'S' THEN 1 ELSE 0 END AS BIT) AS ControlarStock,
									CAST(CASE WHEN Actualizar_Precio = 'S' THEN 1 ELSE 0 END AS BIT) AS ActualizarPrecio,
                                    CAST(CASE WHEN Detraccion = 'S' THEN 1 ELSE 0 END AS BIT) AS Detraccion,
                                    CAST(CASE WHEN Percepcion_Compra = 'S' THEN 1 ELSE 0 END AS BIT) AS PercepcionCompra,
									CAST(CASE WHEN Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo
								FROM
									v_lst_articulo
								WHERE
									Codigo_Barra LIKE '%' + @codigoBarras + '%'
									AND Descripcion LIKE '%' + @descripcion + '%'
									{(isActivo is null ? string.Empty : "AND Activo = @isActivo")}
								ORDER BY
									Id
								{GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vArticulo> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new
                {
                    codigoBarras = new DbString { Value = codigoBarras, IsAnsi = true, IsFixedLength = false, Length = 20 },
                    descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 150 },
                    isActivo = new DbString { Value = isActivo is null || !isActivo.Value ? "N" : "S", IsAnsi = true, IsFixedLength = true, Length = 1 },
                }))
                {
                    pagina = new oPagina<vArticulo>
                    {
                        Data = await result.ReadAsync<vArticulo>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(string id)
        {
            var splitId = SplitId(id);
            string query = "SELECT COUNT(Lin_Codigo) FROM Articulo WHERE Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    lineaId = new DbString { Value = splitId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    articuloId = new DbString { Value = splitId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                });

                return existe > 0;
            }
        }

        public async Task<(bool Existe, string ValorRepetido)> DatosRepetidos(string id, string codigoBarras, string descripcion)
        {
            var splitId = id is null ? null : SplitId(id);

            using (var db = GetConnection())
            {
                string query = @$"SELECT COUNT(Lin_Codigo) FROM Articulo WHERE {(id is null ? string.Empty : "NOT(Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId) AND")} Art_CodBarra = @codigoBarras";

                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    lineaId = new DbString { Value = splitId?.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitId?.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    articuloId = new DbString { Value = splitId?.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 },
                    codigoBarras = new DbString { Value = codigoBarras, IsAnsi = true, IsFixedLength = false, Length = 20 }
                });

                if (existe > 0)
                {
                    return (true, "código de barras");
                }

                query = @$"SELECT COUNT(Lin_Codigo) FROM Articulo WHERE {(id is null ? string.Empty : "NOT(Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId) AND")} Art_Descripcion = @descripcion";

                existe = await db.QueryFirstAsync<int>(query, new
                {
                    lineaId = new DbString { Value = splitId?.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    subLineaId = new DbString { Value = splitId?.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    articuloId = new DbString { Value = splitId?.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 },
                    descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 150 }
                });

                if (existe > 0)
                {
                    return (true, "descripción");
                }

                return (false, null);
            }
        }

        public async Task ConsultarStock(IEnumerable<oArticuloValidarStock> articulos)
        {
            string query;

            using (var db = GetConnection())
            {
                foreach (var articulo in articulos)
                {
                    query = $"SELECT Art_Stock01 AS Stock, CAST(CASE WHEN Art_CtrlStock = 'S' THEN 1 ELSE 0 END AS BIT) AS ControlarStock FROM Articulo WHERE Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId";

                    var splitArticuloId = SplitId(articulo.Id);

                    var data = await db.QueryFirstAsync<(decimal Stock, bool ControlarStock)>(query, new
                    {
                        lineaId = new DbString { Value = splitArticuloId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        subLineaId = new DbString { Value = splitArticuloId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        articuloId = new DbString { Value = splitArticuloId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                    });

                    articulo.Stock = data.Stock;
                    articulo.ControlarStock = data.ControlarStock;

                    if (!articulo.ControlarStock)
                    {
                        articulo.Stock = 0;
                        continue;
                    }

                    decimal cantidad = 0;

                    if (articulo.IsIngreso)
                    {
                        if (Comun.IsCompraIdValido(articulo.DocumentoVentaCompraId))
                        {
                            var splitDocumentoCompraId = new oSplitDocumentoCompraId(articulo.DocumentoVentaCompraId);

                            query = @"  SELECT DCom_Cantidad FROM Detalle_Compra WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId 
                                        AND Com_Serie = @serie AND Com_Numero = @numero AND Cli_Codigo = @clienteId AND Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId 
                                        AND Art_Codigo = @articuloId AND DCom_AfectarStock = 'S'";

                            cantidad = await db.QueryFirstOrDefaultAsync<decimal?>(query, new
                            {
                                empresaId = new DbString { Value = splitDocumentoCompraId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                                proveedorId = new DbString { Value = splitDocumentoCompraId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                                tipoDocumentoId = new DbString { Value = splitDocumentoCompraId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                                serie = new DbString { Value = splitDocumentoCompraId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                                numero = new DbString { Value = splitDocumentoCompraId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                                clienteId = new DbString { Value = splitDocumentoCompraId.ClienteId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                                lineaId = new DbString { Value = splitArticuloId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                                subLineaId = new DbString { Value = splitArticuloId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                                articuloId = new DbString { Value = splitArticuloId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                            }) ?? 0;

                            cantidad *= -1;
                        }
                    }
                    else
                    {
                        if (Comun.IsVentaIdValido(articulo.DocumentoVentaCompraId))
                        {
                            var splitDocumentoVentaId = new oSplitDocumentoVentaId(articulo.DocumentoVentaCompraId);

                            query = @"  SELECT DVen_Cantidad FROM Detalle_Venta WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie 
                                        AND Ven_Numero = @numero AND Lin_Codigo = @lineaId AND SubL_Codigo = @subLineaId AND Art_Codigo = @articuloId AND DVen_AfectarStock = 'S'";

                            cantidad = await db.QueryFirstOrDefaultAsync<decimal?>(query, new
                            {
                                empresaId = new DbString { Value = splitDocumentoVentaId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                                tipoDocumentoId = new DbString { Value = splitDocumentoVentaId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                                serie = new DbString { Value = splitDocumentoVentaId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                                numero = new DbString { Value = splitDocumentoVentaId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 },
                                lineaId = new DbString { Value = splitArticuloId.LineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                                subLineaId = new DbString { Value = splitArticuloId.SubLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                                articuloId = new DbString { Value = splitArticuloId.ArticuloId, IsAnsi = true, IsFixedLength = false, Length = 4 }
                            }) ?? 0;
                        }
                    }

                    articulo.Stock += cantidad;
                }
            }
        }

        public async Task<string> GetNuevoId(string lineaId, string subLineaId)
        {
            return await GetNuevoId("SELECT MAX(Art_Codigo) FROM Articulo WHERE Lin_Codigo = @lineaId ANd SubL_Codigo = @subLineaId", new
            {
                lineaId = new DbString { Value = lineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                subLineaId = new DbString { Value = subLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 }
            }, "000#");
        }

        public static oSplitArticuloId SplitId(string id) => new(id);
        #endregion
    }
}
