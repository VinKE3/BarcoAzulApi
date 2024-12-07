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
    public class UnidadMedidaController : GlobalController
    {
        private readonly bUnidadMedida _bUnidadMedida;
        private readonly string _origen;

        public UnidadMedidaController(bUnidadMedida bUnidadMedida)
        {
            _bUnidadMedida = bUnidadMedida;
            _origen = "Unidad de Medida";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.UnidadMedida, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(oUnidadMedida model)
        {
            if (ModelState.IsValid)
            {
                if (await _bUnidadMedida.DatosRepetidos(null, model.Descripcion))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con la descripción ingresada."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bUnidadMedida.Registrar(model);
                AgregarMensajes(_bUnidadMedida.Mensajes);

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
        [AuthorizeAction(NombresMenus.UnidadMedida, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oUnidadMedida model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bUnidadMedida.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bUnidadMedida.DatosRepetidos(model.Id, model.Descripcion))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con la descripción ingresada."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bUnidadMedida.Modificar(model);
                AgregarMensajes(_bUnidadMedida.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificada exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:maxlength(2)}")]
        [AuthorizeAction(NombresMenus.UnidadMedida, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bUnidadMedida.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bUnidadMedida.Eliminar(id);
            AgregarMensajes(_bUnidadMedida.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:maxlength(2)}")]
        [AuthorizeAction(NombresMenus.UnidadMedida, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id)
        {
            if (!await _bUnidadMedida.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var unidadMedida = await _bUnidadMedida.GetPorId(id);
            AgregarMensajes(_bUnidadMedida.Mensajes);

            if (unidadMedida is not null)
            {
                return Ok(GenerarRespuesta(true, unidadMedida));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.UnidadMedida, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string descripcion, [FromQuery] oPaginacion paginacion)
        {
            var unidadesMedida = await _bUnidadMedida.Listar(descripcion, paginacion);

            if (unidadesMedida is not null)
            {
                return Ok(GenerarRespuesta(true, unidadesMedida));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
