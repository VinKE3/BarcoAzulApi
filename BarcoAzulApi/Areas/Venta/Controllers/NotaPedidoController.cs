using BarcoAzul.Api.Repositorio.Venta;
using BarcoAzul.Api.Logica.Venta;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Utilidades;
using Microsoft.AspNetCore.StaticFiles;
using BarcoAzul.Api.Logica;

namespace BarcoAzulApi.Areas.Venta.Controllers
{
    [ApiController]
    [Area(NombreAreas.Venta)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class NotaPedidoController : GlobalController
    {
        private readonly bNotaPedido _bNotaPedido;
        private readonly string _origen;

        public NotaPedidoController(bNotaPedido bNotaPedido)
        {
            _bNotaPedido = bNotaPedido;
            _origen = "NotaPedido";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.NotaPedido, UsuarioPermiso.Registrar)]

        public async Task<IActionResult> Registrar(NotaPedidoDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bNotaPedido.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bNotaPedido.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool registrado = await _bNotaPedido.Registrar(model);
                AgregarMensajes(_bNotaPedido.Mensajes);

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
        [AuthorizeAction(NombresMenus.NotaPedido, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(NotaPedidoDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bNotaPedido.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bNotaPedido.IsBloqueado(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bNotaPedido.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bNotaPedido.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool modificado = await _bNotaPedido.Modificar(model);
                AgregarMensajes(_bNotaPedido.Mensajes);

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
        [AuthorizeAction(NombresMenus.NotaPedido, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bNotaPedido.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bNotaPedido.Eliminar(id);
            AgregarMensajes(_bNotaPedido.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente (Documento: {Comun.VentaIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut($"{nameof(Anular)}/{{id}}")]
        [AuthorizeAction(NombresMenus.NotaPedido, UsuarioPermiso.Anular)]
        public async Task<IActionResult> Anular(string id)
        {
            if (!await _bNotaPedido.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            if (await _bNotaPedido.IsBloqueado(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool anulado = await _bNotaPedido.Anular(id);
            AgregarMensajes(_bNotaPedido.Mensajes);

            if (anulado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: anulada exitosamente (Documento: {Comun.VentaIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }


        [HttpGet($"{nameof(Imprimir)}/{{id}}")]
        [AuthorizeAction(NombresMenus.NotaPedido, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Imprimir(string id)
        {
            if (!await _bNotaPedido.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var (nombreArchivo, archivo) = await _bNotaPedido.Imprimir(id);
            AgregarMensajes(_bNotaPedido.Mensajes);

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
        [AuthorizeAction(NombresMenus.NotaPedido, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bNotaPedido.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var notaPedido = await _bNotaPedido.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bNotaPedido.Mensajes);

            if (notaPedido is not null)
            {
                return Ok(GenerarRespuesta(true, notaPedido));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.NotaPedido, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, string clienteNombre, [FromQuery] oPaginacion paginacion)
        {
            var notasPedido = await _bNotaPedido.Listar(fechaInicio, fechaFin, clienteNombre, paginacion);
            AgregarMensajes(_bNotaPedido.Mensajes);

            if (notasPedido is not null)
            {
                return Ok(GenerarRespuesta(true, notasPedido));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.NotaPedido, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bNotaPedido.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpGet(nameof(IsPermitido))]
        public async Task<IActionResult> IsPermitido(TipoAccion accion, string id = "")
        {
            if (accion == TipoAccion.Modificar || accion == TipoAccion.Anular)
            {
                if (!Comun.IsVentaIdValido(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el ID no es válido."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bNotaPedido.Existe(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (await _bNotaPedido.IsAnulado(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está anulado."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (await _bNotaPedido.IsBloqueado(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (await _bNotaPedido.IsFacturado(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro ha sido facturado."));
                    return Ok(GenerarRespuesta(true, false));
                }

                var notaPedido = await _bNotaPedido.GetPorId(id);

                if (!await _bNotaPedido.AnioMesHabilitado(notaPedido.FechaEmision))
                {
                    AgregarMensajes(_bNotaPedido.Mensajes);
                    return Ok(GenerarRespuesta(true, false));
                }
            }

            return Ok(GenerarRespuesta(true, true));
        }


    }
}
