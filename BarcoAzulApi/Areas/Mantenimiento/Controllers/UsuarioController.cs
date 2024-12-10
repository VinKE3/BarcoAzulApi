using BarcoAzul.Api.Logica.Empresa;
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
    public class UsuarioController : GlobalController
    {
        private readonly bUsuario _bUsuario;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public UsuarioController(bUsuario bUsuario, oConfiguracionGlobal configuracionGlobal)
        {
            _bUsuario = bUsuario;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Usuario";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Usuario, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(UsuarioRegistrarDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _bUsuario.DatosRepetidos(null, model.Nick))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nick ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bUsuario.Registrar(model);
                AgregarMensajes(_bUsuario.Mensajes);

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
        [AuthorizeAction(NombresMenus.Usuario, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(UsuarioModificarDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == _configuracionGlobal.DefaultUsuarioId)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede modificar el usuario usado por defecto en el sistema."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bUsuario.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bUsuario.DatosRepetidos(model.Id, model.Nick))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nick ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bUsuario.Modificar(model);
                AgregarMensajes(_bUsuario.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:length(3)}")]
        [AuthorizeAction(NombresMenus.Usuario, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (id == _configuracionGlobal.DefaultLineaId)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede eliminar el usuario usado por defecto en el sistema."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            if (!await _bUsuario.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bUsuario.Eliminar(id);
            AgregarMensajes(_bUsuario.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:length(3)}")]
        [AuthorizeAction(NombresMenus.Usuario, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bUsuario.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var usuario = await _bUsuario.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bUsuario.Mensajes);

            if (usuario is not null)
            {
                return Ok(GenerarRespuesta(true, usuario));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.Usuario, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string nick, [FromQuery] oPaginacion paginacion)
        {
            var usuarios = await _bUsuario.Listar(nick, paginacion);

            if (usuarios is not null)
            {
                return Ok(GenerarRespuesta(true, usuarios));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Usuario, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bUsuario.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpPut(nameof(CambiarClave))]
        public async Task<IActionResult> CambiarClave(oUsuarioCambiarClave model)
        {
            if (ModelState.IsValid)
            {
                var modificado = await _bUsuario.CambiarClave(model);
                AgregarMensajes(_bUsuario.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: cambio de clave completado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }
    }
}
