using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bCargo : bComun, ILogicaService
    {
        public bCargo(IConnectionManager connectionManager) : base(connectionManager, origen: "Cargo") { }

        public async Task<bool> Registrar(oCargo model)
        {
            try
            {
                model.ProcesarDatos();

                dCargo dCargo = new(GetConnectionString());
                model.Id = await dCargo.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oCargo model)
        {
            try
            {
                model.ProcesarDatos();

                dCargo dCargo = new(GetConnectionString());
                await dCargo.Modificar(model);

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
                dCargo dCargo = new(GetConnectionString());
                await dCargo.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oCargo> GetPorId(int id)
        {
            try
            {
                dCargo dCargo = new(GetConnectionString());
                return await dCargo.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<oCargo>> Listar(string descripcion, oPaginacion paginacion)
        {
            try
            {
                dCargo dCargo = new(GetConnectionString());
                return await dCargo.Listar(descripcion ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(int id) => await new dCargo(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(int? id, string descripcion) => await new dCargo(GetConnectionString()).DatosRepetidos(id, descripcion);
    }
}
