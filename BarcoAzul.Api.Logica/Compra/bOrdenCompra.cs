using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Informes.PDFs;
using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Compra;
using BarcoAzul.Api.Repositorio.Empresa;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Compra
{
    public class bOrdenCompra : bComun
    {
        public bOrdenCompra(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Orden de Compra", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(OrdenCompraDTO model)
        {
            try
            {
                dOrdenCompra dOrdenCompra = new(GetConnectionString());

                var ordenCompra = Mapping.Mapper.Map<oOrdenCompra>(model);

                ordenCompra.ProcesarDatos();
                ordenCompra.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                ordenCompra.TipoDocumentoId = model.TipoDocumentoId = dOrdenCompra.TipoDocumentoId;
                ordenCompra.Serie = model.Serie = "0001";
                ordenCompra.Numero = model.Numero = await dOrdenCompra.GetNuevoNumero(ordenCompra.EmpresaId, ordenCompra.Serie);
                ordenCompra.ClienteId = model.ClienteId = _configuracionGlobal.DefaultClienteId;
                ordenCompra.UsuarioId = _datosUsuario.Id;
                ordenCompra.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dOrdenCompra.Registrar(ordenCompra);

                    dOrdenCompraDetalle dOrdenCompraDetalle = new(GetConnectionString());
                    await dOrdenCompraDetalle.Registrar(ordenCompra.Detalles);

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

        public async Task<bool> Modificar(OrdenCompraDTO model)
        {
            try
            {
                dOrdenCompra dOrdenCompra = new(GetConnectionString());

                var ordenCompra = Mapping.Mapper.Map<oOrdenCompra>(model);

                ordenCompra.ProcesarDatos();
                ordenCompra.UsuarioId = _datosUsuario.Id;
                ordenCompra.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dOrdenCompra.Modificar(ordenCompra);

                    dOrdenCompraDetalle dOrdenCompraDetalle = new(GetConnectionString());
                    await dOrdenCompraDetalle.Modificar(ordenCompra.Detalles);

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

                    dCompraDocumentoRelacionado dCompraDocumentoRelacionado = new(GetConnectionString());
                    await dCompraDocumentoRelacionado.Eliminar(id);

                    dOrdenCompraDetalle dOrdenCompraDetalle = new(GetConnectionString());
                    await dOrdenCompraDetalle.EliminarDeOrdenCompra(id);

                    dOrdenCompra dOrdenCompra = new(GetConnectionString());
                    await dOrdenCompra.Eliminar(id);

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

        public async Task<bool> Finalizar(string id)
        {
            try
            {
                dOrdenCompra dOrdenCompra = new(GetConnectionString());
                await dOrdenCompra.ActualizarEstadoFacturado(id, true);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<(string Nombre, byte[] Archivo)> Imprimir(string id)
        {
            try
            {
                var ordenCompra = await GetPorId(id, true);

                //Datos adicionales para la impresión
                ordenCompra.Proveedor.Departamento = await new dDepartamento(GetConnectionString()).GetPorId(ordenCompra.Proveedor.DepartamentoId);
                ordenCompra.Proveedor.Provincia = await new dProvincia(GetConnectionString()).GetPorId(ordenCompra.Proveedor.DepartamentoId + ordenCompra.Proveedor.ProvinciaId);
                ordenCompra.Proveedor.Distrito = await new dDistrito(GetConnectionString()).GetPorId(ordenCompra.Proveedor.DepartamentoId + ordenCompra.Proveedor.ProvinciaId + ordenCompra.Proveedor.DistritoId);

                dCargo dCargo = new(GetConnectionString());

                if (ordenCompra.Responsable1 is not null && ordenCompra.Responsable1.CargoId is not null)
                    ordenCompra.Responsable1.Cargo = await dCargo.GetPorId(ordenCompra.Responsable1.CargoId.Value);

                if (ordenCompra.Responsable2 is not null && ordenCompra.Responsable2.CargoId is not null)
                    ordenCompra.Responsable2.Cargo = await dCargo.GetPorId(ordenCompra.Responsable2.CargoId.Value);

                if (ordenCompra.Responsable3 is not null && ordenCompra.Responsable3.CargoId is not null)
                    ordenCompra.Responsable3.Cargo = await dCargo.GetPorId(ordenCompra.Responsable3.CargoId.Value);
                //Datos adicionales para la impresión

                var rptPath = RptPath.RptOrdenCompraPath;
                var configuracionEmpresa = await new dEmpresa(GetConnectionString()).Get();

                var pdfOrdenCompra = new PDFOrdenCompra(ordenCompra, configuracionEmpresa, rptPath);

                return ($"{ordenCompra.TipoDocumentoId}-{ordenCompra.Serie}-{int.Parse(ordenCompra.Numero)}.pdf", pdfOrdenCompra.Generar());
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return (string.Empty, null);
            }
        }

        public async Task<oOrdenCompra> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dOrdenCompra dOrdenCompra = new(GetConnectionString());
                var ordenCompra = await dOrdenCompra.GetPorId(id);

                dOrdenCompraDetalle dOrdenCompraDetalle = new(GetConnectionString());
                ordenCompra.Detalles = (await dOrdenCompraDetalle.ListarPorOrdenCompra(ordenCompra.Id)).ToList();

                if (incluirReferencias)
                {
                    ordenCompra.TipoDocumento = await new dTipoDocumento(GetConnectionString()).GetPorId(ordenCompra.TipoDocumentoId);

                    if (!string.IsNullOrWhiteSpace(ordenCompra.Responsable1Id))
                        ordenCompra.Responsable1 = await new dPersonal(GetConnectionString()).GetPorId(ordenCompra.Responsable1Id);

                    if (!string.IsNullOrWhiteSpace(ordenCompra.Responsable2Id))
                        ordenCompra.Responsable2 = await new dPersonal(GetConnectionString()).GetPorId(ordenCompra.Responsable2Id);

                    if (!string.IsNullOrWhiteSpace(ordenCompra.Responsable3Id))
                        ordenCompra.Responsable3 = await new dPersonal(GetConnectionString()).GetPorId(ordenCompra.Responsable3Id);

                    ordenCompra.Proveedor = await new dProveedor(GetConnectionString()).GetPorId(ordenCompra.ProveedorId);

                    if (!string.IsNullOrWhiteSpace(ordenCompra.ProveedorContactoId))
                        ordenCompra.ProveedorContacto = await new dProveedorContacto(GetConnectionString()).GetPorId(ordenCompra.ProveedorContactoId);

                    ordenCompra.Moneda = dMoneda.GetPorId(ordenCompra.MonedaId);
                    ordenCompra.TipoCompra = dTipoVentaCompra.GetPorId(ordenCompra.TipoCompraId);
                    ordenCompra.TipoPago = await new dTipoCobroPago(GetConnectionString()).GetPorId(ordenCompra.TipoPagoId);

                    if (!string.IsNullOrWhiteSpace(ordenCompra.CuentaCorrienteId))
                        ordenCompra.CuentaCorriente = await new dCuentaCorriente(GetConnectionString()).GetPorId(ordenCompra.CuentaCorrienteId);

                    if (!string.IsNullOrWhiteSpace(ordenCompra.ProveedorCuentaCorriente1Id))
                        ordenCompra.ProveedorCuentaCorriente1 = await new dProveedorCuentaCorriente(GetConnectionString()).GetPorId(ordenCompra.ProveedorCuentaCorriente1Id);

                    if (!string.IsNullOrWhiteSpace(ordenCompra.ProveedorCuentaCorriente2Id))
                        ordenCompra.ProveedorCuentaCorriente2 = await new dProveedorCuentaCorriente(GetConnectionString()).GetPorId(ordenCompra.ProveedorCuentaCorriente2Id);
                }

                return ordenCompra;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vOrdenCompra>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string proveedorNombre, string estado, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dOrdenCompra dOrdenCompra = new(GetConnectionString());
                return await dOrdenCompra.Listar(fechaInicio.Value, fechaFin.Value, proveedorNombre ?? string.Empty, estado, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<oPagina<vOrdenCompraPendiente>> ListarPendientes(DateTime? fechaInicio, DateTime? fechaFin, string proveedorId, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dOrdenCompra dOrdenCompra = new(GetConnectionString());
                return await dOrdenCompra.ListarPendientes(fechaInicio.Value, fechaFin.Value, proveedorId, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dOrdenCompra(GetConnectionString()).Existe(id);

        public async Task<bool> IsBloqueado(string id) => await new dOrdenCompra(GetConnectionString()).IsBloqueado(id);

        public async Task<bool> IsFacturado(string id) => await new dOrdenCompra(GetConnectionString()).IsFacturado(id);

        public async Task<object> FormularioTablas()
        {
            var tiposCompra = dTipoVentaCompra.ListarTodos();
            var monedas = dMoneda.ListarTodos();
            var tiposPago = await new dTipoCobroPago(GetConnectionString()).ListarTodos();
            var lugaresEntrega = await new dLugarEntrega(GetConnectionString()).ListarTodos();
            var porcentajesIGV = await new dEmpresaIGV(GetConnectionString()).ListarTodos();
            var porcentajesRetencion = await new dEmpresaRetencion(GetConnectionString()).ListarTodos();
            var porcentajesPercepcion = await new dEmpresaPercepcion(GetConnectionString()).ListarTodos();
            var personal = await new dPersonal(GetConnectionString()).ListarTodos();
            var cuentasCorrientes = await new dCuentaCorriente(GetConnectionString()).ListarTodos();
            var serie = "0001";

            return new
            {
                tiposCompra,
                monedas,
                tiposPago,
                lugaresEntrega,
                porcentajesIGV,
                porcentajesRetencion,
                porcentajesPercepcion,
                personal,
                cuentasCorrientes,
                serie
            };
        }
    }
}
