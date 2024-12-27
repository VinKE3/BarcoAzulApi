using BarcoAzul.Api.Logica.Mantenimiento;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Mantenimiento.Controllers
{
    [ApiController]
    [Area(NombreAreas.Mantenimiento)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CorrelativoController : GlobalController
    {
        private readonly bCorrelativo _bCorrelativo;
        private readonly string _origen;

        public CorrelativoController(bCorrelativo bCorrelativo)
        {
            _bCorrelativo = bCorrelativo;
            _origen = "Correlativo";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Correlativo, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(oCorrelativo model)
        {
            if (ModelState.IsValid)
            {
                if (await _bCorrelativo.Existe(model.TipoDocumentoId, model.Serie))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el tipo de documento y serie ingresada."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bCorrelativo.Registrar(model);
                AgregarMensajes(_bCorrelativo.Mensajes);

                if (registrado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: registrado exitosamente."));
                    return CreatedAtAction(nameof(GetPorId), new { tipoDocumentoId = model.TipoDocumentoId, serie = model.Serie }, GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.Correlativo, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oCorrelativo model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bCorrelativo.Existe(model.TipoDocumentoId, model.Serie))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                bool modificado = await _bCorrelativo.Modificar(model);
                AgregarMensajes(_bCorrelativo.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{tipoDocumentoId:length(2)}/{serie:length(4)}")]
        [AuthorizeAction(NombresMenus.Correlativo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string tipoDocumentoId, string serie)
        {
            if (!await _bCorrelativo.Existe(tipoDocumentoId, serie))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var correlativo = await _bCorrelativo.GetPorId(tipoDocumentoId, serie);
            AgregarMensajes(_bCorrelativo.Mensajes);

            if (correlativo is not null)
            {
                return Ok(GenerarRespuesta(true, correlativo));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.Correlativo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar([FromQuery] oPaginacion paginacion)
        {
            var correlativos = await _bCorrelativo.Listar(null, paginacion);

            if (correlativos is not null)
            {
                return Ok(GenerarRespuesta(true, correlativos));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Correlativo, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bCorrelativo.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
