using BarcoAzul.Api.Logica;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using BarcoAzul.Api.Logica.Venta;
using BarcoAzul.Api.Logica.Otros;
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
    public class DocumentoVentaController : GlobalController
    {
        private readonly bDocumentoVenta _bDocumentoVenta;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public DocumentoVentaController(bDocumentoVenta bDocumentoVenta, oConfiguracionGlobal configuracionGlobal)
        {
            _bDocumentoVenta = bDocumentoVenta;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Documento de Venta";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(DocumentoVentaDTO model)
        {
            model.ConfiguracionGlobal = _configuracionGlobal;
            TryValidateModel(model);

            if (ModelState.IsValid)
            {
                if (model.AfectarStock && model.IngresoEgresoStock == "-" && !await _bDocumentoVenta.StockSuficiente(model))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!_bDocumentoVenta.CostoDetallesValido(model))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!_bDocumentoVenta.IsFechaValida(TipoAccion.Registrar, model.FechaEmision))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bDocumentoVenta.ClienteTieneCreditoDisponible(model))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bDocumentoVenta.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool registrado = await _bDocumentoVenta.Registrar(model);
                AgregarMensajes(_bDocumentoVenta.Mensajes);

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
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(DocumentoVentaDTO model)
        {
            model.ConfiguracionGlobal = _configuracionGlobal;
            TryValidateModel(model);

            if (ModelState.IsValid)
            {
                if (model.AfectarStock && model.IngresoEgresoStock == "-" && !await _bDocumentoVenta.StockSuficiente(model))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!_bDocumentoVenta.CostoDetallesValido(model))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!_bDocumentoVenta.IsFechaValida(TipoAccion.Registrar, model.FechaEmision))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bDocumentoVenta.ClienteTieneCreditoDisponible(model))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bDocumentoVenta.AnioMesHabilitado(model.FechaEmision))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool modificado = await _bDocumentoVenta.Modificar(model);
                AgregarMensajes(_bDocumentoVenta.Mensajes);

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
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bDocumentoVenta.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            if (await _bDocumentoVenta.IsEnviadoSunat(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está en los servidores de SUNAT."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            (bool isBloqueado, string mensaje) = await _bDocumentoVenta.IsBloqueado(id);

            if (isBloqueado)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool eliminado = await _bDocumentoVenta.Eliminar(id);
            AgregarMensajes(_bDocumentoVenta.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente (Documento: {Comun.VentaIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut($"{nameof(Anular)}/{{id}}")]
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Anular)]
        public async Task<IActionResult> Anular(string id)
        {
            if (!await _bDocumentoVenta.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            if (!await _bDocumentoVenta.IsEnviadoSunat(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro aún no ha sido enviado a SUNAT."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            (bool isBloqueado, string mensaje) = await _bDocumentoVenta.IsBloqueado(id);

            if (isBloqueado)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: {mensaje}"));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool anulado = await _bDocumentoVenta.Anular(id);
            AgregarMensajes(_bDocumentoVenta.Mensajes);

            if (anulado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: anulado exitosamente (Documento: {Comun.VentaIdADocumento(id)})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        //:TODO DESCOMENTAR CUANDO SE IMPLEMENTE LA FUNCION DE IMPRIMIR
        //[HttpGet($"{nameof(Imprimir)}/{{id}}")]
        //[AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Consultar)]
        //public async Task<IActionResult> Imprimir(string id)
        //{
        //    if (!await _bDocumentoVenta.Existe(id))
        //    {
        //        AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
        //        return NotFound(GenerarRespuesta(false));
        //    }

        //    var (nombreArchivo, archivo) = await _bDocumentoVenta.Imprimir(id);
        //    AgregarMensajes(_bDocumentoVenta.Mensajes);

        //    if (archivo is not null)
        //    {
        //        MemoryStream stream = new(archivo);

        //        new FileExtensionContentTypeProvider().TryGetContentType(nombreArchivo, out string contentType);
        //        contentType ??= "application/octet-stream";

        //        System.Net.Mime.ContentDisposition cd = new()
        //        {
        //            FileName = nombreArchivo,
        //            Inline = true
        //        };

        //        Response.Headers.Add("Content-Disposition", cd.ToString());

        //        return File(stream, contentType, nombreArchivo);
        //    }

        //    return BadRequest(GenerarRespuesta(false));
        //}

        [HttpPut($"{nameof(Enviar)}")]
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Enviar(oEnviarDocumentoVenta model)
        {
            if (ModelState.IsValid)
            {
                bool procesado = await _bDocumentoVenta.Enviar(model);
                AgregarMensajes(_bDocumentoVenta.Mensajes);

                if (procesado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: procesado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bDocumentoVenta.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var documentoVenta = await _bDocumentoVenta.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bDocumentoVenta.Mensajes);

            if (documentoVenta is not null)
            {
                return Ok(GenerarRespuesta(true, documentoVenta));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(GetPorTipoDocumentoSerieNumero))]
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorTipoDocumentoSerieNumero(string tipoDocumentoId, string serie, int numero, bool incluirReferencias = false)
        {
            var id = $"{_configuracionGlobal.EmpresaId}{tipoDocumentoId}{serie}{numero:0000000000}";

            if (!await _bDocumentoVenta.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var documentoVenta = await _bDocumentoVenta.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bDocumentoVenta.Mensajes);

            if (documentoVenta is not null)
            {
                return Ok(GenerarRespuesta(true, documentoVenta));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, string clienteNombre, bool? isEnviado, [FromQuery] oPaginacion paginacion)
        {
            var documentosVenta = await _bDocumentoVenta.Listar(fechaInicio, fechaFin, clienteNombre, isEnviado, paginacion);
            AgregarMensajes(_bDocumentoVenta.Mensajes);

            if (documentosVenta is not null)
            {
                return Ok(GenerarRespuesta(true, documentosVenta));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        //TODO: Revisar como validar que el usuario tenga permisos para usar esta opción
        [HttpGet(nameof(ListarSimplificado))]
        public async Task<IActionResult> ListarSimplificado(string numeroDocumento, [FromQuery] oPaginacion paginacion)
        {
            var documentosVenta = await _bDocumentoVenta.ListarSimplificado(numeroDocumento, paginacion);
            AgregarMensajes(_bDocumentoVenta.Mensajes);

            if (documentosVenta is not null)
            {
                return Ok(GenerarRespuesta(true, documentosVenta));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(ListarAnticipos))]
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> ListarAnticipos(DateTime? fechaInicio, DateTime? fechaFin, string clienteId, string numeroDocumento, [FromQuery] oPaginacion paginacion)
        {
            if (string.IsNullOrWhiteSpace(clienteId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, "El ID del cliente es requerido."));
                return BadRequest(GenerarRespuesta(false));
            }

            var anticipos = await _bDocumentoVenta.ListarAnticipos(fechaInicio, fechaFin, clienteId, numeroDocumento, paginacion);
            return Ok(GenerarRespuesta(true, anticipos));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bDocumentoVenta.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpGet(nameof(GetDocumentosReferencia))]
        [AuthorizeAction(NombresMenus.DocumentoVenta, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> GetDocumentosReferencia(string clienteId)
        {
            if (string.IsNullOrWhiteSpace(clienteId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, "El ID del cliente es requerido."));
                return BadRequest(GenerarRespuesta(false));
            }

            bDocumentoReferencia bDocumentoReferencia = new(_bDocumentoVenta.ConnectionManager);
            var documentosReferencia = await bDocumentoReferencia.ListarPorCliente(clienteId);

            return Ok(GenerarRespuesta(true, documentosReferencia));
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

                if (!await _bDocumentoVenta.Existe(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return Ok(GenerarRespuesta(true, false));
                }

                var documentoVenta = await _bDocumentoVenta.GetPorId(id);

                if (!_bDocumentoVenta.IsFechaValida(accion, documentoVenta.FechaEmision))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bDocumentoVenta.AnioMesHabilitado(documentoVenta.FechaEmision))
                {
                    AgregarMensajes(_bDocumentoVenta.Mensajes);
                    return Ok(GenerarRespuesta(true, false));
                }

                var isEnviado = await _bDocumentoVenta.IsEnviadoSunat(id);

                if ((accion == TipoAccion.Modificar || accion == TipoAccion.Eliminar) && isEnviado)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está en los servidores de SUNAT."));
                    return Ok(GenerarRespuesta(true, false));
                }
                else if (accion == TipoAccion.Anular && !isEnviado)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro aún no ha sido enviado a SUNAT."));
                    return Ok(GenerarRespuesta(true, false));
                }

                var (isBloqueado, mensaje) = await _bDocumentoVenta.IsBloqueado(id);

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
