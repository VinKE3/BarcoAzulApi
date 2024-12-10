using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using BarcoAzul.Api.Logica.Mantenimiento;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Mantenimiento.Controllers
{
    [ApiController]
    [Area(NombreAreas.Mantenimiento)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PersonalController : GlobalController
    {
        private readonly bPersonal _bPersonal;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public PersonalController(bPersonal bPersonal, oConfiguracionGlobal configuracionGlobal)
        {
            _bPersonal = bPersonal;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Personal";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Personal, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(PersonalDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _bPersonal.DatosRepetidos(null, model.NumeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de documento de identidad ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bPersonal.Registrar(model);
                AgregarMensajes(_bPersonal.Mensajes);

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
        [AuthorizeAction(NombresMenus.Personal, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(PersonalDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == _configuracionGlobal.DefaultPersonalId)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede modificar el personal usado por defecto en el sistema."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bPersonal.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bPersonal.DatosRepetidos(model.Id, model.NumeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de documento de identidad ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bPersonal.Modificar(model);
                AgregarMensajes(_bPersonal.Mensajes);

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
        [AuthorizeAction(NombresMenus.Personal, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (id == _configuracionGlobal.DefaultPersonalId)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede eliminar el personal usado por defecto en el sistema."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            if (!await _bPersonal.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bPersonal.Eliminar(id);
            AgregarMensajes(_bPersonal.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.Personal, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bPersonal.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var personal = await _bPersonal.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bPersonal.Mensajes);

            if (personal is not null)
            {
                return Ok(GenerarRespuesta(true, personal));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.Personal, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string nombreCompleto, [FromQuery] oPaginacion paginacion)
        {
            var personal = await _bPersonal.Listar(nombreCompleto, paginacion);

            if (personal is not null)
            {
                return Ok(GenerarRespuesta(true, personal));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("FormularioTablas")]
        [AuthorizeAction(NombresMenus.Personal, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bPersonal.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
