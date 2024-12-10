using BarcoAzul.Api.Logica.Mantenimiento;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Configuracion;
using BarcoAzul.Api.Repositorio.Almacen;


namespace BarcoAzul.Api.Logica.Configuracion
{
    public class bConfiguracionGlobal : bComun
    {
        public bConfiguracionGlobal(IConnectionManager connectionManager) : base(connectionManager, origen: "Configuración") { }

        public async Task<oConfiguracionGlobal> Get()
        {
            var configuracionGlobal = await new dConfiguracionGlobal(GetConnectionString()).Get();

            dCuadreStock dCuadreStock = new(GetConnectionString());
            configuracionGlobal.FechaUltimoCuadre = await dCuadreStock.GetFechaUltimoCuadre();

            return configuracionGlobal;
        }

        public async Task<oConfiguracionSimplificado> GetSimplificado(oConfiguracionGlobal configuracionGlobal)
        {
            try
            {
                var configuracionSimplificado = Mapping.Mapper.Map<oConfiguracionSimplificado>(configuracionGlobal);

                configuracionSimplificado.Cliente = await new bCliente(null, ConnectionManager).GetPorId(configuracionSimplificado.ClienteId, true);
                configuracionSimplificado.Proveedor = await new bProveedor(null, ConnectionManager).GetPorId(configuracionSimplificado.ProveedorId, true);
                configuracionSimplificado.Articulo = await new bArticulo(null, ConnectionManager).GetPorId(configuracionSimplificado.ArticuloId, true);

                return configuracionSimplificado;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }
    }
}
