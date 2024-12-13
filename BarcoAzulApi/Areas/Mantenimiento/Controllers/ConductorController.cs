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
    public class ConductorController : GlobalController
    {
        private readonly bConductor _bConductor;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public ConductorController(bConductor bConductor, oConfiguracionGlobal configuracionGlobal)
        {
            _bConductor = bConductor;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Conductor";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Conductor, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(ConductorDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _bConductor.DatosRepetidos(null, model.NumeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de documento de identidad ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bConductor.Registrar(model);
                AgregarMensajes(_bConductor.Mensajes);

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
        [AuthorizeAction(NombresMenus.Conductor, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(ConductorDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == _configuracionGlobal.DefaultConductorId)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede modificar el conductor usado por defecto en el sistema."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bConductor.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bConductor.DatosRepetidos(model.Id, model.NumeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de documento de identidad ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bConductor.Modificar(model);
                AgregarMensajes(_bConductor.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:maxlength(6)}")]
        [AuthorizeAction(NombresMenus.Conductor, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (id == _configuracionGlobal.DefaultConductorId)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede eliminar el conductor usado por defecto en el sistema."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            if (!await _bConductor.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bConductor.Eliminar(id);
            AgregarMensajes(_bConductor.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:maxlength(6)}")]
        [AuthorizeAction(NombresMenus.Conductor, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bConductor.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var conductor = await _bConductor.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bConductor.Mensajes);

            if (conductor is not null)
            {
                return Ok(GenerarRespuesta(true, conductor));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.Conductor, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string nombre, [FromQuery] oPaginacion paginacion)
        {
            var conductores = await _bConductor.Listar(nombre, paginacion);

            if (conductores is not null)
            {
                return Ok(GenerarRespuesta(true, conductores));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Conductor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bConductor.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
