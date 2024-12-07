using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Mantenimiento;

namespace BarcoAzul.Api.Logica.Mantenimiento
{
    public class bTipoCambio : bComun, ILogicaService
    {
        public bTipoCambio(oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Tipo de Cambio", datosUsuario) { }

        public async Task<bool> Registrar(oTipoCambio model)
        {
            try
            {
                model.UsuarioId = _datosUsuario.Id;
                model.Origen = "SISTEMA-WEB";

                dTipoCambio dTipoCambio = new(GetConnectionString());
                await dTipoCambio.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Modificar(oTipoCambio model)
        {
            try
            {
                model.UsuarioId = _datosUsuario.Id;

                dTipoCambio dTipoCambio = new(GetConnectionString());
                await dTipoCambio.Modificar(model);

                return true;

            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Modificar);
                return false;
            }
        }

        public async Task<bool> Eliminar(DateTime id)
        {
            try
            {
                dTipoCambio dTipoCambio = new(GetConnectionString());
                await dTipoCambio.Eliminar(id);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oTipoCambio> GetPorId(DateTime id)
        {
            try
            {
                dTipoCambio dTipoCambio = new(GetConnectionString());
                return await dTipoCambio.GetPorId(id);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null!;
            }
        }

        public async Task<oPagina<oTipoCambio>> Listar(int? anio, int? mes, oPaginacion paginacion)
        {
            try
            {
                anio ??= DateTime.Today.Year;

                dTipoCambio dTipoCambio = new(GetConnectionString());
                return await dTipoCambio.Listar(anio.Value, mes, paginacion);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null!;
            }
        }

        public async Task<bool> Existe(DateTime id) => await new dTipoCambio(GetConnectionString()).Existe(id);
    }
}
