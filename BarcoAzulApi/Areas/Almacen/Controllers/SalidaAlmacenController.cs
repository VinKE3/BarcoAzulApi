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
using Microsoft.AspNetCore.StaticFiles;

namespace BarcoAzulApi.Areas.Almacen.Controllers
{
    [ApiController]
    [Area(NombreAreas.Almacen)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SalidaAlmacenController : GlobalController
    {
        private readonly bSalidaAlmacen _bSalidaAlmacen;
        private readonly string _origen;

        public SalidaAlmacenController(bSalidaAlmacen bSalidaAlmacen)
        {
            _bSalidaAlmacen = bSalidaAlmacen;
            _origen = "Salida Almacén";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.SalidaAlmacen, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(SalidaAlmacenDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bSalidaAlmacen.AnioMesHabilitado(model.FechaInicio))
                {
                    AgregarMensajes(_bSalidaAlmacen.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bSalidaAlmacen.StockSuficiente(model))
                {
                    AgregarMensajes(_bSalidaAlmacen.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!_bSalidaAlmacen.IsFechaValida(TipoAccion.Registrar, model.FechaInicio))
                {
                    AgregarMensajes(_bSalidaAlmacen.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool registrado = await _bSalidaAlmacen.Registrar(model);
                AgregarMensajes(_bSalidaAlmacen.Mensajes);

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
        [AuthorizeAction(NombresMenus.SalidaAlmacen, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(SalidaAlmacenDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bSalidaAlmacen.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                var (isBloqueado, mensaje) = await _bSalidaAlmacen.IsBloqueado(model.Id);

                if (isBloqueado)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bSalidaAlmacen.AnioMesHabilitado(model.FechaInicio))
                {
                    AgregarMensajes(_bSalidaAlmacen.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bSalidaAlmacen.StockSuficiente(model))
                {
                    AgregarMensajes(_bSalidaAlmacen.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!_bSalidaAlmacen.IsFechaValida(TipoAccion.Modificar, model.FechaTerminacion))
                {
                    AgregarMensajes(_bSalidaAlmacen.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool modificado = await _bSalidaAlmacen.Modificar(model);
                AgregarMensajes(_bSalidaAlmacen.Mensajes);

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
        [AuthorizeAction(NombresMenus.SalidaAlmacen, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bSalidaAlmacen.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var (isBloqueado, mensaje) = await _bSalidaAlmacen.IsBloqueado(id);

            if (isBloqueado)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool eliminado = await _bSalidaAlmacen.Eliminar(id);
            AgregarMensajes(_bSalidaAlmacen.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente (Documento: {Comun.VentaIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut($"{nameof(Anular)}/{{id}}")]
        [AuthorizeAction(NombresMenus.SalidaAlmacen, UsuarioPermiso.Anular)]
        public async Task<IActionResult> Anular(string id)
        {
            if (!await _bSalidaAlmacen.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var (isBloqueado, mensaje) = await _bSalidaAlmacen.IsBloqueado(id);

            if (isBloqueado)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool anulado = await _bSalidaAlmacen.Anular(id);
            AgregarMensajes(_bSalidaAlmacen.Mensajes);

            if (anulado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: anulada exitosamente (Documento: {Comun.VentaIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet($"{nameof(Imprimir)}/{{id}}")]
        [AuthorizeAction(NombresMenus.SalidaAlmacen, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Imprimir(string id)
        {
            if (!await _bSalidaAlmacen.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var (nombreArchivo, archivo) = await _bSalidaAlmacen.Imprimir(id);
            AgregarMensajes(_bSalidaAlmacen.Mensajes);

            if (archivo is not null)
            {
                MemoryStream stream = new(archivo);

                new FileExtensionContentTypeProvider().TryGetContentType(nombreArchivo, out string contentType);
                contentType ??= "application/octet-stream";

                System.Net.Mime.ContentDisposition cd = new()
                {
                    FileName = nombreArchivo,
                    Inline = true
                };
                Response.Headers.Add("Content-Disposition", cd.ToString());

                return File(stream, contentType, nombreArchivo);
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut($"{nameof(Cerrar)}/{{id}}")]
        [AuthorizeAction(NombresMenus.SalidaAlmacen, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Cerrar(string id)
        {
            if (!await _bSalidaAlmacen.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool cerrado = await _bSalidaAlmacen.Cerrar(id);
            AgregarMensajes(_bSalidaAlmacen.Mensajes);

            if (cerrado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: cerrada exitosamente (Documento: {Comun.VentaIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.SalidaAlmacen, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bSalidaAlmacen.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var salidaAlmacen = await _bSalidaAlmacen.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bSalidaAlmacen.Mensajes);

            if (salidaAlmacen is not null)
            {
                return Ok(GenerarRespuesta(true, salidaAlmacen));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.SalidaAlmacen, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, string numeroDocumento, [FromQuery] oPaginacion paginacion)
        {
            var salidasAlmacen = await _bSalidaAlmacen.Listar(fechaInicio, fechaFin, numeroDocumento, paginacion);
            AgregarMensajes(_bSalidaAlmacen.Mensajes);

            if (salidasAlmacen is not null)
            {
                return Ok(GenerarRespuesta(true, salidasAlmacen));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.SalidaAlmacen, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bSalidaAlmacen.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpGet(nameof(IsPermitido))]
        public async Task<IActionResult> IsPermitido(TipoAccion accion, string id = "")
        {
            if (accion == TipoAccion.Modificar || accion == TipoAccion.Eliminar || accion == TipoAccion.Anular)
            {
                if (!Comun.IsVentaIdValido(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el ID no es válido."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bSalidaAlmacen.Existe(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return Ok(GenerarRespuesta(true, false));
                }

                var (isBloqueado, mensaje) = await _bSalidaAlmacen.IsBloqueado(id);

                if (isBloqueado)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                    return Ok(GenerarRespuesta(true, false));
                }

                var salidaAlmacen = await _bSalidaAlmacen.GetPorId(id);

                if (!_bSalidaAlmacen.IsFechaValida(accion, salidaAlmacen.FechaInicio))
                {
                    AgregarMensajes(_bSalidaAlmacen.Mensajes);
                    return Ok(GenerarRespuesta(true, false));
                }
            }

            return Ok(GenerarRespuesta(true, true));
        }


    }
}
