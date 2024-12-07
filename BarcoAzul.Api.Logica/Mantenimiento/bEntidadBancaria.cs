using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;


namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bEntidadBancaria : bComun, ILogicaService
    {
        public bEntidadBancaria(IConnectionManager connectionManager) : base(connectionManager, origen: "Entidad Bancaria") { }

        public async Task<bool> Registrar(vEntidadBancaria model)
        {
            try
            {
                var entidadBancaria = Mapping.Mapper.Map<oEntidadBancaria>(model);
                entidadBancaria.ProcesarDatos();

                dEntidadBancaria dEntidadBancaria = new(GetConnectionString());
                model.Id = await dEntidadBancaria.Registrar(entidadBancaria);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(vEntidadBancaria model)
        {
            try
            {
                var entidadBancaria = Mapping.Mapper.Map<oEntidadBancaria>(model);
                entidadBancaria.ProcesarDatos();

                dEntidadBancaria dEntidadBancaria = new(GetConnectionString());
                await dEntidadBancaria.Modificar(entidadBancaria);

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
                dEntidadBancaria dEntidadBancaria = new(GetConnectionString());
                await dEntidadBancaria.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oEntidadBancaria> GetPorId(int id, bool incluirReferencias = false)
        {
            try
            {
                dEntidadBancaria dEntidadBancaria = new(GetConnectionString());
                var entidadBancaria = await dEntidadBancaria.GetPorId(id);

                if (incluirReferencias)
                {
                    entidadBancaria.TipoEntidadBancaria = dTipoEntidadBancaria.GetPorId(entidadBancaria.Tipo);
                }

                return entidadBancaria;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null!;
            }
        }

        public async Task<oPagina<vEntidadBancaria>> Listar(string nombre, oPaginacion paginacion)
        {
            try
            {
                dEntidadBancaria dEntidadBancaria = new(GetConnectionString());
                return await dEntidadBancaria.Listar(nombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null!;
            }
        }

        public async Task<bool> Existe(int id) => await new dEntidadBancaria(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(int? id, string nombre) => await new dEntidadBancaria(GetConnectionString()).DatosRepetidos(id, nombre);

        public static object FormularioTablas()
        {
            var tiposEntidadesBancarias = dTipoEntidadBancaria.ListarTodos();

            return new
            {
                tiposEntidadesBancarias
            };
        }
    }
}
