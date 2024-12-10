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
    public class ClienteDireccionController : GlobalController
    {
        private readonly bClienteDireccion _bClienteDireccion;
        private readonly string _origen;

        public ClienteDireccionController(bClienteDireccion bClienteDireccion)
        {
            _bClienteDireccion = bClienteDireccion;
            _origen = "Cliente - Dirección";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Registrar(oClienteDireccion model)
        {
            if (ModelState.IsValid)
            {
                bool registrado = await _bClienteDireccion.Registrar(model);
                AgregarMensajes(_bClienteDireccion.Mensajes);

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
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oClienteDireccion model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bClienteDireccion.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                bool modificado = await _bClienteDireccion.Modificar(model);
                AgregarMensajes(_bClienteDireccion.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificada exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:int}")]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (!await _bClienteDireccion.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bClienteDireccion.Eliminar(id);
            AgregarMensajes(_bClienteDireccion.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:int}")]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(int id)
        {
            if (!await _bClienteDireccion.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var clienteDireccion = await _bClienteDireccion.GetPorId(id);
            AgregarMensajes(_bClienteDireccion.Mensajes);

            if (clienteDireccion is not null)
            {
                return Ok(GenerarRespuesta(true, clienteDireccion));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(ListarPorCliente))]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> ListarPorCliente(string clienteId)
        {
            if (!await new bCliente(null, _bClienteDireccion.ConnectionManager).Existe(clienteId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no existe un cliente con el ID proporcionado."));
                return NotFound(GenerarRespuesta(false));
            }

            var clienteDirecciones = await _bClienteDireccion.ListarPorCliente(clienteId);
            AgregarMensajes(_bClienteDireccion.Mensajes);

            if (clienteDirecciones is not null)
            {
                return Ok(GenerarRespuesta(true, clienteDirecciones));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bClienteDireccion.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
