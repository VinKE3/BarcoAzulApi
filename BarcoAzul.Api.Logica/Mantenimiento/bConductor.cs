using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bConductor : bComun
    {
        public bConductor(oDatosUsuario datosUsuario, IConnectionManager connectionManager) : base(connectionManager, origen: "Conductor", datosUsuario) { }

        public async Task<bool> Registrar(ConductorDTO model)
        {
            try
            {
                var conductor = Mapping.Mapper.Map<oConductor>(model);

                conductor.ProcesarDatos();
                conductor.UsuarioId = _datosUsuario.Id;

                dConductor dConductor = new(GetConnectionString());
                conductor.Id = model.Id = await dConductor.GetNuevoId();

                await dConductor.Registrar(conductor);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(ConductorDTO model)
        {
            try
            {
                var conductor = Mapping.Mapper.Map<oConductor>(model);
                conductor.ProcesarDatos();
                conductor.UsuarioId = _datosUsuario.Id;

                dConductor dConductor = new(GetConnectionString());
                await dConductor.Modificar(conductor);

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
                dConductor dConductor = new(GetConnectionString());
                await dConductor.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oConductor> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dConductor dConductor = new(GetConnectionString());
                var conductor = await dConductor.GetPorId(id);

                if (incluirReferencias)
                {
                    conductor.EmpresaTransporte = await new dEmpresaTransporte(GetConnectionString()).GetPorId(conductor.EmpresaId + conductor.EmpresaTransporteId);
                   
                    //if(!string.IsNullOrWhiteSpace(conductor.TipoDocumentoIdentidad))
                    //    conductor.TipoDocumentoIdentidad = await new dTipoDocumentoIdentidad(GetConnectionString()).GetPorId(conductor.TipoDocumentoIdentidad);
                    if (!string.IsNullOrWhiteSpace(conductor.DepartamentoId))
                        conductor.Departamento = await new dDepartamento(GetConnectionString()).GetPorId(conductor.DepartamentoId);

                    if (!string.IsNullOrWhiteSpace(conductor.ProvinciaId))
                        conductor.Provincia = await new dProvincia(GetConnectionString()).GetPorId(conductor.DepartamentoId + conductor.ProvinciaId);

                    if (!string.IsNullOrWhiteSpace(conductor.DistritoId))
                        conductor.Distrito = await new dDistrito(GetConnectionString()).GetPorId(conductor.DepartamentoId + conductor.ProvinciaId + conductor.DistritoId);
                }

                return conductor;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vConductor>> Listar(string nombre, oPaginacion paginacion)
        {
            try
            {
                dConductor dConductor = new(GetConnectionString());
                return await dConductor.Listar(nombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dConductor(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string numeroDocumentoIdentidad) => await new dConductor(GetConnectionString()).DatosRepetidos(id, numeroDocumentoIdentidad);

        public async Task<object> FormularioTablas()
        {
            var empresasTransporte = await new dEmpresaTransporte(GetConnectionString()).ListarTodos();
            var tiposDocumentoIdentidad = await new dTipoDocumentoIdentidad(GetConnectionString()).ListarTodos();
            var departamentos = await new dDepartamento(GetConnectionString()).ListarTodos();
            var provincias = await new dProvincia(GetConnectionString()).ListarTodos();
            var distritos = await new dDistrito(GetConnectionString()).ListarTodos();
            // Definir el array de tipoTransportista
            var tiposTransportista = new[]
            {
        new { id = "01", descripcion = "01 TRANSPORTISTA" },
        new { id = "02", descripcion = "02 CONDUCTOR" }
    };
            return new
            {
                empresasTransporte,
                tiposDocumentoIdentidad,
                departamentos = bUtilidad.ListarDepartamentosProvinciasDistritos(departamentos, provincias, distritos),
                tiposTransportista // Agregar el array al resultado
            };
        }
    }
}
