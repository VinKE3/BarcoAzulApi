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
    public class SubLineaController : GlobalController
    {
        private readonly bSubLinea _bSubLinea;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public SubLineaController(bSubLinea bSubLinea, oConfiguracionGlobal configuracionGlobal)
        {
            _bSubLinea = bSubLinea;
            _configuracionGlobal = configuracionGlobal;
            _origen = "SubLínea";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.SubLinea, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(oSubLinea model)
        {
            if (ModelState.IsValid)
            {
                if (await _bSubLinea.DatosRepetidos(null, model.Descripcion))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con la descripción ingresada."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bSubLinea.Registrar(model);
                AgregarMensajes(_bSubLinea.Mensajes);

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
        [AuthorizeAction(NombresMenus.SubLinea, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oSubLinea model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == _configuracionGlobal.DefaultLineaId + _configuracionGlobal.DefaultSubLineaId)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede modificar la sublínea usada por defecto en el sistema."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bSubLinea.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bSubLinea.DatosRepetidos(model.Id, model.Descripcion))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con la descripción ingresada."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bSubLinea.Modificar(model);
                AgregarMensajes(_bSubLinea.Mensajes);

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
        [AuthorizeAction(NombresMenus.SubLinea, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (id == _configuracionGlobal.DefaultLineaId + _configuracionGlobal.DefaultSubLineaId)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede eliminar la sublínea usada por defecto en el sistema."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            if (!await _bSubLinea.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bSubLinea.Eliminar(id);
            AgregarMensajes(_bSubLinea.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:length(4)}")]
        [AuthorizeAction(NombresMenus.SubLinea, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id)
        {
            if (!await _bSubLinea.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var subLinea = await _bSubLinea.GetPorId(id);
            AgregarMensajes(_bSubLinea.Mensajes);

            if (subLinea is not null)
            {
                return Ok(GenerarRespuesta(true, subLinea));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.SubLinea, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string descripcion, [FromQuery] oPaginacion paginacion)
        {
            var subLineas = await _bSubLinea.Listar(descripcion, paginacion);

            if (subLineas is not null)
            {
                return Ok(GenerarRespuesta(true, subLineas));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("FormularioTablas")]
        [AuthorizeAction(NombresMenus.SubLinea, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bSubLinea.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
