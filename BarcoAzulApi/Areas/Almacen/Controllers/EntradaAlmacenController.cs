using BarcoAzul.Api.Logica.Almacen;
using BarcoAzul.Api.Logica;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
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
    public class EntradaAlmacenController : GlobalController
    {
        private readonly bEntradaAlmacen _bEntradaAlmacen;
        private readonly string _origen;

        public EntradaAlmacenController(bEntradaAlmacen bEntradaAlmacen)
        {
            _bEntradaAlmacen = bEntradaAlmacen;
            _origen = "Entrada Almacén";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.EntradaAlmacen, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(EntradaAlmacenDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bEntradaAlmacen.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bEntradaAlmacen.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!_bEntradaAlmacen.IsFechaValida(TipoAccion.Registrar, model.FechaEmision))
                {
                    AgregarMensajes(_bEntradaAlmacen.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool registrado = await _bEntradaAlmacen.Registrar(model);
                AgregarMensajes(_bEntradaAlmacen.Mensajes);

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
        [AuthorizeAction(NombresMenus.EntradaAlmacen, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(EntradaAlmacenDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bEntradaAlmacen.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                var (isBloqueado, mensaje) = await _bEntradaAlmacen.IsBloqueado(model.Id);

                if (isBloqueado)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bEntradaAlmacen.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bEntradaAlmacen.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!_bEntradaAlmacen.IsFechaValida(TipoAccion.Modificar, model.FechaEmision))
                {
                    AgregarMensajes(_bEntradaAlmacen.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool modificado = await _bEntradaAlmacen.Modificar(model);
                AgregarMensajes(_bEntradaAlmacen.Mensajes);

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
        [AuthorizeAction(NombresMenus.EntradaAlmacen, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bEntradaAlmacen.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var (isBloqueado, mensaje) = await _bEntradaAlmacen.IsBloqueado(id);

            if (isBloqueado)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool eliminado = await _bEntradaAlmacen.Eliminar(id);
            AgregarMensajes(_bEntradaAlmacen.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente (Documento: {Comun.CompraIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut($"{nameof(Anular)}/{{id}}")]
        [AuthorizeAction(NombresMenus.EntradaAlmacen, UsuarioPermiso.Anular)]
        public async Task<IActionResult> Anular(string id)
        {
            if (!await _bEntradaAlmacen.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var (isBloqueado, mensaje) = await _bEntradaAlmacen.IsBloqueado(id);

            if (isBloqueado)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool anulado = await _bEntradaAlmacen.Anular(id);
            AgregarMensajes(_bEntradaAlmacen.Mensajes);

            if (anulado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: anulada exitosamente (Documento: {Comun.CompraIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut($"{nameof(Cerrar)}/{{id}}")]
        [AuthorizeAction(NombresMenus.EntradaAlmacen, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Cerrar(string id)
        {
            if (!await _bEntradaAlmacen.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool cerrado = await _bEntradaAlmacen.Cerrar(id);
            AgregarMensajes(_bEntradaAlmacen.Mensajes);

            if (cerrado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: cerrada exitosamente (Documento: {Comun.CompraIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.EntradaAlmacen, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bEntradaAlmacen.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var entradaAlmacen = await _bEntradaAlmacen.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bEntradaAlmacen.Mensajes);

            if (entradaAlmacen is not null)
            {
                return Ok(GenerarRespuesta(true, entradaAlmacen));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.EntradaAlmacen, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, string observacion, [FromQuery] oPaginacion paginacion)
        {
            var entradasAlmacen = await _bEntradaAlmacen.Listar(fechaInicio, fechaFin, observacion, paginacion);
            AgregarMensajes(_bEntradaAlmacen.Mensajes);

            if (entradasAlmacen is not null)
            {
                return Ok(GenerarRespuesta(true, entradasAlmacen));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.EntradaAlmacen, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bEntradaAlmacen.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpGet(nameof(IsPermitido))]
        public async Task<IActionResult> IsPermitido(TipoAccion accion, string id = "")
        {
            if (accion == TipoAccion.Modificar || accion == TipoAccion.Eliminar || accion == TipoAccion.Anular)
            {
                if (!Comun.IsCompraIdValido(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el ID no es válido."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bEntradaAlmacen.Existe(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return Ok(GenerarRespuesta(true, false));
                }

                var (isBloqueado, mensaje) = await _bEntradaAlmacen.IsBloqueado(id);

                if (isBloqueado)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                    return Ok(GenerarRespuesta(true, false));
                }

                var entradaAlmacen = await _bEntradaAlmacen.GetPorId(id);

                if (!_bEntradaAlmacen.IsFechaValida(accion, entradaAlmacen.FechaEmision))
                {
                    AgregarMensajes(_bEntradaAlmacen.Mensajes);
                    return Ok(GenerarRespuesta(true, false));
                }
            }

            return Ok(GenerarRespuesta(true, true));
        }
    }
}
