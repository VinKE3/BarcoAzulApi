using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;

namespace BarcoAzul.Api.Logica.Finanzas
{
    public class bCuentaPorCobrar : bComun
    {
        public bCuentaPorCobrar(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Cuenta por Cobrar", datosUsuario, configuracionGlobal) { }

        public async Task<oCuentaPorCobrar> GetPorId(string id)
        {
            try
            {
                dCuentaPorCobrar dCuentaPorCobrar = new(GetConnectionString());
                var cuentaPorCobrar = await dCuentaPorCobrar.GetPorId(id);

                dAbonoVenta dAbonoVenta = new(GetConnectionString());
                cuentaPorCobrar.Abonos = Mapping.Mapper.Map<List<oCuentaPorCobrarAbono>>((await dAbonoVenta.ListarPorDocumentoVenta(cuentaPorCobrar.Id)).ToList());

                cuentaPorCobrar.TipoDocumento = await new dTipoDocumento(GetConnectionString()).GetPorId(cuentaPorCobrar.TipoDocumentoId);
                cuentaPorCobrar.Cliente = await new dCliente(GetConnectionString()).GetPorId(cuentaPorCobrar.ClienteId);
                cuentaPorCobrar.Moneda = dMoneda.GetPorId(cuentaPorCobrar.MonedaId);

                return cuentaPorCobrar;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vCuentaPorCobrar>> Listar(string tipoDocumentoId, DateTime? fechaInicio, DateTime? fechaFin, string clienteNombre, bool? isCancelado, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;
                var tiposDocumentosId = string.IsNullOrWhiteSpace(tipoDocumentoId) ? GetTiposDocumentoPermitidos() : new string[] { tipoDocumentoId };

                dCuentaPorCobrar dCuentaPorCobrar = new(GetConnectionString());
                return await dCuentaPorCobrar.Listar(tiposDocumentosId, fechaInicio.Value, fechaFin.Value, clienteNombre ?? string.Empty, isCancelado, _datosUsuario.PersonalId, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<oPagina<vCuentaPorCobrarPendiente>> ListarPendientes(string numeroDocumento, oPaginacion paginacion, string tipoDocumentoId = "", string clienteId = "")
        {
            try
            {
                dCuentaPorCobrar dCuentaPorCobrar = new(GetConnectionString());
                return await dCuentaPorCobrar.ListarPendientes(numeroDocumento ?? string.Empty, paginacion, tipoDocumentoId, clienteId);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dCuentaPorCobrar(GetConnectionString()).Existe(id);

        public async Task<object> FiltroTablas()
        {
            var tiposDocumento = await new dTipoDocumento(GetConnectionString()).Listar(GetTiposDocumentoPermitidos());

            return new
            {
                tiposDocumento
            };
        }

        private static string[] GetTiposDocumentoPermitidos() => new[] { "01", "03", "07", "08", "LC", "NV" };
    }
}
