using BarcoAzul.Api.Logica.Venta;
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

namespace BarcoAzulApi.Areas.Venta.Controllers
{
    [ApiController]
    [Area(NombreAreas.Venta)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class GuiaRemisionController : GlobalController
    {
        private readonly bGuiaRemision _bGuiaRemision;
        private readonly string _origen;

        public GuiaRemisionController(bGuiaRemision bGuiaRemision)
        {
            _bGuiaRemision = bGuiaRemision;
            _origen = "Guía de Remisión";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.GuiaRemision, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(GuiaRemisionDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bGuiaRemision.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bGuiaRemision.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (model.AfectarStock && model.IngresoEgresoStock == "-" && !await _bGuiaRemision.StockSuficiente(model))
                {
                    AgregarMensajes(_bGuiaRemision.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (model.AfectarStock && !_bGuiaRemision.IsFechaValida(TipoAccion.Registrar, model.FechaEmision))
                {
                    AgregarMensajes(_bGuiaRemision.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool registrado = await _bGuiaRemision.Registrar(model);
                AgregarMensajes(_bGuiaRemision.Mensajes);

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
        [AuthorizeAction(NombresMenus.GuiaRemision, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(GuiaRemisionDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bGuiaRemision.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bGuiaRemision.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (model.AfectarStock && model.IngresoEgresoStock == "-" && !await _bGuiaRemision.StockSuficiente(model))
                {
                    AgregarMensajes(_bGuiaRemision.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (model.AfectarStock && !_bGuiaRemision.IsFechaValida(TipoAccion.Modificar, model.FechaEmision))
                {
                    AgregarMensajes(_bGuiaRemision.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool modificado = await _bGuiaRemision.Modificar(model);
                AgregarMensajes(_bGuiaRemision.Mensajes);

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
        [AuthorizeAction(NombresMenus.GuiaRemision, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bGuiaRemision.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            if (await _bGuiaRemision.IsBloqueado(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool eliminado = await _bGuiaRemision.Eliminar(id);
            AgregarMensajes(_bGuiaRemision.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente (Documento: {Comun.VentaIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut($"{nameof(Anular)}/{{id}}")]
        [AuthorizeAction(NombresMenus.GuiaRemision, UsuarioPermiso.Anular)]
        public async Task<IActionResult> Anular(string id)
        {
            if (!await _bGuiaRemision.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            if (await _bGuiaRemision.IsBloqueado(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool anulado = await _bGuiaRemision.Anular(id);
            AgregarMensajes(_bGuiaRemision.Mensajes);

            if (anulado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: anulada exitosamente (Documento: {Comun.VentaIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet($"{nameof(Imprimir)}/{{id}}")]
        [AuthorizeAction(NombresMenus.GuiaRemision, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Imprimir(string id)
        {
            if (!await _bGuiaRemision.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var (nombreArchivo, archivo) = await _bGuiaRemision.Imprimir(id);
            AgregarMensajes(_bGuiaRemision.Mensajes);

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

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.GuiaRemision, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bGuiaRemision.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var guiaRemision = await _bGuiaRemision.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bGuiaRemision.Mensajes);

            if (guiaRemision is not null)
            {
                return Ok(GenerarRespuesta(true, guiaRemision));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.GuiaRemision, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, string clienteNombre, string serie, [FromQuery] oPaginacion paginacion)
        {
            var guiasRemision = await _bGuiaRemision.Listar(fechaInicio, fechaFin, clienteNombre, serie, paginacion);
            AgregarMensajes(_bGuiaRemision.Mensajes);

            if (guiasRemision is not null)
            {
                return Ok(GenerarRespuesta(true, guiasRemision));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.GuiaRemision, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bGuiaRemision.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpGet(nameof(IsPermitido))]
        public async Task<IActionResult> IsPermitido(TipoAccion accion, string id = "")
        {
            if (accion == TipoAccion.Eliminar || accion == TipoAccion.Modificar || accion == TipoAccion.Anular)
            {
                if (!Comun.IsVentaIdValido(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el ID no es válido."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bGuiaRemision.Existe(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return Ok(GenerarRespuesta(true, false));
                }

                var guiaRemision = await _bGuiaRemision.GetPorId(id);

                if (!_bGuiaRemision.IsFechaValida(accion, guiaRemision.FechaEmision))
                {
                    AgregarMensajes(_bGuiaRemision.Mensajes);
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bGuiaRemision.AnioMesHabilitado(guiaRemision.FechaEmision))
                {
                    AgregarMensajes(_bGuiaRemision.Mensajes);
                    return Ok(GenerarRespuesta(true, false));
                }

                if (await _bGuiaRemision.IsBloqueado(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (accion == TipoAccion.Modificar && await _bGuiaRemision.IsAnulado(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está anulado."));
                    return Ok(GenerarRespuesta(true, false));
                }
            }

            return Ok(GenerarRespuesta(true, true));
        }
    }
}
