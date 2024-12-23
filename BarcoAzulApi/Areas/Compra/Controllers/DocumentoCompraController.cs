using BarcoAzul.Api.Logica;
using BarcoAzul.Api.Logica.Compra;
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
    public class DocumentoCompraController : GlobalController
    {
        private readonly bDocumentoCompra _bDocumentoCompra;
        private readonly string _origen;

        public DocumentoCompraController(bDocumentoCompra bDocumentoCompra)
        {
            _bDocumentoCompra = bDocumentoCompra;
            _origen = "Documento de Compra";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.DocumentoCompra, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(DocumentoCompraDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model.AfectarStock && !_bDocumentoCompra.IsFechaValida(TipoAccion.Registrar, model.FechaEmision))
                {
                    AgregarMensajes(_bDocumentoCompra.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bDocumentoCompra.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bDocumentoCompra.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool registrado = await _bDocumentoCompra.Registrar(model);
                AgregarMensajes(_bDocumentoCompra.Mensajes);

                if (registrado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: registrado exitosamente (Documento: {model.NumeroDocumento})."));
                    return CreatedAtAction(nameof(GetPorId), new { id = model.Id }, GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.DocumentoCompra, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(DocumentoCompraDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bDocumentoCompra.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                (bool isBloqueado, string mensaje) = await _bDocumentoCompra.IsBloqueado(model.Id);

                if (isBloqueado)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (model.AfectarStock && !_bDocumentoCompra.IsFechaValida(TipoAccion.Modificar, model.FechaEmision))
                {
                    AgregarMensajes(_bDocumentoCompra.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bDocumentoCompra.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bDocumentoCompra.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool modificado = await _bDocumentoCompra.Modificar(model);
                AgregarMensajes(_bDocumentoCompra.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente (Documento: {model.NumeroDocumento})."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id}")]
        [AuthorizeAction(NombresMenus.DocumentoCompra, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bDocumentoCompra.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            (bool isBloqueado, string mensaje) = await _bDocumentoCompra.IsBloqueado(id);

            if (isBloqueado)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool eliminado = await _bDocumentoCompra.Eliminar(id);
            AgregarMensajes(_bDocumentoCompra.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente (Documento: {Comun.CompraIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.DocumentoCompra, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bDocumentoCompra.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var documentoCompra = await _bDocumentoCompra.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bDocumentoCompra.Mensajes);

            if (documentoCompra is not null)
            {
                return Ok(GenerarRespuesta(true, documentoCompra));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.DocumentoCompra, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, string proveedorNombre, [FromQuery] oPaginacion paginacion)
        {
            var documentosCompra = await _bDocumentoCompra.Listar(fechaInicio, fechaFin, proveedorNombre, paginacion);
            AgregarMensajes(_bDocumentoCompra.Mensajes);

            if (documentosCompra is not null)
            {
                return Ok(GenerarRespuesta(true, documentosCompra));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        //TODO: Revisar como validar que el usuario tenga permisos para usar esta opción
        [HttpGet(nameof(ListarPendientes))]
        public async Task<IActionResult> ListarPendientes(DateTime? fechaInicio, DateTime? fechaFin, string proveedorId, [FromQuery] oPaginacion paginacion)
        {
            if (string.IsNullOrWhiteSpace(proveedorId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, "El ID del proveedor es requerido."));
                return BadRequest(GenerarRespuesta(false));
            }

            var documentosCompra = await _bDocumentoCompra.ListarPendientes(fechaInicio, fechaFin, proveedorId, paginacion);
            AgregarMensajes(_bDocumentoCompra.Mensajes);

            if (documentosCompra is not null)
            {
                return Ok(GenerarRespuesta(true, documentosCompra));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.DocumentoCompra, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bDocumentoCompra.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        //[HttpGet(nameof(GetDocumentosReferencia))]
        //[AuthorizeAction(NombresMenus.DocumentoCompra, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        //public async Task<IActionResult> GetDocumentosReferencia(string proveedorId)
        //{
        //    if (string.IsNullOrWhiteSpace(proveedorId))
        //    {
        //        AgregarMensaje(new oMensaje(MensajeTipo.Error, "El ID del proveedor es requerido."));
        //        return BadRequest(GenerarRespuesta(false));
        //    }

        //    bDocumentoReferencia bDocumentoReferencia = new(_bDocumentoCompra.ConnectionManager);
        //    var documentosReferencia = await bDocumentoReferencia.ListarPorProveedor(proveedorId);

        //    return Ok(GenerarRespuesta(true, documentosReferencia));
        //}

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

                if (!await _bDocumentoCompra.Existe(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return Ok(GenerarRespuesta(true, false));
                }

                var documentoCompra = await _bDocumentoCompra.GetPorId(id);

                if (!_bDocumentoCompra.IsFechaValida(accion, documentoCompra.FechaContable))
                {
                    AgregarMensajes(_bDocumentoCompra.Mensajes);
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bDocumentoCompra.AnioMesHabilitado(documentoCompra.FechaContable))
                {
                    AgregarMensajes(_bDocumentoCompra.Mensajes);
                    return Ok(GenerarRespuesta(true, false));
                }

                var (isBloqueado, mensaje) = await _bDocumentoCompra.IsBloqueado(id);

                if (isBloqueado)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                    return Ok(GenerarRespuesta(true, false));
                }
            }

            return Ok(GenerarRespuesta(true, true));
        }
    }
}
