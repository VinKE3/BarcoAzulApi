using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzul.Api.Logica.Mantenimiento;
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
    public class ClienteController : GlobalController
    {
        private readonly bCliente _bCliente;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public ClienteController(bCliente bCliente, oConfiguracionGlobal configuracionGlobal)
        {
            _bCliente = bCliente;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Cliente";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(ClienteDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _bCliente.DatosRepetidos(null, model.NumeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de documento de identidad ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bCliente.Registrar(model);
                AgregarMensajes(_bCliente.Mensajes);

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
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(ClienteDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == _configuracionGlobal.DefaultClienteId)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede modificar el cliente usado por defecto en el sistema."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bCliente.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bCliente.DatosRepetidos(model.Id, model.NumeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de documento de identidad ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bCliente.Modificar(model);
                AgregarMensajes(_bCliente.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:length(6)}")]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (id == _configuracionGlobal.DefaultClienteId)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede eliminar el cliente usado por defecto en el sistema."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            if (!await _bCliente.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bCliente.Eliminar(id);
            AgregarMensajes(_bCliente.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:length(6)}")]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bCliente.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var cliente = await _bCliente.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bCliente.Mensajes);

            if (cliente is not null)
            {
                return Ok(GenerarRespuesta(true, cliente));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string numeroDocumentoIdentidad, string nombre, [FromQuery] oPaginacion paginacion)
        {
            var clientes = await _bCliente.Listar(numeroDocumentoIdentidad, nombre, paginacion);

            if (clientes is not null)
            {
                return Ok(GenerarRespuesta(true, clientes));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("FormularioTablas")]
        [AuthorizeAction(NombresMenus.Cliente, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bCliente.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
