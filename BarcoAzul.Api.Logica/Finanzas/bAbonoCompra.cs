using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Finanzas;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Otros;

namespace BarcoAzul.Api.Logica.Finanzas
{
    public class bAbonoCompra : bComun
    {
        public bAbonoCompra(oDatosUsuario datosUsuario, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Abono de Compra", datosUsuario) { }

        public async Task<bool> Registrar(oAbonoCompra model)
        {
            try
            {
                dAbonoCompra dAbonoCompra = new(GetConnectionString());

                model.UsuarioId = _datosUsuario.Id;
                model.AbonoId = await dAbonoCompra.GetNuevoId(model.Id);
                model.IsBloqueado = false;
                model.ProcesarDatos();

                await dAbonoCompra.Registrar(model);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Registrar);
                return false;
            }
        }

        public async Task<bool> Eliminar(string compraId, int abonoId)
        {
            try
            {
                dAbonoCompra dAbonoCompra = new(GetConnectionString());
                await dAbonoCompra.Eliminar(compraId, abonoId);

                return true;
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Eliminar);
                return false;
            }
        }

        public async Task<oAbonoCompra> GetPorId(string compraId, int abonoId)
        {
            try
            {
                dAbonoCompra dAbonoCompra = new(GetConnectionString());
                return await dAbonoCompra.GetPorId(compraId, abonoId);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<bool> Existe(string compraId, int abonoId) => await new dAbonoCompra(GetConnectionString()).Existe(compraId, abonoId);

        public async Task<bool> IsBloqueado(string compraId, int abonoId) => await new dAbonoCompra(GetConnectionString()).IsBloqueado(compraId, abonoId);

        public async Task<object> FormularioTablas()
        {
            var monedas = dMoneda.ListarTodos();
            var cuentasCorrientes = await new dCuentaCorriente(GetConnectionString()).ListarTodos();
            var tiposPago = new[]
            {
                new { Texto = "EFECTIVO", Valor = "EF" },
                new { Texto = "DEPOSITO", Valor = "DE" },
                new { Texto = "TRANSFERENCIA", Valor = "TR" }
            };

            return new
            {
                monedas,
                cuentasCorrientes,
                tiposPago
            };
        }
    }
}
