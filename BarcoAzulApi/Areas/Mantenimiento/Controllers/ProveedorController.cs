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
    public class ProveedorController : GlobalController
    {
        private readonly bProveedor _bProveedor;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public ProveedorController(bProveedor bProveedor, oConfiguracionGlobal configuracionGlobal)
        {
            _bProveedor = bProveedor;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Proveedor";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(ProveedorDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _bProveedor.DatosRepetidos(null, model.NumeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de documento de identidad ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bProveedor.Registrar(model);
                AgregarMensajes(_bProveedor.Mensajes);

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
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(ProveedorDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == _configuracionGlobal.DefaultProveedorId)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede modificar el proveedor usado por defecto en el sistema."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bProveedor.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bProveedor.DatosRepetidos(model.Id, model.NumeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de documento de identidad ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bProveedor.Modificar(model);
                AgregarMensajes(_bProveedor.Mensajes);

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
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (id == _configuracionGlobal.DefaultProveedorId)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede eliminar el proveedor usado por defecto en el sistema."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            if (!await _bProveedor.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bProveedor.Eliminar(id);
            AgregarMensajes(_bProveedor.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:length(6)}")]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bProveedor.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var proveedor = await _bProveedor.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bProveedor.Mensajes);

            if (proveedor is not null)
            {
                return Ok(GenerarRespuesta(true, proveedor));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string numeroDocumentoIdentidad, string nombre, [FromQuery] oPaginacion paginacion)
        {
            var proveedores = await _bProveedor.Listar(numeroDocumentoIdentidad, nombre, paginacion);

            if (proveedores is not null)
            {
                return Ok(GenerarRespuesta(true, proveedores));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Proveedor, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bProveedor.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
