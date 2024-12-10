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
    public class ClientePersonalController : GlobalController
    {
        private readonly bClientePersonal _bClientePersonal;
        private readonly string _origen;

        public ClientePersonalController(bClientePersonal bClientePersonal)
        {
            _bClientePersonal = bClientePersonal;
            _origen = "Cliente - Personal";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Registrar(ClientePersonalDTO model)
        {
            if (ModelState.IsValid)
            {
                bool registrado = await _bClientePersonal.Registrar(model);
                AgregarMensajes(_bClientePersonal.Mensajes);

                if (registrado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: registrado exitosamente."));
                    return CreatedAtAction(nameof(GetPorId), new { id = model.Id }, GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id}")]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bClientePersonal.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bClientePersonal.Eliminar(id);
            AgregarMensajes(_bClientePersonal.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id)
        {
            if (!await _bClientePersonal.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var clientePersonal = await _bClientePersonal.GetPorId(id);
            AgregarMensajes(_bClientePersonal.Mensajes);

            if (clientePersonal is not null)
            {
                return Ok(GenerarRespuesta(true, clientePersonal));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(ListarPorCliente))]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> ListarPorCliente(string clienteId)
        {
            if (!await new bCliente(null, _bClientePersonal.ConnectionManager).Existe(clienteId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no existe un cliente con el ID proporcionado."));
                return NotFound(GenerarRespuesta(false));
            }

            var clientePersonal = await _bClientePersonal.ListarPorCliente(clienteId);
            AgregarMensajes(_bClientePersonal.Mensajes);

            if (clientePersonal is not null)
            {
                return Ok(GenerarRespuesta(true, clientePersonal));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bClientePersonal.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
