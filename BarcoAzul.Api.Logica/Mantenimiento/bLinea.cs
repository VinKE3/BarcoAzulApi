using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bLinea : bComun, ILogicaService
    {
        public bLinea(IConnectionManager connectionManager) : base(connectionManager, origen: "Línea") { }

        public async Task<bool> Registrar(oLinea model)
        {
            try
            {
                model.ProcesarDatos();

                dLinea dLinea = new(GetConnectionString());
                model.Id = await dLinea.GetNuevoId();

                await dLinea.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oLinea model)
        {
            try
            {
                model.ProcesarDatos();

                dLinea dLinea = new(GetConnectionString());
                await dLinea.Modificar(model);

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
                dLinea dLinea = new(GetConnectionString());
                await dLinea.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oLinea> GetPorId(string id)
        {
            try
            {
                dLinea dLinea = new(GetConnectionString());
                return await dLinea.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null!;
            }
        }

        public async Task<oPagina<oLinea>> Listar(string descripcion, oPaginacion paginacion)
        {
            try
            {
                dLinea dLinea = new(GetConnectionString());
                return await dLinea.Listar(descripcion ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null!;
            }
        }

        public async Task<bool> Existe(string id) => await new dLinea(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string descripcion) => await new dLinea(GetConnectionString()).DatosRepetidos(id, descripcion);
    }
}
