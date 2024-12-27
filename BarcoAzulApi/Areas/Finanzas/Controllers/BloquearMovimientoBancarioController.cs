using BarcoAzul.Api.Logica.Finanzas;
using BarcoAzul.Api.Modelos.Otros;
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
    public class BloquearMovimientoBancarioController : GlobalController
    {
        private readonly bBloquearMovimientoBancario _bBloquearMovimientoBancario;
        private readonly string _origen;

        public BloquearMovimientoBancarioController(bBloquearMovimientoBancario bBloquearMovimientoBancario)
        {
            _bBloquearMovimientoBancario = bBloquearMovimientoBancario;
            _origen = "Bloquear Movimiento Bancario";
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.BloquearMovimientoBancario, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Procesar(oBloquearMovimientoBancario oBloquearMovimientoBancario)
        {
            if (ModelState.IsValid)
            {
                bool procesado = await _bBloquearMovimientoBancario.Procesar(oBloquearMovimientoBancario);
                AgregarMensajes(_bBloquearMovimientoBancario.Mensajes);

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
        [AuthorizeAction(NombresMenus.BloquearMovimientoBancario, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, [FromQuery] oPaginacion paginacion)
        {
            var movimientosBancarios = await _bBloquearMovimientoBancario.Listar(fechaInicio, fechaFin, paginacion);

            if (movimientosBancarios is not null)
            {
                return Ok(GenerarRespuesta(true, movimientosBancarios));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
