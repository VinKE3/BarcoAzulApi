using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bProvincia : bComun, ILogicaService
    {
        public bProvincia(IConnectionManager connectionManager) : base(connectionManager, "Provincia") { }

        public async Task<bool> Registrar(ProvinciaDTO model)
        {
            try
            {
                var provincia = Mapping.Mapper.Map<oProvincia>(model);
                provincia.ProcesarDatos();

                dProvincia dProvincia = new(GetConnectionString());
                await dProvincia.Registrar(provincia);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(ProvinciaDTO model)
        {
            try
            {
                var provincia = Mapping.Mapper.Map<oProvincia>(model);
                provincia.ProcesarDatos();

                dProvincia dProvincia = new(GetConnectionString());
                await dProvincia.Modificar(provincia);

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
                dProvincia dProvincia = new(GetConnectionString());
                await dProvincia.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oProvincia> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dProvincia dProvincia = new(GetConnectionString());
                var provincia = await dProvincia.GetPorId(id);

                if (incluirReferencias)
                {
                    provincia.Departamento = await new dDepartamento(GetConnectionString()).GetPorId(provincia.DepartamentoId);
                }

                return provincia;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vProvincia>> Listar(string nombre, oPaginacion paginacion)
        {
            try
            {
                dProvincia dProvincia = new(GetConnectionString());
                return await dProvincia.Listar(nombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dProvincia(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string nombre) => await new dProvincia(GetConnectionString()).DatosRepetidos(id, nombre);

        public async Task<object> FormularioTablas()
        {
            var departamentos = await new dDepartamento(GetConnectionString()).ListarTodos();

            return new
            {
                departamentos
            };
        }
    }
}
