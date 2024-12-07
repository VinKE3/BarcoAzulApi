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
    public class bCuentaCorriente : bComun, ILogicaService
    {
        public bCuentaCorriente(oConfiguracionGlobal configuracionGlobal, oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Cuenta Corriente", datosUsuario, configuracionGlobal) { }

        public async Task<bool> Registrar(CuentaCorrienteDTO model)
        {
            try
            {
                var cuentaCorriente = Mapping.Mapper.Map<oCuentaCorriente>(model);
                cuentaCorriente.ProcesarDatos();

                dCuentaCorriente dCuentaCorriente = new(GetConnectionString());
                cuentaCorriente.EmpresaId = _configuracionGlobal.EmpresaId;
                cuentaCorriente.CuentaCorrienteId = await dCuentaCorriente.GetNuevoId(cuentaCorriente.EmpresaId);
                cuentaCorriente.UsuarioId = _datosUsuario.Id;

                await dCuentaCorriente.Registrar(cuentaCorriente);

                model.EmpresaId = cuentaCorriente.EmpresaId;
                model.CuentaCorrienteId = cuentaCorriente.CuentaCorrienteId;

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(CuentaCorrienteDTO model)
        {
            try
            {
                var cuentaCorriente = Mapping.Mapper.Map<oCuentaCorriente>(model);
                cuentaCorriente.ProcesarDatos();

                dCuentaCorriente dCuentaCorriente = new(GetConnectionString());
                cuentaCorriente.UsuarioId = _datosUsuario.Id;

                await dCuentaCorriente.Modificar(cuentaCorriente);

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
                dCuentaCorriente dCuentaCorriente = new(GetConnectionString());
                await dCuentaCorriente.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oCuentaCorriente> GetPorId(string id, bool incluirReferencias = false)
        {
            try
            {
                dCuentaCorriente dCuentaCorriente = new(GetConnectionString());
                var cuentaCorriente = await dCuentaCorriente.GetPorId(id);

                if (incluirReferencias)
                {
                    cuentaCorriente.EntidadBancaria = await new dEntidadBancaria(GetConnectionString()).GetPorId(cuentaCorriente.EntidadBancariaId);
                    cuentaCorriente.Moneda = dMoneda.GetPorId(cuentaCorriente.MonedaId);
                }

                return cuentaCorriente;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<oPagina<vCuentaCorriente>> Listar(string numero, oPaginacion paginacion)
        {
            try
            {
                dCuentaCorriente dCuentaCorriente = new(GetConnectionString());
                return await dCuentaCorriente.Listar(numero ?? string.Empty, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<bool> Existe(string id) => await new dCuentaCorriente(GetConnectionString()).Existe(id);

        public async Task<bool> DatosRepetidos(string id, string numero) => await new dCuentaCorriente(GetConnectionString()).DatosRepetidos(id, numero);

        public async Task<object> FormularioTablas()
        {
            var entidadesBancarias = await new dEntidadBancaria(GetConnectionString()).ListarTodos();
            var monedas = dMoneda.ListarTodos();
            var tiposCuentaBancaria = dTipoCuentaBancaria.ListarTodos();

            return new
            {
                entidadesBancarias,
                monedas,
                tiposCuentaBancaria
            };
        }
    }
}
