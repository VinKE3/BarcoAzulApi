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
    public class DepartamentoController : GlobalController
    {
        private readonly bDepartamento _bDepartamento;
        private readonly string _origen;

        public DepartamentoController(bDepartamento bDepartamento)
        {
            _bDepartamento = bDepartamento;
            _origen = "Departamento";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Departamento, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(oDepartamento model)
        {
            if (ModelState.IsValid)
            {
                if (await _bDepartamento.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el código ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                if (await _bDepartamento.DatosRepetidos(model.Id, model.Nombre))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nombre ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bDepartamento.Registrar(model);
                AgregarMensajes(_bDepartamento.Mensajes);

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
        [AuthorizeAction(NombresMenus.Departamento, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oDepartamento model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bDepartamento.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bDepartamento.DatosRepetidos(model.Id, model.Nombre))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nombre ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bDepartamento.Modificar(model);
                AgregarMensajes(_bDepartamento.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:length(2)}")]
        [AuthorizeAction(NombresMenus.Departamento, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bDepartamento.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bDepartamento.Eliminar(id);
            AgregarMensajes(_bDepartamento.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:length(2)}")]
        [AuthorizeAction(NombresMenus.Departamento, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id)
        {
            if (!await _bDepartamento.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var departamento = await _bDepartamento.GetPorId(id);
            AgregarMensajes(_bDepartamento.Mensajes);

            if (departamento is not null)
            {
                return Ok(GenerarRespuesta(true, departamento));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.Departamento, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string nombre, [FromQuery] oPaginacion paginacion)
        {
            var departamentos = await _bDepartamento.Listar(nombre, paginacion);

            if (departamentos is not null)
            {
                return Ok(GenerarRespuesta(true, departamentos));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
