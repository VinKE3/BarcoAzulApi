using BarcoAzul.Api.Logica.Venta;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Venta.Controllers
{
    [ApiController]
    [Area(NombreAreas.Venta)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BloquearVentaController : GlobalController
    {
        private readonly bBloquearVenta _bBloquearVenta;
        private readonly string _origen;

        public BloquearVentaController(bBloquearVenta bBloquearVenta)
        {
            _bBloquearVenta = bBloquearVenta;
            _origen = "Bloquear Venta";
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.BloquearVenta, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Procesar(oBloquearVenta bloquearVenta)
        {
            if (ModelState.IsValid)
            {
                bool procesado = await _bBloquearVenta.Procesar(bloquearVenta);
                AgregarMensajes(_bBloquearVenta.Mensajes);

                if (procesado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: procesado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.BloquearVenta, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string tipoDocumentoId, DateTime? fechaInicio, DateTime? fechaFin, [FromQuery] oPaginacion paginacion)
        {
            var ventas = await _bBloquearVenta.Listar(tipoDocumentoId, fechaInicio, fechaFin, paginacion);

            if (ventas is not null)
            {
                return Ok(GenerarRespuesta(true, ventas));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.BloquearVenta, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bBloquearVenta.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
