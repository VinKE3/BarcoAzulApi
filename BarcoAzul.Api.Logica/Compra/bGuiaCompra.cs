using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Compra;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using MCWebAPI.Modelos.Otros;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Compra
{
    public class bGuiaCompra : bComun
    {
        public bGuiaCompra(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Guía de Compra", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(GuiaCompraDTO model)
        {
            try
            {
                var guiaCompra = Mapping.Mapper.Map<oGuiaCompra>(model);

                guiaCompra.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                guiaCompra.TipoDocumentoId = model.TipoDocumentoId = dGuiaCompra.TipoDocumentoId;
                guiaCompra.ClienteId = model.ClienteId = _configuracionGlobal.DefaultClienteId;
                guiaCompra.UsuarioId = _datosUsuario.Id;

                if (await Existe(guiaCompra.Id))
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el ID ingresado."));
                    return false;
                }

                guiaCompra.PersonalId = _configuracionGlobal.DefaultPersonalId;
                guiaCompra.PorcentajeIGV = _configuracionGlobal.DefaultPorcentajeIgv;

                guiaCompra.ProcesarDatos();
                guiaCompra.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dGuiaCompra dGuiaCompra = new(GetConnectionString());
                    await dGuiaCompra.Registrar(guiaCompra);

                    dGuiaCompraDetalle dGuiaCompraDetalle = new(GetConnectionString());
                    await dGuiaCompraDetalle.Registrar(guiaCompra.Detalles);

                    if (!string.IsNullOrWhiteSpace(guiaCompra.DocumentoReferenciaId))
                    {
                        dOrdenCompra dOrdenCompra = new(GetConnectionString());

                        foreach (var detalle in guiaCompra.Detalles)
                        {
                            await dOrdenCompra.ActualizarCantidadEntrada(guiaCompra.DocumentoReferenciaId, detalle.Id, detalle.Cantidad, Operacion.Disminuir);
                        }

                        await dGuiaCompra.ActualizarDocumentoCompraRelacionadoComoFacturado(guiaCompra.DocumentoReferenciaId);
                    }

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

        public async Task<bool> Modificar(GuiaCompraDTO model)
        {
            try
            {
                var guiaCompra = Mapping.Mapper.Map<oGuiaCompra>(model);

                guiaCompra.UsuarioId = _datosUsuario.Id;
                guiaCompra.PorcentajeIGV = _configuracionGlobal.DefaultPorcentajeIgv;

                guiaCompra.ProcesarDatos();
                guiaCompra.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dGuiaCompra dGuiaCompra = new(GetConnectionString());
                    await dGuiaCompra.Modificar(guiaCompra);

                    dGuiaCompraDetalle dGuiaCompraDetalle = new(GetConnectionString());
                    await dGuiaCompraDetalle.Modificar(guiaCompra.Detalles);

                    if (!string.IsNullOrWhiteSpace(guiaCompra.DocumentoReferenciaId))
                    {
                        dOrdenCompra dOrdenCompra = new(GetConnectionString());

                        foreach (var detalle in guiaCompra.Detalles)
                        {
                            await dOrdenCompra.ActualizarCantidadEntrada(guiaCompra.DocumentoReferenciaId, detalle.Id, detalle.Cantidad, Operacion.Disminuir);
                        }

                        await dGuiaCompra.ActualizarDocumentoCompraRelacionadoComoFacturado(guiaCompra.DocumentoReferenciaId);
                    }

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
                    dGuiaCompra dGuiaCompra = new(GetConnectionString());
                    await dGuiaCompra.DeshacerCompra(id);
                    await dGuiaCompra.ActualizarEstadoDocumentoCompraRelacionado(id);

                    dAbonoCompra dAbonoCompra = new(GetConnectionString());
                    await dAbonoCompra.Eliminar(id);

                    dCompraDocumentoRelacionado dCompraDocumentoRelacionado = new(GetConnectionString());
                    await dCompraDocumentoRelacionado.Eliminar(id);

                    dGuiaCompraDetalle dGuiaCompraDetalle = new(GetConnectionString());
                    await dGuiaCompraDetalle.EliminarDeGuiaCompra(id);

                    await dGuiaCompra.Eliminar(id);

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

        public async Task<oGuiaCompra> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dGuiaCompra dGuiaCompra = new(GetConnectionString());
                var guiaCompra = await dGuiaCompra.GetPorId(id);

                dGuiaCompraDetalle dGuiaCompraDetalle = new(GetConnectionString());
                guiaCompra.Detalles = (await dGuiaCompraDetalle.ListarPorGuiaCompra(guiaCompra.Id)).ToList();

                if (incluirReferencias)
                {
                    guiaCompra.Proveedor = await new dProveedor(GetConnectionString()).GetPorId(guiaCompra.ProveedorId);
                    guiaCompra.MotivoTraslado = await new dMotivoTraslado(GetConnectionString()).GetPorId(guiaCompra.MotivoTrasladoId);
                    guiaCompra.Moneda = dMoneda.GetPorId(guiaCompra.MonedaId);
                }

                return guiaCompra;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vGuiaCompra>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string proveedorNombre, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dGuiaCompra dGuiaCompra = new(GetConnectionString());
                return await dGuiaCompra.Listar(fechaInicio.Value, fechaFin.Value, proveedorNombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dGuiaCompra(GetConnectionString()).Existe(id);

        public async Task<bool> IsBloqueado(string id) => await new dGuiaCompra(GetConnectionString()).IsBloqueado(id);

        public async Task<object> FormularioTablas()
        {
            var departamentos = await new dDepartamento(GetConnectionString()).ListarTodos();
            var provincias = await new dProvincia(GetConnectionString()).ListarTodos();
            var distritos = await new dDistrito(GetConnectionString()).ListarTodos();
            var motivosTraslado = await new dMotivoTraslado(GetConnectionString()).ListarTodos();
            var conductores = await new dConductor(GetConnectionString()).ListarTodos();
            var monedas = dMoneda.ListarTodos();
            var tipos = new[]
            {
                new { Texto = "+", Valor = "+" },
                new { Texto = "-", Valor = "-" },
            };

            return new
            {
                departamentos = bUtilidad.ListarDepartamentosProvinciasDistritos(departamentos, provincias, distritos),
                motivosTraslado,
                conductores,
                monedas,
                tipos
            };
        }
    }
}
