using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Venta;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Venta
{
    public class bBloquearVenta : bComun, ILogicaService
    {
        public bBloquearVenta(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Bloquear Venta", configuracionGlobal: configuracionGlobal) { }

        public async Task<bool> Procesar(oBloquearVenta bloquearVenta)
        {
            try
            {
                dBloquearVenta dBloquearVenta = new(GetConnectionString());

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dBloquearVenta.Procesar(bloquearVenta);
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

        public async Task<oPagina<vBloquearVenta>> Listar(string tipoDocumentoId, DateTime? fechaInicio, DateTime? fechaFin, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;
                var tiposDocumentoNoPermitidos = string.IsNullOrWhiteSpace(tipoDocumentoId) ? GetTiposDocumentoNoPermitidos() : null;

                dBloquearVenta dBloquearVenta = new(GetConnectionString());
                return await dBloquearVenta.Listar(tiposDocumentoNoPermitidos, tipoDocumentoId, fechaInicio.Value, fechaFin.Value, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<object> FormularioTablas()
        {
            var tiposDocumento = await new dTipoDocumento(GetConnectionString()).Listar(new string[] { "01", "03", "07", "08", "09", "LC", "SA", "PL" });

            return new
            {
                tiposDocumento
            };
        }

        private static string[] GetTiposDocumentoNoPermitidos() => new string[] { "13", "CI", "CU", "NP", "LC", "SC", "OP" };
    }
}
