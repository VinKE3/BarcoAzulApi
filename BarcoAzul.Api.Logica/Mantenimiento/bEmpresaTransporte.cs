using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bEmpresaTransporte : bComun, ILogicaService
    {
        public bEmpresaTransporte(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Empresa de Transporte", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(EmpresaTransporteDTO model)
        {
            try
            {
                var empresaTransporte = Mapping.Mapper.Map<oEmpresaTransporte>(model);

                empresaTransporte.ProcesarDatos();
                empresaTransporte.EmpresaId = model.EmpresaId = _configuracionGlobal.EmpresaId;
                empresaTransporte.UsuarioId = _datosUsuario.Id;

                dEmpresaTransporte dEmpresaTransporte = new(GetConnectionString());
                empresaTransporte.EmpresaTransporteId = model.EmpresaTransporteId = await dEmpresaTransporte.GetNuevoId(empresaTransporte.EmpresaId);

                await dEmpresaTransporte.Registrar(empresaTransporte);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(EmpresaTransporteDTO model)
        {
            try
            {
                var empresaTransporte = Mapping.Mapper.Map<oEmpresaTransporte>(model);

                empresaTransporte.ProcesarDatos();
                empresaTransporte.UsuarioId = _datosUsuario.Id;

                dEmpresaTransporte dEmpresaTransporte = new(GetConnectionString());
                await dEmpresaTransporte.Modificar(empresaTransporte);

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
                dEmpresaTransporte dEmpresaTransporte = new(GetConnectionString());
                await dEmpresaTransporte.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oEmpresaTransporte> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dEmpresaTransporte dEmpresaTransporte = new(GetConnectionString());
                var empresaTransporte = await dEmpresaTransporte.GetPorId(id);

                if (incluirReferencias)
                {
                    if (!string.IsNullOrWhiteSpace(empresaTransporte.DepartamentoId))
                        empresaTransporte.Departamento = await new dDepartamento(GetConnectionString()).GetPorId(empresaTransporte.DepartamentoId);

                    if (!string.IsNullOrWhiteSpace(empresaTransporte.ProvinciaId))
                        empresaTransporte.Provincia = await new dProvincia(GetConnectionString()).GetPorId(empresaTransporte.DepartamentoId + empresaTransporte.ProvinciaId);

                    if (!string.IsNullOrWhiteSpace(empresaTransporte.DistritoId))
                        empresaTransporte.Distrito = await new dDistrito(GetConnectionString()).GetPorId(empresaTransporte.DepartamentoId + empresaTransporte.ProvinciaId + empresaTransporte.DistritoId);
                }

                return empresaTransporte;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vEmpresaTransporte>> Listar(string nombre, oPaginacion paginacion)
        {
            try
            {
                dEmpresaTransporte dEmpresaTransporte = new(GetConnectionString());
                return await dEmpresaTransporte.Listar(nombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dEmpresaTransporte(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string numeroDocumentoIdentidad) => await new dEmpresaTransporte(GetConnectionString()).DatosRepetidos(id, numeroDocumentoIdentidad);
    }
}
