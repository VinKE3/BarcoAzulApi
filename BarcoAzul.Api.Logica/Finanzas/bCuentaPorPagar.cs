using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;

namespace BarcoAzul.Api.Logica.Finanzas
{
    public class bCuentaPorPagar : bComun
    {
        public bCuentaPorPagar(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Cuenta por Pagar", configuracionGlobal: configuracionGlobal) { }

        public async Task<oCuentaPorPagar> GetPorId(string id)
        {
            try
            {
                dCuentaPorPagar dCuentaPorPagar = new(GetConnectionString());
                var cuentaPorPagar = await dCuentaPorPagar.GetPorId(id);

                dAbonoCompra dAbonoCompra = new(GetConnectionString());
                cuentaPorPagar.Abonos = Mapping.Mapper.Map<List<oCuentaPorPagarAbono>>((await dAbonoCompra.ListarPorDocumentoCompra(cuentaPorPagar.Id)).ToList());

                cuentaPorPagar.TipoDocumento = await new dTipoDocumento(GetConnectionString()).GetPorId(cuentaPorPagar.TipoDocumentoId);
                cuentaPorPagar.Proveedor = await new dProveedor(GetConnectionString()).GetPorId(cuentaPorPagar.ProveedorId);
                cuentaPorPagar.Moneda = dMoneda.GetPorId(cuentaPorPagar.MonedaId);

                return cuentaPorPagar;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vCuentaPorPagar>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string proveedorNombre, bool? isCancelado, oPaginacion paginacion)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dCuentaPorPagar dCuentaPorPagar = new(GetConnectionString());
                return await dCuentaPorPagar.Listar(GetTiposDocumentoPermitidos(), fechaInicio.Value, fechaFin.Value, proveedorNombre ?? string.Empty, isCancelado, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<oPagina<vCuentaPorPagarPendiente>> ListarPendientes(string numeroDocumento, oPaginacion paginacion, string tipoDocumentoId = "", string proveedorId = "")
        {
            try
            {
                dCuentaPorPagar dCuentaPorPagar = new(GetConnectionString());
                return await dCuentaPorPagar.ListarPendientes(numeroDocumento ?? string.Empty, paginacion, tipoDocumentoId, proveedorId);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dCuentaPorPagar(GetConnectionString()).Existe(id);

        public async Task<bool> IsBloqueado(string id) => await new dCuentaPorPagar(GetConnectionString()).IsBloqueado(id);

        private static string[] GetTiposDocumentoPermitidos() => new string[] { "01", "02", "03", "04", "07", "08", "12", "14", "NV", "PR", "CR", "CV", "LC", "CF", "CH", "FN" };
    }
}
