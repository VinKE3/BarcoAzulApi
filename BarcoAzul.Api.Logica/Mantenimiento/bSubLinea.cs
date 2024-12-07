using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;


namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bSubLinea : bComun, ILogicaService
    {
        public bSubLinea(IConnectionManager connectionManager) : base(connectionManager, origen: "SubLínea") { }

        public async Task<bool> Registrar(oSubLinea model)
        {
            try
            {
                model.ProcesarDatos();

                dSubLinea dSubLinea = new(GetConnectionString());
                model.SubLineaId = await dSubLinea.GetNuevoId(model.LineaId);

                await dSubLinea.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oSubLinea model)
        {
            try
            {
                model.ProcesarDatos();

                dSubLinea dSubLinea = new(GetConnectionString());
                await dSubLinea.Modificar(model);

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
                dSubLinea dSubLinea = new(GetConnectionString());
                await dSubLinea.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oSubLinea> GetPorId(string id)
        {
            try
            {
                dSubLinea dSubLinea = new(GetConnectionString());
                return await dSubLinea.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null!;
            }
        }

        public async Task<oPagina<vSubLinea>> Listar(string descripcion, oPaginacion paginacion)
        {
            try
            {
                dSubLinea dSubLinea = new(GetConnectionString());
                return await dSubLinea.Listar(descripcion ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null!;
            }
        }

        public async Task<bool> Existe(string id) => await new dSubLinea(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string descripcion) => await new dSubLinea(GetConnectionString()).DatosRepetidos(id, descripcion);

        public async Task<object> FormularioTablas()
        {
            var lineas = await new dLinea(GetConnectionString()).ListarTodos();

            return new
            {
                lineas
            };
        }
    }
}
