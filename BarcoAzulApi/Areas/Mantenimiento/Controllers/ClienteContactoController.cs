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
    public class ClienteContactoController : GlobalController
    {
        private readonly bClienteContacto _bClienteContacto;
        private readonly string _origen;

        public ClienteContactoController(bClienteContacto bClienteContacto)
        {
            _bClienteContacto = bClienteContacto;
            _origen = "Cliente - Contacto";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Registrar(oClienteContacto model)
        {
            if (ModelState.IsValid)
            {
                bool registrado = await _bClienteContacto.Registrar(model);
                AgregarMensajes(_bClienteContacto.Mensajes);

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
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oClienteContacto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bClienteContacto.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                bool modificado = await _bClienteContacto.Modificar(model);
                AgregarMensajes(_bClienteContacto.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id}")]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bClienteContacto.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bClienteContacto.Eliminar(id);
            AgregarMensajes(_bClienteContacto.Mensajes);

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
            if (!await _bClienteContacto.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var clienteContacto = await _bClienteContacto.GetPorId(id);
            AgregarMensajes(_bClienteContacto.Mensajes);

            if (clienteContacto is not null)
            {
                return Ok(GenerarRespuesta(true, clienteContacto));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(ListarPorCliente))]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> ListarPorCliente(string clienteId)
        {
            if (!await new bCliente(null, _bClienteContacto.ConnectionManager).Existe(clienteId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no existe un cliente con el ID proporcionado."));
                return NotFound(GenerarRespuesta(false));
            }

            var clienteContactos = await _bClienteContacto.ListarPorCliente(clienteId);
            AgregarMensajes(_bClienteContacto.Mensajes);

            if (clienteContactos is not null)
            {
                return Ok(GenerarRespuesta(true, clienteContactos));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bClienteContacto.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
