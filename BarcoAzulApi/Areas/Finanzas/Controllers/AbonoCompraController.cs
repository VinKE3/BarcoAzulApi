using BarcoAzul.Api.Logica.Finanzas;
using BarcoAzul.Api.Logica;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Finanzas.Controllers
{
    [ApiController]
    [Area(NombreAreas.Finanzas)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AbonoCompraController : GlobalController
    {
        private readonly bAbonoCompra _bAbonoCompra;
        private readonly IConnectionManager _connectionManager;
        private readonly string _origen;

        public AbonoCompraController(bAbonoCompra bAbonoCompra, IConnectionManager connectionManager)
        {
            _bAbonoCompra = bAbonoCompra;
            _connectionManager = connectionManager;
            _origen = "Abono de Compra";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.CuentaPorPagar, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Registrar(oAbonoCompra model)
        {
            if (ModelState.IsValid)
            {
                bCuentaPorPagar bCuentaPorPagar = new(null, _connectionManager);

                if (await bCuentaPorPagar.IsBloqueado(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool registrado = await _bAbonoCompra.Registrar(model);
                AgregarMensajes(_bAbonoCompra.Mensajes);

                if (registrado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: registrado exitosamente."));
                    return CreatedAtAction(nameof(GetPorId), new { compraId = model.Id, abonoId = model.AbonoId }, GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{compraId}/{abonoId:int}")]
        [AuthorizeAction(NombresMenus.CuentaPorPagar, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string compraId, int abonoId)
        {
            bCuentaPorPagar bCuentaPorPagar = new(null, _connectionManager);

            if (await bCuentaPorPagar.IsBloqueado(compraId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            if (!await _bAbonoCompra.Existe(compraId, abonoId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el abono buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            if (await _bAbonoCompra.IsBloqueado(compraId, abonoId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el abono está bloqueado."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool eliminado = await _bAbonoCompra.Eliminar(compraId, abonoId);
            AgregarMensajes(_bAbonoCompra.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{compraId}/{abonoId:int}")]
        [AuthorizeAction(NombresMenus.CuentaPorPagar, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string compraId, int abonoId)
        {
            if (!await _bAbonoCompra.Existe(compraId, abonoId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var abonoCompra = await _bAbonoCompra.GetPorId(compraId, abonoId);
            AgregarMensajes(_bAbonoCompra.Mensajes);

            if (abonoCompra is not null)
            {
                return Ok(GenerarRespuesta(true, abonoCompra));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.CuentaPorPagar, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bAbonoCompra.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpGet(nameof(IsPermitido))]
        public async Task<IActionResult> IsPermitido(TipoAccion accion, string compraId, int? abonoId = null)
        {
            if (accion == TipoAccion.Registrar || accion == TipoAccion.Eliminar)
            {
                if (!Comun.IsCompraIdValido(compraId))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el ID no es válido."));
                    return Ok(GenerarRespuesta(true, false));
                }

                bCuentaPorPagar bCuentaPorPagar = new(null, _connectionManager);

                if (await bCuentaPorPagar.IsBloqueado(compraId))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (accion == TipoAccion.Eliminar && abonoId is not null && await _bAbonoCompra.IsBloqueado(compraId, abonoId.Value))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el abono está bloqueado."));
                    return Ok(GenerarRespuesta(true, false));
                }
            }

            return Ok(GenerarRespuesta(true, true));
        }
    }
}
