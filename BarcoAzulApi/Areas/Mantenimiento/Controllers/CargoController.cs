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
    public class CargoController : GlobalController
    {
        private readonly bCargo _bCargo;
        private readonly string _origen;

        public CargoController(bCargo bCargo)
        {
            _bCargo = bCargo;
            _origen = "Cargo";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Cargo, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(oCargo model)
        {
            if (ModelState.IsValid)
            {
                if (await _bCargo.DatosRepetidos(null, model.Descripcion))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con la descripción ingresada."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bCargo.Registrar(model);
                AgregarMensajes(_bCargo.Mensajes);

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
        [AuthorizeAction(NombresMenus.Cargo, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oCargo model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bCargo.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bCargo.DatosRepetidos(model.Id, model.Descripcion))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con la descripción ingresada."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bCargo.Modificar(model);
                AgregarMensajes(_bCargo.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:int}")]
        [AuthorizeAction(NombresMenus.Cargo, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!await _bCargo.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bCargo.Eliminar(id);
            AgregarMensajes(_bCargo.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:int}")]
        [AuthorizeAction(NombresMenus.Cargo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(int id)
        {
            if (!await _bCargo.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var cargo = await _bCargo.GetPorId(id);
            AgregarMensajes(_bCargo.Mensajes);

            if (cargo is not null)
            {
                return Ok(GenerarRespuesta(true, cargo));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.Cargo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string descripcion, [FromQuery] oPaginacion paginacion)
        {
            var cargos = await _bCargo.Listar(descripcion, paginacion);

            if (cargos is not null)
            {
                return Ok(GenerarRespuesta(true, cargos));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
