using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Repositorio.Empresa;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using System.Transactions;

namespace BarcoAzul.Api.Logica.Empresa
{
    public class bEmpresa : bComun, ILogicaService
    {
        public bEmpresa(IConnectionManager connectionManager) : base(connectionManager, origen: "Empresa - Configuración") { }

        public async Task<bool> Modificar(oConfiguracionEmpresa model)
        {
            try
            {
                model.ProcesarDatos();
                model.CompletarDatosPorcentajesIGV();
                model.CompletarDatosPorcentajesRetencion();
                model.CompletarDatosPorcentajesDetraccion();
                model.CompletarDatosPorcentajesPercepcion();

                using (TransactionScope scope = new(TransactionScopeAsyncFlowOption.Enabled))
                {
                    dEmpresa dEmpresa = new(GetConnectionString());
                    await dEmpresa.Modificar(model);

                    dEmpresaIGV dEmpresaIGV = new(GetConnectionString());
                    await dEmpresaIGV.Eliminar();
                    await dEmpresaIGV.Registrar(model.PorcentajesIGV);

                    dEmpresaRetencion dEmpresaRetencion = new(GetConnectionString());
                    await dEmpresaRetencion.Eliminar();
                    await dEmpresaRetencion.Registrar(model.PorcentajesRetencion);

                    dEmpresaDetraccion dEmpresaDetraccion = new(GetConnectionString());
                    await dEmpresaDetraccion.Eliminar();
                    await dEmpresaDetraccion.Registrar(model.PorcentajesDetraccion);

                    dEmpresaPercepcion dEmpresaPercepcion = new(GetConnectionString());
                    await dEmpresaPercepcion.Eliminar();
                    await dEmpresaPercepcion.Registrar(model.PorcentajesPercepcion);

                    scope.Complete();
                }

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<oConfiguracionEmpresa> Get()
        {
            try
            {
                dEmpresa dEmpresa = new(GetConnectionString());

                var empresa = await dEmpresa.Get();
                empresa.PorcentajesIGV = await new dEmpresaIGV(GetConnectionString()).ListarTodos();
                empresa.PorcentajesRetencion = await new dEmpresaRetencion(GetConnectionString()).ListarTodos();
                empresa.PorcentajesDetraccion = await new dEmpresaDetraccion(GetConnectionString()).ListarTodos();
                empresa.PorcentajesPercepcion = await new dEmpresaPercepcion(GetConnectionString()).ListarTodos();

                return empresa;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task ActualizarFechaFiltro() => await new dEmpresa(GetConnectionString()).ActualizarFechaFiltro();

        public async Task<object> FormularioTablas()
        {
            var departamentos = await new dDepartamento(GetConnectionString()).ListarTodos();
            var provincias = await new dProvincia(GetConnectionString()).ListarTodos();
            var distritos = await new dDistrito(GetConnectionString()).ListarTodos();

            return new
            {
                departamentos = bUtilidad.ListarDepartamentosProvinciasDistritos(departamentos, provincias, distritos)
            };
        }
    }
}
