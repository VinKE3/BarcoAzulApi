using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Finanzas;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Finanzas
{
    public class bBloquearMovimientoBancario : bComun, ILogicaService
    {
        public bBloquearMovimientoBancario(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Bloquear Movimiento Bancario", configuracionGlobal: configuracionGlobal) { }

        public async Task<bool> Procesar(oBloquearMovimientoBancario bloquearMovimientoBancario)
        {
            try
            {
                dBloquearMovimientoBancario dBloquearMovimientoBancario = new(GetConnectionString());

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dBloquearMovimientoBancario.Procesar(bloquearMovimientoBancario);
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

        public async Task<oPagina<vBloquearMovimientoBancario>> Listar(DateTime? fechaInicio, DateTime? fechaFin, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dBloquearMovimientoBancario dBloquearMovimientoBancario = new(GetConnectionString());
                return await dBloquearMovimientoBancario.Listar(fechaInicio.Value, fechaFin.Value, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }
    }
}
