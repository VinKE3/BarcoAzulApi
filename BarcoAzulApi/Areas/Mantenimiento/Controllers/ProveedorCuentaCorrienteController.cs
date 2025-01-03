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
    public class ProveedorCuentaCorrienteController : GlobalController
    {
        private readonly bProveedorCuentaCorriente _bProveedorCuentaCorriente;
        private readonly string _origen;

        public ProveedorCuentaCorrienteController(bProveedorCuentaCorriente bProveedorCuentaCorriente)
        {
            _bProveedorCuentaCorriente = bProveedorCuentaCorriente;
            _origen = "Proveedor - Cuenta Corriente";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Registrar(oProveedorCuentaCorriente model)
        {
            if (ModelState.IsValid)
            {
                bool registrado = await _bProveedorCuentaCorriente.Registrar(model);
                AgregarMensajes(_bProveedorCuentaCorriente.Mensajes);

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
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oProveedorCuentaCorriente model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bProveedorCuentaCorriente.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                bool modificado = await _bProveedorCuentaCorriente.Modificar(model);
                AgregarMensajes(_bProveedorCuentaCorriente.Mensajes);

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
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bProveedorCuentaCorriente.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bProveedorCuentaCorriente.Eliminar(id);
            AgregarMensajes(_bProveedorCuentaCorriente.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id)
        {
            if (!await _bProveedorCuentaCorriente.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var proveedorCuentaCorriente = await _bProveedorCuentaCorriente.GetPorId(id);
            AgregarMensajes(_bProveedorCuentaCorriente.Mensajes);

            if (proveedorCuentaCorriente is not null)
            {
                return Ok(GenerarRespuesta(true, proveedorCuentaCorriente));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(ListarPorProveedor))]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> ListarPorProveedor(string proveedorId)
        {
            if (!await new bProveedor(null, _bProveedorCuentaCorriente.ConnectionManager).Existe(proveedorId))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no existe un proveedor con el ID proporcionado."));
                return NotFound(GenerarRespuesta(false));
            }

            var proveedorCuentasCorrientes = await _bProveedorCuentaCorriente.ListarPorProveedor(proveedorId);
            AgregarMensajes(_bProveedorCuentaCorriente.Mensajes);

            if (proveedorCuentasCorrientes is not null)
            {
                return Ok(GenerarRespuesta(true, proveedorCuentasCorrientes));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bProveedorCuentaCorriente.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
