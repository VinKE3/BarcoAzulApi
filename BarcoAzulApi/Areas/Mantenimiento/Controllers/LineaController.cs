
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using BarcoAzul.Api.Logica.Mantenimiento;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Mantenimiento.Controllers
{
    [ApiController]
    [Area(NombreAreas.Mantenimiento)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LineaController : GlobalController
    {
        private readonly bLinea _bLinea;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public LineaController(bLinea bLinea, oConfiguracionGlobal configuracionGlobal)
        {
            _bLinea = bLinea;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Línea";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Linea, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(oLinea model)
        {
            if (ModelState.IsValid)
            {
                if (await _bLinea.DatosRepetidos(null, model.Descripcion))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con la descripción ingresada."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bLinea.Registrar(model);
                AgregarMensajes(_bLinea.Mensajes);

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
        [AuthorizeAction(NombresMenus.Linea, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oLinea model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == _configuracionGlobal.DefaultLineaId)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede modificar la línea usada por defecto en el sistema."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bLinea.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bLinea.DatosRepetidos(model.Id, model.Descripcion))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con la descripción ingresada."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bLinea.Modificar(model);
                AgregarMensajes(_bLinea.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificada exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:length(2)}")]
        [AuthorizeAction(NombresMenus.Linea, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (id == _configuracionGlobal.DefaultLineaId)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede eliminar la línea usada por defecto en el sistema."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            if (!await _bLinea.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bLinea.Eliminar(id);
            AgregarMensajes(_bLinea.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:length(2)}")]
        [AuthorizeAction(NombresMenus.Linea, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id)
        {
            if (!await _bLinea.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var linea = await _bLinea.GetPorId(id);
            AgregarMensajes(_bLinea.Mensajes);

            if (linea is not null)
            {
                return Ok(GenerarRespuesta(true, linea));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.Linea, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string descripcion, [FromQuery] oPaginacion paginacion)
        {
            var lineas = await _bLinea.Listar(descripcion, paginacion);

            if (lineas is not null)
            {
                return Ok(GenerarRespuesta(true, lineas));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
