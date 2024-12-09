using BarcoAzul.Api.Logica.Mantenimiento;
using BarcoAzul.Api.Modelos.DTOs;
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
    public class ProvinciaController : GlobalController
    {
        private readonly bProvincia _bProvincia;
        private readonly string _origen;

        public ProvinciaController(bProvincia bProvincia)
        {
            _bProvincia = bProvincia;
            _origen = "Provincia";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Provincia, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(ProvinciaDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _bProvincia.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el código ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                if (await _bProvincia.DatosRepetidos(model.Id, model.Nombre))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nombre ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bProvincia.Registrar(model);
                AgregarMensajes(_bProvincia.Mensajes);

                if (registrado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: registrada exitosamente."));
                    return CreatedAtAction(nameof(GetPorId), new { id = model.Id }, GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.Provincia, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(ProvinciaDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bProvincia.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bProvincia.DatosRepetidos(model.Id, model.Nombre))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nombre ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bProvincia.Modificar(model);
                AgregarMensajes(_bProvincia.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificada exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:length(4)}")]
        [AuthorizeAction(NombresMenus.Provincia, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bProvincia.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bProvincia.Eliminar(id);
            AgregarMensajes(_bProvincia.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:length(4)}")]
        [AuthorizeAction(NombresMenus.Provincia, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bProvincia.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var provincia = await _bProvincia.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bProvincia.Mensajes);

            if (provincia is not null)
            {
                return Ok(GenerarRespuesta(true, provincia));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.Provincia, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string nombre, [FromQuery] oPaginacion paginacion)
        {
            var provincias = await _bProvincia.Listar(nombre, paginacion);

            if (provincias is not null)
            {
                return Ok(GenerarRespuesta(true, provincias));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("FormularioTablas")]
        [AuthorizeAction(NombresMenus.Provincia, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bProvincia.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
