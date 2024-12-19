using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bVehiculo : bComun, ILogicaService
    {
        public bVehiculo(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager) : base(connectionManager, origen: "Vehículo", configuracionGlobal: configuracionGlobal) { }

        public async Task<bool> Registrar(VehiculoDTO model)
        {
            try
            {
                var vehiculo = Mapping.Mapper.Map<oVehiculo>(model);
                vehiculo.ProcesarDatos();

                dVehiculo dVehiculo = new(GetConnectionString());
                vehiculo.Id = model.Id = await dVehiculo.GetNuevoId();
                vehiculo.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;

                await dVehiculo.Registrar(vehiculo);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(VehiculoDTO model)
        {
            try
            {
                var vehiculo = Mapping.Mapper.Map<oVehiculo>(model);
                vehiculo.ProcesarDatos();

                dVehiculo dVehiculo = new(GetConnectionString());
                await dVehiculo.Modificar(vehiculo);

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
                dVehiculo dVehiculo = new(GetConnectionString());
                await dVehiculo.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oVehiculo> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dVehiculo dVehiculo = new(GetConnectionString());
                var vehiculo = await dVehiculo.GetPorId(id);

                if (incluirReferencias)
                {
                    vehiculo.EmpresaTransporte = await new dEmpresaTransporte(GetConnectionString()).GetPorId(vehiculo.EmpresaId + vehiculo.EmpresaTransporteId);
                }

                return vehiculo;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vVehiculo>> Listar(string numeroPlaca, oPaginacion paginacion)
        {
            try
            {
                dVehiculo dVehiculo = new(GetConnectionString());
                return await dVehiculo.Listar(numeroPlaca ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dVehiculo(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string numeroPlaca) => await new dVehiculo(GetConnectionString()).DatosRepetidos(id, numeroPlaca);

        public async Task<object> FormularioTablas()
        {
            var empresasTransporte = await new dEmpresaTransporte(GetConnectionString()).ListarTodos();

            return new
            {
                empresasTransporte
            };
        }
    }
}
