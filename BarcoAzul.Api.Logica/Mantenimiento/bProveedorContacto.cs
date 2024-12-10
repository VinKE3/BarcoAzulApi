using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Repositorio.Mantenimiento;


namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bProveedorContacto : bComun, ILogicaService
    {
        public bProveedorContacto(IConnectionManager connectionManager) : base(connectionManager, origen: "Proveedor - Contacto") { }

        public async Task<bool> Registrar(oProveedorContacto model)
        {
            try
            {
                model.ProcesarDatos();

                dProveedorContacto dProveedorContacto = new(GetConnectionString());
                model.ContactoId = await dProveedorContacto.GetNuevoId(model.ProveedorId);

                await dProveedorContacto.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oProveedorContacto model)
        {
            try
            {
                model.ProcesarDatos();

                dProveedorContacto dProveedorContacto = new(GetConnectionString());
                await dProveedorContacto.Modificar(model);

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
                dProveedorContacto dProveedorContacto = new(GetConnectionString());
                await dProveedorContacto.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oProveedorContacto> GetPorId(string id)
        {
            try
            {
                dProveedorContacto dProveedorContacto = new(GetConnectionString());
                return await dProveedorContacto.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<IEnumerable<oProveedorContacto>> ListarPorProveedor(string proveedorId)
        {
            try
            {
                dProveedorContacto dProveedorContacto = new(GetConnectionString());
                return await dProveedorContacto.ListarPorProveedor(proveedorId);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dProveedorContacto(GetConnectionString()).Existe(id);

        public async Task<object> FormularioTablas()
        {
            var cargos = await new dCargo(GetConnectionString()).ListarTodos();

            return new
            {
                cargos
            };
        }
    }
}
