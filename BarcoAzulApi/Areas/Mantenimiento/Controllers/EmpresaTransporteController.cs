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
    public class EmpresaTransporteController : GlobalController
    {
        private readonly bEmpresaTransporte _bEmpresaTransporte;
        private readonly string _origen;

        public EmpresaTransporteController(bEmpresaTransporte bEmpresaTransporte)
        {
            _bEmpresaTransporte = bEmpresaTransporte;
            _origen = "Empresa de Transporte";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.EmpresaTransporte, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(EmpresaTransporteDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _bEmpresaTransporte.DatosRepetidos(null, model.NumeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de documento de identidad ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bEmpresaTransporte.Registrar(model);
                AgregarMensajes(_bEmpresaTransporte.Mensajes);

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
        [AuthorizeAction(NombresMenus.EmpresaTransporte, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(EmpresaTransporteDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bEmpresaTransporte.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bEmpresaTransporte.DatosRepetidos(model.Id, model.NumeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de documento de identidad ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bEmpresaTransporte.Modificar(model);
                AgregarMensajes(_bEmpresaTransporte.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificada exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id}")]
        [AuthorizeAction(NombresMenus.EmpresaTransporte, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bEmpresaTransporte.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bEmpresaTransporte.Eliminar(id);
            AgregarMensajes(_bEmpresaTransporte.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.EmpresaTransporte, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bEmpresaTransporte.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var empresaTransporte = await _bEmpresaTransporte.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bEmpresaTransporte.Mensajes);

            if (empresaTransporte is not null)
            {
                return Ok(GenerarRespuesta(true, empresaTransporte));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.EmpresaTransporte, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string nombre, [FromQuery] oPaginacion paginacion)
        {
            var empresasTransporte = await _bEmpresaTransporte.Listar(nombre, paginacion);

            if (empresasTransporte is not null)
            {
                return Ok(GenerarRespuesta(true, empresasTransporte));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
