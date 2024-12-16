using BarcoAzul.Api.Logica.Informes.Articulos;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Almacen.Controllers
{
    [ApiController]
    [Area(NombreAreas.Almacen)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MovimientoArticuloController : GlobalController
    {
        private readonly bMovimientoArticulo _bMovimientoArticulo;
        private readonly string _origen;

        public MovimientoArticuloController(bMovimientoArticulo bMovimientoArticulo)
        {
            _bMovimientoArticulo = bMovimientoArticulo;
            _origen = "Movimiento de Artículo";
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.MovimientoArticulo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin)
        {
            var movimientosArticulos = await _bMovimientoArticulo.Listar(fechaInicio, fechaFin, "");
            AgregarMensajes(_bMovimientoArticulo.Mensajes);

            if (movimientosArticulos is not null)
            {
                return Ok(GenerarRespuesta(true, movimientosArticulos));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(GetKardexArticulo))]
        [AuthorizeAction(NombresMenus.MovimientoArticulo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetKardexArticulo(string id, DateTime? fechaInicio, DateTime? fechaFin)
        {
            var kardexArticulo = await _bMovimientoArticulo.GetKardexArticulo(id, fechaInicio, fechaFin);
            AgregarMensajes(_bMovimientoArticulo.Mensajes);

            if (kardexArticulo is not null)
            {
                return Ok(GenerarRespuesta(true, kardexArticulo));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.MovimientoArticulo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bMovimientoArticulo.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
