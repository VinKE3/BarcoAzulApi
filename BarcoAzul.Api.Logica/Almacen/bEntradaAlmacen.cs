using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Almacen;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Almacen
{
    public class bEntradaAlmacen : bComun
    {
        public bEntradaAlmacen(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Entrada Almacén", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(EntradaAlmacenDTO model)
        {
            try
            {
                dEntradaAlmacen dEntradaAlmacen = new(GetConnectionString());

                var entradaAlmacen = Mapping.Mapper.Map<oEntradaAlmacen>(model);
                entradaAlmacen.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                entradaAlmacen.TipoDocumentoId = model.TipoDocumentoId = dEntradaAlmacen.TipoDocumentoId;
                entradaAlmacen.Numero = model.Numero = await dEntradaAlmacen.GetNuevoNumero(entradaAlmacen.EmpresaId, entradaAlmacen.Serie);
                entradaAlmacen.ClienteId = model.ClienteId = _configuracionGlobal.DefaultClienteId;
                entradaAlmacen.UsuarioId = _datosUsuario.Id;

                entradaAlmacen.ProcesarDatos();
                entradaAlmacen.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dEntradaAlmacen.Registrar(entradaAlmacen);

                    dEntradaAlmacenDetalle dEntradaAlmacenDetalle = new(GetConnectionString());
                    await dEntradaAlmacenDetalle.Registrar(entradaAlmacen.Detalles);

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

        public async Task<bool> Modificar(EntradaAlmacenDTO model)
        {
            try
            {
                var entradaAlmacen = Mapping.Mapper.Map<oEntradaAlmacen>(model);

                entradaAlmacen.UsuarioId = _datosUsuario.Id;
                entradaAlmacen.ProcesarDatos();
                entradaAlmacen.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dEntradaAlmacen dEntradaAlmacen = new(GetConnectionString());
                    await dEntradaAlmacen.Modificar(entradaAlmacen);

                    dEntradaAlmacenDetalle dEntradaAlmacenDetalle = new(GetConnectionString());
                    await dEntradaAlmacenDetalle.Modificar(entradaAlmacen.Detalles);

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
                    dAbonoCompra dAbonoCompra = new(GetConnectionString());
                    await dAbonoCompra.Eliminar(id);

                    dEntradaAlmacenDetalle dEntradaAlmacenDetalle = new(GetConnectionString());
                    await dEntradaAlmacenDetalle.EliminarDeEntradaAlmacen(id);

                    dEntradaAlmacen dEntradaAlmacen = new(GetConnectionString());
                    await dEntradaAlmacen.Eliminar(id);

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

        public async Task<bool> Anular(string id)
        {
            try
            {
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dEntradaAlmacen dEntradaAlmacen = new(GetConnectionString());
                    await dEntradaAlmacen.Anular(id);

                    dEntradaAlmacenDetalle dEntradaAlmacenDetalle = new(GetConnectionString());
                    await dEntradaAlmacenDetalle.AnularDeEntradaAlmacen(id);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Anular);
                return false;
            }
        }

        public async Task<bool> Cerrar(string id)
        {
            try
            {
                dEntradaAlmacen dEntradaAlmacen = new(GetConnectionString());
                await dEntradaAlmacen.ActualizarEstadoCancelado(id, true);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<oEntradaAlmacen> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dEntradaAlmacen dEntradaAlmacen = new(GetConnectionString());
                var entradaAlmacen = await dEntradaAlmacen.GetPorId(id);

                dEntradaAlmacenDetalle dEntradaAlmacenDetalle = new(GetConnectionString());
                entradaAlmacen.Detalles = (await dEntradaAlmacenDetalle.ListarPorEntradaAlmacen(entradaAlmacen.Id)).ToList();

                if (incluirReferencias)
                {
                    entradaAlmacen.Proveedor = await new dProveedor(GetConnectionString()).GetPorId(id);
                    entradaAlmacen.Moneda = dMoneda.GetPorId(id);
                    entradaAlmacen.Personal = await new dPersonal(GetConnectionString()).GetPorId(id);
                }

                return entradaAlmacen;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vEntradaAlmacen>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string observacion, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dEntradaAlmacen dEntradaAlmacen = new(GetConnectionString());
                return await dEntradaAlmacen.Listar(fechaInicio.Value, fechaFin.Value, observacion ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dEntradaAlmacen(GetConnectionString()).Existe(id);

        public async Task<(bool IsBloqueado, string Mensaje)> IsBloqueado(string id) => await new dEntradaAlmacen(GetConnectionString()).IsBloqueado(id);

        public async Task<object> FormularioTablas()
        {
            var personal = await new dPersonal(GetConnectionString()).ListarTodos();
            var monedas = dMoneda.ListarTodos();
            var motivos = await new dMotivoTraslado(GetConnectionString()).ListarTodos();
            var serie = "0001";

            return new
            {
                personal,
                monedas,
                motivos,
                serie
            };
        }
    }
}
