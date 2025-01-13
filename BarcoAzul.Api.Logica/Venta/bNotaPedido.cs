using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.DTOs;
using System.Transactions;
using BarcoAzul.Api.Repositorio.Venta;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Informes.PDFs;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Empresa;

namespace BarcoAzul.Api.Logica.Venta
{
    public class bNotaPedido : bComun
    {
        public bNotaPedido(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "NotaPedido", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(NotaPedidoDTO model)
        {
            try
            {
                dNotaPedido dNotaPedido = new(GetConnectionString());

                var notaPedido = Mapping.Mapper.Map<oNotaPedido>(model);

                notaPedido.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                notaPedido.TipoDocumentoId = model.TipoDocumentoId = dNotaPedido.TipoDocumentoId;
                notaPedido.Numero = model.Numero = await dNotaPedido.GetNuevoNumero(notaPedido.EmpresaId, notaPedido.Serie);
                notaPedido.UsuarioId = _datosUsuario.Id;

                notaPedido.ProcesarDatos();
                notaPedido.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dNotaPedido.Registrar(notaPedido);

                    dNotaPedidoDetalle dNotaPedidoDetalle = new(GetConnectionString());
                    await dNotaPedidoDetalle.Registrar(notaPedido.Detalles);

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

        public async Task<bool> Modificar(NotaPedidoDTO model)
        {
            try
            {
                var notaPedido = Mapping.Mapper.Map<oNotaPedido>(model);

                notaPedido.UsuarioId = _datosUsuario.Id;
                notaPedido.ProcesarDatos();
                notaPedido.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dNotaPedido dNotaPedido = new(GetConnectionString());
                    await dNotaPedido.Modificar(notaPedido);

                    dNotaPedidoDetalle dNotaPedidoDetalle = new(GetConnectionString());
                    await dNotaPedidoDetalle.Modificar(notaPedido.Detalles);

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
                    dNotaPedido dNotaPedido = new(GetConnectionString());
                    await dNotaPedido.Eliminar(id);

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
                    dNotaPedido dNotaPedido = new(GetConnectionString());
                    await dNotaPedido.Anular(id);

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

        public async Task<(string Nombre, byte[] Archivo)> Imprimir(string id)
        {
            try
            {
                var notaPedido = await GetPorId(id, true);
                var rptPath = RptPath.RptNotaPedidoPath;

                var pdfNotaPedido = new PDFNotaPedido(notaPedido, _configuracionGlobal, rptPath);

                return ($"{notaPedido.TipoDocumentoId}-{notaPedido.Serie}-{int.Parse(notaPedido.Numero)}.pdf", pdfNotaPedido.Generar());
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return (string.Empty, null);
            }
        }

        public async Task<oNotaPedido> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dNotaPedido dNotaPedido = new(GetConnectionString());
                var notaPedido = await dNotaPedido.GetPorId(id);

                dNotaPedidoDetalle dNotaPedidoDetalle = new(GetConnectionString());
                notaPedido.Detalles = (await dNotaPedidoDetalle.ListarPorNotaPedido(notaPedido.Id)).ToList();

                if (incluirReferencias)
                {
                    notaPedido.Cliente = await new dCliente(GetConnectionString()).GetPorId(notaPedido.ClienteId);
                    notaPedido.Personal = await new dPersonal(GetConnectionString()).GetPorId(notaPedido.PersonalId);
                    notaPedido.Moneda = dMoneda.GetPorId(notaPedido.MonedaId);
                    notaPedido.TipoVenta = dTipoVentaCompra.GetPorId(notaPedido.TipoVentaId);
                    notaPedido.TipoCobro = await new dTipoCobroPago(GetConnectionString()).GetPorId(notaPedido.TipoCobroId);
                }

                return notaPedido;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vNotaPedido>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string clienteNombre, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dNotaPedido dNotaPedido = new(GetConnectionString());
                return await dNotaPedido.Listar(fechaInicio.Value, fechaFin.Value, clienteNombre ?? string.Empty, _datosUsuario.PersonalId, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dNotaPedido(GetConnectionString()).Existe(id);

        public async Task<bool> IsBloqueado(string id) => await new dNotaPedido(GetConnectionString()).IsBloqueado(id);

        public async Task<bool> IsAnulado(string id) => await new dNotaPedido(GetConnectionString()).IsAnulado(id);

        public async Task<bool> IsFacturado(string id) => await new dNotaPedido(GetConnectionString()).IsFacturado(id);

        public async Task<object> FormularioTablas()
        {
            var tiposDocumento = await new dTipoDocumento(GetConnectionString()).Listar(new string[] { "01", "02", "NP" });
            var vendedores = await new dPersonal(GetConnectionString()).ListarTodos();
            var monedas = dMoneda.ListarTodos();
            var tiposVenta = dTipoVentaCompra.ListarTodos();
            var tiposCobro = await new dTipoCobroPago(GetConnectionString()).ListarTodos();
            var cuentasCorrientes = await new dCuentaCorriente(GetConnectionString()).ListarTodos();
            var porcentajesIGV = await new dEmpresaIGV(GetConnectionString()).ListarTodos();
            var porcentajesRetencion = await new dEmpresaRetencion(GetConnectionString()).ListarTodos();
            var porcentajesPercepcion = await new dEmpresaPercepcion(GetConnectionString()).ListarTodos();
            var serie = "0002";

            return new
            {
                tiposDocumento,
                vendedores,
                monedas,
                tiposVenta,
                tiposCobro,
                cuentasCorrientes,
                porcentajesIGV,
                porcentajesRetencion,
                porcentajesPercepcion,
                serie
            };
        }

    }
}
