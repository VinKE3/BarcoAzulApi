using BarcoAzul.Api.Logica.Mantenimiento;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
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
    public class EntidadBancariaController : GlobalController
    {
        private readonly bEntidadBancaria _bEntidadBancaria;
        private readonly string _origen;

        public EntidadBancariaController(bEntidadBancaria bEntidadBancaria)
        {
            _bEntidadBancaria = bEntidadBancaria;
            _origen = "Entidad Bancaria";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.EntidadBancaria, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(vEntidadBancaria model)
        {
            if (ModelState.IsValid)
            {
                if (await _bEntidadBancaria.DatosRepetidos(null, model.Nombre))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nombre ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bEntidadBancaria.Registrar(model);
                AgregarMensajes(_bEntidadBancaria.Mensajes);

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
        [AuthorizeAction(NombresMenus.EntidadBancaria, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(vEntidadBancaria model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bEntidadBancaria.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bEntidadBancaria.DatosRepetidos(model.Id, model.Nombre))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nombre ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bEntidadBancaria.Modificar(model);
                AgregarMensajes(_bEntidadBancaria.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificada exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:int}")]
        [AuthorizeAction(NombresMenus.EntidadBancaria, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!await _bEntidadBancaria.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bEntidadBancaria.Eliminar(id);
            AgregarMensajes(_bEntidadBancaria.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:int}")]
        [AuthorizeAction(NombresMenus.EntidadBancaria, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(int id, bool incluirReferencias = false)
        {
            if (!await _bEntidadBancaria.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var entidadBancaria = await _bEntidadBancaria.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bEntidadBancaria.Mensajes);

            if (entidadBancaria is not null)
            {
                return Ok(GenerarRespuesta(true, entidadBancaria));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.EntidadBancaria, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string nombre, [FromQuery] oPaginacion paginacion)
        {
            var entidadesBancarias = await _bEntidadBancaria.Listar(nombre, paginacion);

            if (entidadesBancarias is not null)
            {
                return Ok(GenerarRespuesta(true, entidadesBancarias));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("FormularioTablas")]
        [AuthorizeAction(NombresMenus.EntidadBancaria, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public IActionResult FormularioTablas()
        {
            var tablas = bEntidadBancaria.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
