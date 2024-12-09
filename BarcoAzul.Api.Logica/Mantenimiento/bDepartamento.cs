using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bDepartamento : bComun, ILogicaService
    {
        public bDepartamento(IConnectionManager connectionManager) : base(connectionManager, origen: "Departamento") { }

        public async Task<bool> Registrar(oDepartamento model)
        {
            try
            {
                model.ProcesarDatos();
                dDepartamento dDepartamento = new(GetConnectionString());

                await dDepartamento.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oDepartamento model)
        {
            try
            {
                model.ProcesarDatos();

                dDepartamento dDepartamento = new(GetConnectionString());
                await dDepartamento.Modificar(model);

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
                dDepartamento dDepartamento = new(GetConnectionString());
                await dDepartamento.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oDepartamento> GetPorId(string id)
        {
            try
            {
                dDepartamento dDepartamento = new(GetConnectionString());
                return await dDepartamento.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<oDepartamento>> Listar(string nombre, oPaginacion paginacion)
        {
            try
            {
                dDepartamento dDepartamento = new(GetConnectionString());
                return await dDepartamento.Listar(nombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dDepartamento(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string nombre) => await new dDepartamento(GetConnectionString()).DatosRepetidos(id, nombre);
    }
}
