using BarcoAzul.Api.Logica.Finanzas;
using BarcoAzul.Api.Logica;
using BarcoAzul.Api.Modelos.DTOs;
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
    public class MovimientoBancarioController : GlobalController
    {
        private readonly bMovimientoBancario _bMovimientoBancario;
        private readonly string _origen;

        public MovimientoBancarioController(bMovimientoBancario bMovimientoBancario)
        {
            _bMovimientoBancario = bMovimientoBancario;
            _origen = "Movimiento Bancario";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.MovimientoBancario, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(MovimientoBancarioDTO model)
        {
            if (ModelState.IsValid)
            {
                bool registrado = await _bMovimientoBancario.Registrar(model);
                AgregarMensajes(_bMovimientoBancario.Mensajes);

                if (registrado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: registrado exitosamente."));
                    return CreatedAtAction(nameof(GetPorId), new { id = model.Id }, GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.MovimientoBancario, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(MovimientoBancarioDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bMovimientoBancario.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bMovimientoBancario.IsBloqueado(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool modificado = await _bMovimientoBancario.Modificar(model);
                AgregarMensajes(_bMovimientoBancario.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id}")]
        [AuthorizeAction(NombresMenus.MovimientoBancario, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bMovimientoBancario.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            if (await _bMovimientoBancario.IsBloqueado(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool eliminado = await _bMovimientoBancario.Eliminar(id);
            AgregarMensajes(_bMovimientoBancario.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.MovimientoBancario, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bMovimientoBancario.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var movimientoBancario = await _bMovimientoBancario.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bMovimientoBancario.Mensajes);

            if (movimientoBancario is not null)
            {
                return Ok(GenerarRespuesta(true, movimientoBancario));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.MovimientoBancario, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, string cuentaCorrienteId, string tipoMovimientoId, string concepto, [FromQuery] oPaginacion paginacion)
        {
            var movimientosBancarios = await _bMovimientoBancario.Listar(fechaInicio, fechaFin, cuentaCorrienteId, tipoMovimientoId, concepto, paginacion);
            AgregarMensajes(_bMovimientoBancario.Mensajes);

            if (movimientosBancarios is not null)
            {
                return Ok(GenerarRespuesta(true, movimientosBancarios));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FiltroTablas))]
        [AuthorizeAction(NombresMenus.MovimientoBancario, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> FiltroTablas()
        {
            var tablas = await _bMovimientoBancario.FiltroTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.MovimientoBancario, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bMovimientoBancario.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpGet(nameof(IsPermitido))]
        public async Task<IActionResult> IsPermitido(TipoAccion accion, string id = "")
        {
            if (accion == TipoAccion.Modificar || accion == TipoAccion.Eliminar)
            {
                if (!Comun.IsMovimientoBancarioIdValido(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el ID no es válido."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bMovimientoBancario.Existe(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (await _bMovimientoBancario.IsBloqueado(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return Ok(GenerarRespuesta(true, false));
                }

                var (tieneOtroOrigen, cuentaCorrienteInfo) = await _bMovimientoBancario.TieneOtroOrigen(id);

                if (tieneOtroOrigen)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: El registro ha sido generado desde la cuenta bancaria: {cuentaCorrienteInfo}. Si desea modificar, hágalo desde la cuenta bancaria del cuál se realizó la transacción."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (accion == TipoAccion.Modificar && !await _bMovimientoBancario.IsModificable(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro no es modificable."));
                    return Ok(GenerarRespuesta(true, false));
                }

                //if (accion == TipoAccion.Eliminar)
                //{
                //    var (tienePlanillaRelacionada, numeroPlanilla) = await _bMovimientoBancario.TienePlanillaRelacionada(id);

                //    if (tienePlanillaRelacionada)
                //    {
                //        AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está relacionado con la planilla {numeroPlanilla.Right(8)}."));
                //        return Ok(GenerarRespuesta(true, false));
                //    }
                //}
            }

            return Ok(GenerarRespuesta(true, true));
        }
    }
}
