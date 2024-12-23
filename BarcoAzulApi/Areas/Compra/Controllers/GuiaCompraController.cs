using BarcoAzul.Api.Logica.Compra;
using BarcoAzul.Api.Logica;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
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
    public class GuiaCompraController : GlobalController
    {
        private readonly bGuiaCompra _bGuiaCompra;
        private readonly string _origen;

        public GuiaCompraController(bGuiaCompra bGuiaCompra)
        {
            _bGuiaCompra = bGuiaCompra;
            _origen = "Guía de Compra";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.GuiaCompra, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(GuiaCompraDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model.AfectarStock && !_bGuiaCompra.IsFechaValida(TipoAccion.Registrar, model.FechaEmision))
                {
                    AgregarMensajes(_bGuiaCompra.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bGuiaCompra.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bGuiaCompra.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool registrado = await _bGuiaCompra.Registrar(model);
                AgregarMensajes(_bGuiaCompra.Mensajes);

                if (registrado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: registrada exitosamente (Documento: {model.NumeroDocumento})."));
                    return CreatedAtAction(nameof(GetPorId), new { id = model.Id }, GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.GuiaCompra, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(GuiaCompraDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bGuiaCompra.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bGuiaCompra.IsBloqueado(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (model.AfectarStock && !_bGuiaCompra.IsFechaValida(TipoAccion.Modificar, model.FechaEmision))
                {
                    AgregarMensajes(_bGuiaCompra.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bGuiaCompra.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bGuiaCompra.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool modificado = await _bGuiaCompra.Modificar(model);
                AgregarMensajes(_bGuiaCompra.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificada exitosamente (Documento: {model.NumeroDocumento})."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id}")]
        [AuthorizeAction(NombresMenus.GuiaCompra, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bGuiaCompra.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            if (await _bGuiaCompra.IsBloqueado(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool eliminado = await _bGuiaCompra.Eliminar(id);
            AgregarMensajes(_bGuiaCompra.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente (Documento: {Comun.CompraIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.GuiaCompra, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bGuiaCompra.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var guiaCompra = await _bGuiaCompra.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bGuiaCompra.Mensajes);

            if (guiaCompra is not null)
            {
                return Ok(GenerarRespuesta(true, guiaCompra));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.GuiaCompra, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, string proveedorNombre, [FromQuery] oPaginacion paginacion)
        {
            var guiasCompra = await _bGuiaCompra.Listar(fechaInicio, fechaFin, proveedorNombre, paginacion);
            AgregarMensajes(_bGuiaCompra.Mensajes);

            if (guiasCompra is not null)
            {
                return Ok(GenerarRespuesta(true, guiasCompra));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.GuiaCompra, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bGuiaCompra.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpGet(nameof(IsPermitido))]
        public async Task<IActionResult> IsPermitido(TipoAccion accion, string id = "")
        {
            if (accion == TipoAccion.Modificar || accion == TipoAccion.Eliminar)
            {
                if (!Comun.IsCompraIdValido(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el ID no es válido."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bGuiaCompra.Existe(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (await _bGuiaCompra.IsBloqueado(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return Ok(GenerarRespuesta(true, false));
                }

                var guiaCompra = await _bGuiaCompra.GetPorId(id);

                if (!_bGuiaCompra.IsFechaValida(accion, guiaCompra.FechaEmision))
                {
                    AgregarMensajes(_bGuiaCompra.Mensajes);
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bGuiaCompra.AnioMesHabilitado(guiaCompra.FechaEmision))
                {
                    AgregarMensajes(_bGuiaCompra.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }
            }

            return Ok(GenerarRespuesta(true, true));
        }
    }
}
