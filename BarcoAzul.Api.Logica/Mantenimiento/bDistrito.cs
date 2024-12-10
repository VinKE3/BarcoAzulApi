using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bDistrito : bComun, ILogicaService
    {
        public bDistrito(IConnectionManager connectionManager) : base(connectionManager, origen: "Distrito") { }

        public async Task<bool> Registrar(DistritoDTO model)
        {
            try
            {
                var distrito = Mapping.Mapper.Map<oDistrito>(model);
                distrito.ProcesarDatos();

                dDistrito dDistrito = new(GetConnectionString());
                await dDistrito.Registrar(distrito);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(DistritoDTO model)
        {
            try
            {
                var distrito = Mapping.Mapper.Map<oDistrito>(model);
                distrito.ProcesarDatos();

                dDistrito dDistrito = new(GetConnectionString());
                await dDistrito.Modificar(distrito);

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
                dDistrito dDistrito = new(GetConnectionString());
                await dDistrito.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oDistrito> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dDistrito dDistrito = new(GetConnectionString());
                var distrito = await dDistrito.GetPorId(id);

                if (incluirReferencias)
                {
                    distrito.Departamento = await new dDepartamento(GetConnectionString()).GetPorId(distrito.DepartamentoId);
                    distrito.Provincia = await new dProvincia(GetConnectionString()).GetPorId(distrito.DepartamentoId + distrito.ProvinciaId);
                }

                return distrito;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vDistrito>> Listar(string nombre, oPaginacion paginacion)
        {
            try
            {
                dDistrito dDistrito = new(GetConnectionString());
                return await dDistrito.Listar(nombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dDistrito(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string nombre) => await new dDistrito(GetConnectionString()).DatosRepetidos(id, nombre);

        public async Task<object> FormularioTablas()
        {
            var departamentos = await new dDepartamento(GetConnectionString()).ListarTodos();
            var provincias = await new dProvincia(GetConnectionString()).ListarTodos();

            return new
            {
                departamentos = bUtilidad.ListarDepartamentosProvinciasDistritos(departamentos, provincias)
            };
        }
    }
}
