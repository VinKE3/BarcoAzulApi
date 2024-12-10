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
    public class ProveedorContactoController : GlobalController
    {
        private readonly bProveedorContacto _bProveedorContacto;
        private readonly string _origen;

        public ProveedorContactoController(bProveedorContacto bProveedorContacto)
        {
            _bProveedorContacto = bProveedorContacto;
            _origen = "Proveedor - Contacto";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Registrar(oProveedorContacto model)
        {
            if (ModelState.IsValid)
            {
                bool registrado = await _bProveedorContacto.Registrar(model);
                AgregarMensajes(_bProveedorContacto.Mensajes);

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
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oProveedorContacto model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bProveedorContacto.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                bool modificado = await _bProveedorContacto.Modificar(model);
                AgregarMensajes(_bProveedorContacto.Mensajes);

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
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bProveedorContacto.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bProveedorContacto.Eliminar(id);
            AgregarMensajes(_bProveedorContacto.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id)
        {
            if (!await _bProveedorContacto.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var proveedorContacto = await _bProveedorContacto.GetPorId(id);
            AgregarMensajes(_bProveedorContacto.Mensajes);

            if (proveedorContacto is not null)
            {
                return Ok(GenerarRespuesta(true, proveedorContacto));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(ListarPorProveedor))]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> ListarPorProveedor(string proveedorId)
        {
            if (!await new bProveedor(null, _bProveedorContacto.ConnectionManager).Existe(proveedorId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no existe un proveedor con el ID proporcionado."));
                return NotFound(GenerarRespuesta(false));
            }

            var proveedorContactos = await _bProveedorContacto.ListarPorProveedor(proveedorId);
            AgregarMensajes(_bProveedorContacto.Mensajes);

            if (proveedorContactos is not null)
            {
                return Ok(GenerarRespuesta(true, proveedorContactos));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bProveedorContacto.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
