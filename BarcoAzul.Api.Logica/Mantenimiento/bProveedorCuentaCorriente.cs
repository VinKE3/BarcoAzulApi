using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bProveedorCuentaCorriente : bComun, ILogicaService
    {
        public bProveedorCuentaCorriente(IConnectionManager connectionManager) : base(connectionManager, origen: "Proveedor - Cuenta Corriente") { }

        public async Task<bool> Registrar(oProveedorCuentaCorriente model)
        {
            try
            {
                model.ProcesarDatos();

                dProveedorCuentaCorriente dProveedorCuentaCorriente = new(GetConnectionString());
                model.CuentaCorrienteId = await dProveedorCuentaCorriente.GetNuevoId(model.ProveedorId);

                await dProveedorCuentaCorriente.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oProveedorCuentaCorriente model)
        {
            try
            {
                model.ProcesarDatos();

                dProveedorCuentaCorriente dProveedorCuentaCorriente = new(GetConnectionString());
                await dProveedorCuentaCorriente.Modificar(model);

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
                dProveedorCuentaCorriente dProveedorCuentaCorriente = new(GetConnectionString());
                await dProveedorCuentaCorriente.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oProveedorCuentaCorriente> GetPorId(string id)
        {
            try
            {
                dProveedorCuentaCorriente dProveedorCuentaCorriente = new(GetConnectionString());
                return await dProveedorCuentaCorriente.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<IEnumerable<oProveedorCuentaCorriente>> ListarPorProveedor(string proveeedorId)
        {
            try
            {
                dProveedorCuentaCorriente dProveedorCuentaCorriente = new(GetConnectionString());
                return await dProveedorCuentaCorriente.ListarPorProveedor(proveeedorId);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dProveedorCuentaCorriente(GetConnectionString()).Existe(id);

        public async Task<object> FormularioTablas()
        {
            var monedas = dMoneda.ListarTodos();
            var entidadesBancarias = await new dEntidadBancaria(GetConnectionString()).ListarTodos();

            return new
            {
                monedas,
                entidadesBancarias
            };
        }
    }
}
