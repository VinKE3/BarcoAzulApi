using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bProveedor : bComun, ILogicaService
    {
        public bProveedor(oDatosUsuario datosUsuario, IConnectionManager connectionManager) : base(connectionManager, origen: "Proveedor", datosUsuario) { }

        public async Task<bool> Registrar(ProveedorDTO model)
        {
            try
            {
                var proveedor = Mapping.Mapper.Map<oProveedor>(model);

                proveedor.ProcesarDatos();
                proveedor.UsuarioId = _datosUsuario.Id;

                dProveedor dProveedor = new(GetConnectionString());
                proveedor.Id = model.Id = await dProveedor.GetNuevoId();

                await dProveedor.Registrar(proveedor);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(ProveedorDTO model)
        {
            try
            {
                var proveedor = Mapping.Mapper.Map<oProveedor>(model);

                proveedor.ProcesarDatos();
                proveedor.UsuarioId = _datosUsuario.Id;

                dProveedor dProveedor = new(GetConnectionString());
                await dProveedor.Modificar(proveedor);

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
                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dProveedorContacto dProveedorContacto = new(GetConnectionString());
                    await dProveedorContacto.EliminarDeProveedor(id);

                    dProveedorCuentaCorriente dProveedorCuentaCorriente = new(GetConnectionString());
                    await dProveedorCuentaCorriente.EliminarDeProveedor(id);

                    dProveedor dProveedor = new(GetConnectionString());
                    await dProveedor.Eliminar(id);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oProveedor> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dProveedor dProveedor = new(GetConnectionString());
                var proveedor = await dProveedor.GetPorId(id);

                if (incluirReferencias)
                {
                    proveedor.TipoDocumentoIdentidad = await new dTipoDocumentoIdentidad(GetConnectionString()).GetPorId(proveedor.TipoDocumentoIdentidadId);

                    if (!string.IsNullOrWhiteSpace(proveedor.DepartamentoId))
                        proveedor.Departamento = await new dDepartamento(GetConnectionString()).GetPorId(proveedor.DepartamentoId);

                    if (!string.IsNullOrWhiteSpace(proveedor.ProvinciaId))
                        proveedor.Provincia = await new dProvincia(GetConnectionString()).GetPorId(proveedor.DepartamentoId + proveedor.ProvinciaId);

                    if (!string.IsNullOrWhiteSpace(proveedor.DistritoId))
                        proveedor.Distrito = await new dDistrito(GetConnectionString()).GetPorId(proveedor.DepartamentoId + proveedor.ProvinciaId + proveedor.DistritoId);

                    proveedor.Contactos = await new dProveedorContacto(GetConnectionString()).ListarPorProveedor(proveedor.Id);
                    proveedor.CuentasCorrientes = await new dProveedorCuentaCorriente(GetConnectionString()).ListarPorProveedor(proveedor.Id);
                }

                return proveedor;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vProveedor>> Listar(string numeroDocumentoIdentidad, string nombre, oPaginacion paginacion)
        {
            try
            {
                dProveedor dProveedor = new(GetConnectionString());
                return await dProveedor.Listar(numeroDocumentoIdentidad ?? string.Empty, nombre ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dProveedor(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string numeroDocumentoIdentidad) => await new dProveedor(GetConnectionString()).DatosRepetidos(id, numeroDocumentoIdentidad);

        public async Task<object> FormularioTablas()
        {
            var tiposDocumentoIdentidad = await new dTipoDocumentoIdentidad(GetConnectionString()).ListarTodos();
            var departamentos = await new dDepartamento(GetConnectionString()).ListarTodos();
            var provincias = await new dProvincia(GetConnectionString()).ListarTodos();
            var distritos = await new dDistrito(GetConnectionString()).ListarTodos();

            return new
            {
                tiposDocumentoIdentidad,
                departamentos = bUtilidad.ListarDepartamentosProvinciasDistritos(departamentos, provincias, distritos)
            };
        }
    }
}
