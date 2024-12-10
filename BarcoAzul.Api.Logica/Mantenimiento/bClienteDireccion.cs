using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bClienteDireccion : bComun
    {
        public bClienteDireccion(IConnectionManager connectionManager) : base(connectionManager, origen: "Cliente - Dirección") { }

        public async Task<bool> Registrar(oClienteDireccion model)
        {
            try
            {
                model.ProcesarDatos();

                dClienteDireccion dClienteDireccion = new(GetConnectionString());
                model.TipoDireccionId = "02"; //Secundaria
                model.Id = await dClienteDireccion.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oClienteDireccion model)
        {
            try
            {
                model.ProcesarDatos();

                dClienteDireccion dClienteDireccion = new(GetConnectionString());
                await dClienteDireccion.Modificar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<bool> Eliminar(int id)
        {
            try
            {
                dClienteDireccion dClienteDireccion = new(GetConnectionString());
                await dClienteDireccion.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oClienteDireccion> GetPorId(int id)
        {
            try
            {
                dClienteDireccion dClienteDireccion = new(GetConnectionString());
                return await dClienteDireccion.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<IEnumerable<oClienteDireccion>> ListarPorCliente(string clienteId)
        {
            try
            {
                dClienteDireccion dClienteDireccion = new(GetConnectionString());
                return await dClienteDireccion.ListarPorCliente(clienteId);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(int id) => await new dClienteDireccion(GetConnectionString()).Existe(id);

        public async Task<object> FormularioTablas()
        {
            var departamentos = await new dDepartamento(GetConnectionString()).ListarTodos();
            var provincias = await new dProvincia(GetConnectionString()).ListarTodos();
            var distritos = await new dDistrito(GetConnectionString()).ListarTodos();

            return new
            {
                departamentos = bUtilidad.ListarDepartamentosProvinciasDistritos(departamentos, provincias, distritos)
            };
        }
    }
}
