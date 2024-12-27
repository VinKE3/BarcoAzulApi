using BarcoAzul.Api.Logica.Compra;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Compra.Controllers
{
    [ApiController]
    [Area(NombreAreas.Compra)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BloquearCompraController : GlobalController
    {
        private readonly bBloquearCompra _bBloquearCompra;
        private readonly string _origen;

        public BloquearCompraController(bBloquearCompra bBloquearCompra)
        {
            _bBloquearCompra = bBloquearCompra;
            _origen = "Bloquear Compra";
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.BloquearCompra, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Procesar(oBloquearCompra bloquearCompra)
        {
            if (ModelState.IsValid)
            {
                bool procesado = await _bBloquearCompra.Procesar(bloquearCompra);
                AgregarMensajes(_bBloquearCompra.Mensajes);

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
        [AuthorizeAction(NombresMenus.BloquearCompra, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string tipoDocumentoId, DateTime? fechaInicio, DateTime? fechaFin, [FromQuery] oPaginacion paginacion)
        {
            var compras = await _bBloquearCompra.Listar(tipoDocumentoId, fechaInicio, fechaFin, paginacion);

            if (compras is not null)
            {
                return Ok(GenerarRespuesta(true, compras));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.BloquearCompra, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bBloquearCompra.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
