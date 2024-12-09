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
    public class DistritoController : GlobalController
    {
        private readonly bDistrito _bDistrito;
        private readonly string _origen;

        public DistritoController(bDistrito bDistrito)
        {
            _bDistrito = bDistrito;
            _origen = "Distrito";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Distrito, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(DistritoDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _bDistrito.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el código ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                if (await _bDistrito.DatosRepetidos(model.Id, model.Nombre))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nombre ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bDistrito.Registrar(model);
                AgregarMensajes(_bDistrito.Mensajes);

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
        [AuthorizeAction(NombresMenus.Distrito, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(DistritoDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bDistrito.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bDistrito.DatosRepetidos(model.Id, model.Nombre))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nombre ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bDistrito.Modificar(model);
                AgregarMensajes(_bDistrito.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:length(6)}")]
        [AuthorizeAction(NombresMenus.Distrito, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bDistrito.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bDistrito.Eliminar(id);
            AgregarMensajes(_bDistrito.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:length(6)}")]
        [AuthorizeAction(NombresMenus.Distrito, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bDistrito.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var distrito = await _bDistrito.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bDistrito.Mensajes);

            if (distrito is not null)
            {
                return Ok(GenerarRespuesta(true, distrito));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.Distrito, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string nombre, [FromQuery] oPaginacion paginacion)
        {
            var distritos = await _bDistrito.Listar(nombre, paginacion);

            if (distritos is not null)
            {
                return Ok(GenerarRespuesta(true, distritos));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("FormularioTablas")]
        [AuthorizeAction(NombresMenus.Distrito, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bDistrito.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
