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
    public class CuentaCorrienteController : GlobalController
    {
        private readonly bCuentaCorriente _bCuentaCorriente;
        private readonly string _origen;

        public CuentaCorrienteController(bCuentaCorriente bCuentaCorriente)
        {
            _bCuentaCorriente = bCuentaCorriente;
            _origen = "Cuenta Corriente";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.CuentaCorriente, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(CuentaCorrienteDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _bCuentaCorriente.DatosRepetidos(null, model.Numero))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bCuentaCorriente.Registrar(model);
                AgregarMensajes(_bCuentaCorriente.Mensajes);

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
        [AuthorizeAction(NombresMenus.CuentaCorriente, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(CuentaCorrienteDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bCuentaCorriente.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bCuentaCorriente.DatosRepetidos(model.Id, model.Numero))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bCuentaCorriente.Modificar(model);
                AgregarMensajes(_bCuentaCorriente.Mensajes);

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
        [AuthorizeAction(NombresMenus.CuentaCorriente, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bCuentaCorriente.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bCuentaCorriente.Eliminar(id);
            AgregarMensajes(_bCuentaCorriente.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:length(4)}")]
        [AuthorizeAction(NombresMenus.CuentaCorriente, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bCuentaCorriente.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var cuentaCorriente = await _bCuentaCorriente.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bCuentaCorriente.Mensajes);

            if (cuentaCorriente is not null)
            {
                return Ok(GenerarRespuesta(true, cuentaCorriente));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.CuentaCorriente, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string numero, [FromQuery] oPaginacion paginacion)
        {
            var cuentasCorrientes = await _bCuentaCorriente.Listar(numero, paginacion);

            if (cuentasCorrientes is not null)
            {
                return Ok(GenerarRespuesta(true, cuentasCorrientes));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("FormularioTablas")]
        [AuthorizeAction(NombresMenus.CuentaCorriente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bCuentaCorriente.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
