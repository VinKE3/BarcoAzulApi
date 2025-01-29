using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Almacen;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Almacen
{
    public class bCuadreStock : bComun
    {
        public bCuadreStock(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Cuadre de Stock", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(CuadreStockDTO model)
        {
            try
            {
                dCuadreStock dCuadreStock = new(GetConnectionString());

                var cuadreStock = Mapping.Mapper.Map<oCuadreStock>(model);
                cuadreStock.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                cuadreStock.TipoDocumentoId = model.TipoDocumentoId = dCuadreStock.TipoDocumentoId;
                cuadreStock.Serie = model.Serie = "0001";
                cuadreStock.Numero = model.Numero = await dCuadreStock.GetNuevoNumero(model.EmpresaId, model.Serie);
                cuadreStock.UsuarioId = _datosUsuario.Id;
                cuadreStock.ClienteId = _configuracionGlobal.DefaultClienteId;
                cuadreStock.ProcesarDatos();
                cuadreStock.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dCuadreStock.Registrar(cuadreStock);

                    dCuadreStockDetalle dCuadreStockDetalle = new(GetConnectionString());
                    await dCuadreStockDetalle.Registrar(model.Detalles);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(CuadreStockDTO model)
        {
            try
            {
                var cuadreStock = Mapping.Mapper.Map<oCuadreStock>(model);
                cuadreStock.UsuarioId = _datosUsuario.Id;
                cuadreStock.ProcesarDatos();
                cuadreStock.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dCuadreStock dCuadreStock = new(GetConnectionString());
                    await dCuadreStock.Modificar(cuadreStock);

                    dCuadreStockDetalle dCuadreStockDetalle = new(GetConnectionString());
                    await dCuadreStockDetalle.Modificar(model.Detalles);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<bool> Eliminar(string id)
        {
            try
            {
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dCuadreStockDetalle dCuadreStockDetalle = new(GetConnectionString());
                    await dCuadreStockDetalle.EliminarDeCuadreStock(id);

                    dCuadreStock dCuadreStock = new(GetConnectionString());
                    await dCuadreStock.Eliminar(id);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<bool> AbrirCerrar(string id, bool estado)
        {
            try
            {
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dCuadreStock dCuadreStock = new(GetConnectionString());
                    await dCuadreStock.AbrirCerrar(id, estado);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<oCuadreStock> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dCuadreStock dCuadreStock = new(GetConnectionString());
                var cuadreStock = await dCuadreStock.GetPorId(id);

                if (incluirReferencias)
                {
                    cuadreStock.Moneda = dMoneda.GetPorId(cuadreStock.MonedaId);
                    cuadreStock.Responsable = await new dPersonal(GetConnectionString()).GetPorId(cuadreStock.ResponsableId);
                }

                return cuadreStock;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vCuadreStock>> Listar(DateTime? fechaInicio, DateTime? fechaFin, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dCuadreStock dCuadreStock = new(GetConnectionString());
                return await dCuadreStock.Listar(fechaInicio.Value, fechaFin.Value, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dCuadreStock(GetConnectionString()).Existe(id);

        public async Task<IEnumerable<oCuadreStockDetalle>> GetDetalles(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                {
                    dCuadreStock dCuadreStock = new(GetConnectionString());
                    var articulos = await dCuadreStock.GetArticulosParaCuadreStock();
                    var detalles = new List<oCuadreStockDetalle>();
                    int contador = 0;

                    foreach (var articulo in articulos)
                    {
                        contador++;

                        detalles.Add(new oCuadreStockDetalle
                        {
                            DetalleId = contador,
                            LineaId = articulo.LineaId,
                            SubLineaId = articulo.SubLineaId,
                            ArticuloId = articulo.ArticuloId,
                            MarcaId = articulo.MarcaId,
                            UnidadMedidaDescripcion = articulo.UnidadMedidaDescripcion,
                            MarcaNombre = articulo.MarcaNombre,
                            LineaDescripcion = articulo.LineaDescripcion,
                            SubLineaDescripcion = articulo.SubLineaDescripcion,
                            Descripcion = articulo.Descripcion,
                            CodigoBarras = articulo.CodigoBarras,
                            UnidadMedidaId = articulo.UnidadMedidaId,
                            StockFinal = articulo.Stock,
                            Inventario = articulo.Stock,
                            PrecioUnitario = articulo.PrecioCompra
                            //TipoExistenciaId = articulo.TipoExistenciaId
                        });
                    }

                    return detalles;
                }

                dCuadreStockDetalle dCuadreStockDetalle = new(GetConnectionString());
                return await dCuadreStockDetalle.ListarPorCuadreStock(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<bool> IsBloqueado(string id) => await new dCuadreStock(GetConnectionString()).IsBloqueado(id);

        public async Task<DateTime?> GetFechaUltimoCuadre() => await new dCuadreStock(GetConnectionString()).GetFechaUltimoCuadre();

        public async Task<DateTime?> GetFechaCuadre(string id) => await new dCuadreStock(GetConnectionString()).GetFechaCuadre(id);

        public async Task<bool> RecalcularStock(oRecalcularStock recalcularStock)
        {
            try
            {
                recalcularStock.EmpresaId = _configuracionGlobal.EmpresaId;

                dCuadreStock dCuadreStock = new(GetConnectionString());
                await dCuadreStock.RecalcularStock(recalcularStock);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return false;
            }
        }

        public async Task<object> FormularioTablas()
        {
            var monedas = dMoneda.ListarTodos();
            var vendedores = await new dPersonal(GetConnectionString()).ListarTodos();

            return new
            {
                monedas,
                vendedores
            };
        }

        public static oSplitDocumentoVentaId SplitId(string id) => dCuadreStock.SplitId(id);

        public new async Task<(bool Valido, string Mensaje)> VerificarPeriodoCerrado(DateTime fecha) => await base.VerificarPeriodoCerrado(fecha);
    }
}
