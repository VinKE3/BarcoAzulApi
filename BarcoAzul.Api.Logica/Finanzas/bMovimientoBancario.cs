using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Finanzas
{
    public class bMovimientoBancario : bComun
    {
        public bMovimientoBancario(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Movimiento Bancario", configuracionGlobal: configuracionGlobal) { }

        public async Task<bool> Registrar(MovimientoBancarioDTO model)
        {
            try
            {
                dMovimientoBancario dMovimientoBancario = new(GetConnectionString());
                var movimientoBancario = Mapping.Mapper.Map<oMovimientoBancario>(model);

                movimientoBancario.ProcesarDatos();
                movimientoBancario.Id = model.Id = await dMovimientoBancario.GetNuevoNumero();
                movimientoBancario.TipoDocumentoId = model.TipoDocumentoId = dMovimientoBancario.TipoDocumentoId;
                movimientoBancario.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                movimientoBancario.ClienteId = model.TipoMovimientoId == "EG" || string.IsNullOrWhiteSpace(model.ClienteProveedorId) ? _configuracionGlobal.DefaultClienteId : model.ClienteProveedorId;
                movimientoBancario.ProveedorId = string.IsNullOrWhiteSpace(model.ClienteProveedorId) ? _configuracionGlobal.DefaultProveedorId : model.ClienteProveedorId;
                movimientoBancario.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dMovimientoBancario.Registrar(movimientoBancario);
                    dMovimientoBancarioDetalle dMovimientoBancarioDetalle = new(GetConnectionString());
                    await dMovimientoBancarioDetalle.Registrar(model.TipoMovimientoId, model.Detalles);
                    await dMovimientoBancario.EjecutarStoredProcedureDestinoTransferencia(model.Id);

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

        public async Task<bool> Modificar(MovimientoBancarioDTO model)
        {
            try
            {
                dMovimientoBancario dMovimientoBancario = new(GetConnectionString());
                var movimientoBancario = Mapping.Mapper.Map<oMovimientoBancario>(model);

                movimientoBancario.ProcesarDatos();
                movimientoBancario.TipoDocumentoId = model.TipoDocumentoId = dMovimientoBancario.TipoDocumentoId;
                movimientoBancario.ClienteId = model.TipoMovimientoId == "EG" || string.IsNullOrWhiteSpace(model.ClienteProveedorId) ? _configuracionGlobal.DefaultClienteId : model.ClienteProveedorId;
                movimientoBancario.ProveedorId = string.IsNullOrWhiteSpace(model.ClienteProveedorId) ? _configuracionGlobal.DefaultProveedorId : model.ClienteProveedorId;
                movimientoBancario.CompletarDatosDetalles();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dMovimientoBancario.Modificar(movimientoBancario);
                    dMovimientoBancarioDetalle dMovimientoBancarioDetalle = new(GetConnectionString());
                    await dMovimientoBancarioDetalle.Registrar(model.TipoMovimientoId, model.Detalles);
                    await dMovimientoBancario.EjecutarStoredProcedureDestinoTransferencia(model.Id);

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
                    dMovimientoBancario dMovimientoBancario = new(GetConnectionString());
                    await dMovimientoBancario.Eliminar(id);

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

        public async Task<oMovimientoBancario> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dMovimientoBancario dMovimientoBancario = new(GetConnectionString());
                var movimientoBancario = await dMovimientoBancario.GetPorId(id);
                movimientoBancario.TipoDocumentoId = dMovimientoBancario.TipoDocumentoId;

                dMovimientoBancarioDetalle dMovimientoBancarioDetalle = new(GetConnectionString());
                movimientoBancario.Detalles = (await dMovimientoBancarioDetalle.ListarPorMovimientoBancario(movimientoBancario.TipoMovimientoId, movimientoBancario.DocumentoVentaCompraId)).ToList();

                if (incluirReferencias)
                {
                    movimientoBancario.CuentaCorriente = await new dCuentaCorriente(GetConnectionString()).GetPorId(movimientoBancario.EmpresaId + movimientoBancario.CuentaCorrienteId);
                    movimientoBancario.Moneda = dMoneda.GetPorId(movimientoBancario.MonedaId);
                }

                return movimientoBancario;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vMovimientoBancario>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string cuentaCorrienteId, string tipoMovimientoId, string concepto, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dMovimientoBancario dMovimientoBancario = new(GetConnectionString());
                return await dMovimientoBancario.Listar(fechaInicio.Value, fechaFin.Value, cuentaCorrienteId, tipoMovimientoId, concepto ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dMovimientoBancario(GetConnectionString()).Existe(id);

        public async Task<bool> IsBloqueado(string id) => await new dMovimientoBancario(GetConnectionString()).IsBloqueado(id);

        public async Task<bool> IsModificable(string id) => await new dMovimientoBancario(GetConnectionString()).IsModificable(id);

        public async Task<(bool TieneOtroOrigen, string CuentaCorrienteInfo)> TieneOtroOrigen(string id) => await new dMovimientoBancario(GetConnectionString()).TieneOtroOrigen(id);


        public async Task<object> FiltroTablas()
        {
            var cuentasCorrientes = await new dCuentaCorriente(GetConnectionString()).ListarTodos();

            return new
            {
                cuentasCorrientes
            };
        }

        public async Task<object> FormularioTablas()
        {
            var cuentasCorrientes = await new dCuentaCorriente(GetConnectionString()).ListarTodos();
            var tiposMovimiento = dTipoMovimientoBancario.ListarTodos();
            var tiposOperacion = dTipoOperacionBancaria.ListarTodos();
            return new
            {
                cuentasCorrientes,
                tiposMovimiento,
                tiposOperacion
            };
        }
    }
}
