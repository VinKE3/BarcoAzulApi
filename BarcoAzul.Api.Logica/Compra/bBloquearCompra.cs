using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Compra;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Compra
{
    public class bBloquearCompra : bComun, ILogicaService
    {
        public bBloquearCompra(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Bloquear Compra", configuracionGlobal: configuracionGlobal) { }

        public async Task<bool> Procesar(oBloquearCompra bloquearCompra)
        {
            try
            {
                dBloquearCompra dBloquearCompra = new(GetConnectionString());

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    await dBloquearCompra.Procesar(bloquearCompra);
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

        public async Task<oPagina<vBloquearCompra>> Listar(string tipoDocumentoId, DateTime? fechaInicio, DateTime? fechaFin, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;
                var tiposDocumentosPermitidos = string.IsNullOrWhiteSpace(tipoDocumentoId) ? GetTiposDocumentoPermitidos() : null;

                dBloquearCompra dBloquearCompra = new(GetConnectionString());
                return await dBloquearCompra.Listar(tiposDocumentosPermitidos, tipoDocumentoId, fechaInicio.Value, fechaFin.Value, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<object> FormularioTablas()
        {
            var tiposDocumento = await new dTipoDocumento(GetConnectionString()).Listar(GetTiposDocumentoPermitidos());

            return new
            {
                tiposDocumento
            };
        }

        private static string[] GetTiposDocumentoPermitidos() => new string[] { "01", "03", "07", "08", "09", "CF", "CH", "FN", "LC", "EN", "EC" };
    }
}
