using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Mantenimiento;


namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bUnidadMedida : bComun, ILogicaService
    {
        public bUnidadMedida(IConnectionManager connectionManager) : base(connectionManager, origen: "Unidad de Medida") { }

        public async Task<bool> Registrar(oUnidadMedida model)
        {
            try
            {
                model.ProcesarDatos();

                dUnidadMedida dUnidadMedida = new(GetConnectionString());
                model.Id = await dUnidadMedida.GetNuevoId();

                await dUnidadMedida.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oUnidadMedida model)
        {
            try
            {
                model.ProcesarDatos();

                dUnidadMedida dUnidadMedida = new(GetConnectionString());
                await dUnidadMedida.Modificar(model);

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
                dUnidadMedida dUnidadMedida = new(GetConnectionString());
                await dUnidadMedida.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oUnidadMedida> GetPorId(string id)
        {
            try
            {
                dUnidadMedida dUnidadMedida = new(GetConnectionString());
                return await dUnidadMedida.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null!;
            }
        }

        public async Task<oPagina<oUnidadMedida>> Listar(string descripcion, oPaginacion paginacion)
        {
            try
            {
                dUnidadMedida dUnidadMedida = new(GetConnectionString());
                return await dUnidadMedida.Listar(descripcion ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null!;
            }
        }

        public async Task<bool> Existe(string id) => await new dUnidadMedida(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string descripcion) => await new dUnidadMedida(GetConnectionString()).DatosRepetidos(id, descripcion);
    }
}
