using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bMarca : bComun, ILogicaService
    {
        public bMarca(IConnectionManager connectionManager) : base(connectionManager, origen: "Marca") { }

        public async Task<bool> Registrar(oMarca model)
        {
            try
            {
                model.ProcesarDatos();

                dMarca dMarca = new(GetConnectionString());
                model.Id = await dMarca.GetNuevoId();

                await dMarca.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oMarca model)
        {
            try
            {
                model.ProcesarDatos();

                dMarca dMarca = new(GetConnectionString());
                await dMarca.Modificar(model);

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
                dMarca dMarca = new(GetConnectionString());
                await dMarca.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oMarca> GetPorId(int id)
        {
            try
            {
                dMarca dMarca = new(GetConnectionString());
                return await dMarca.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null!;
            }
        }

        public async Task<oPagina<oMarca>> Listar(string nombre, oPaginacion paginacion)
        {
            try
            {
                dMarca dMarca = new(GetConnectionString());
                return await dMarca.Listar(nombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null!;
            }
        }

        public async Task<bool> Existe(int id) => await new dMarca(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(int? id, string nombre) => await new dMarca(GetConnectionString()).DatosRepetidos(id, nombre);
    }
}
