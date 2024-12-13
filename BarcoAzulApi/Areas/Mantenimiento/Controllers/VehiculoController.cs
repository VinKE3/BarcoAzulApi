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
    public class VehiculoController : GlobalController
    {
        private readonly bVehiculo _bVehiculo;
        private readonly string _origen;

        public VehiculoController(bVehiculo bVehiculo)
        {
            _bVehiculo = bVehiculo;
            _origen = "Vehículo";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Vehiculo, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(VehiculoDTO model)
        {
            if (ModelState.IsValid)
            {
                if (await _bVehiculo.DatosRepetidos(null, model.NumeroPlaca))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de placa ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bVehiculo.Registrar(model);
                AgregarMensajes(_bVehiculo.Mensajes);

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
        [AuthorizeAction(NombresMenus.Vehiculo, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(VehiculoDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bVehiculo.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bVehiculo.DatosRepetidos(model.Id, model.NumeroPlaca))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el número de placa ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bVehiculo.Modificar(model);
                AgregarMensajes(_bVehiculo.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:length(3)}")]
        [AuthorizeAction(NombresMenus.Vehiculo, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bVehiculo.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bVehiculo.Eliminar(id);
            AgregarMensajes(_bVehiculo.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.Vehiculo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bVehiculo.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var vehiculo = await _bVehiculo.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bVehiculo.Mensajes);

            if (vehiculo is not null)
            {
                return Ok(GenerarRespuesta(true, vehiculo));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.Vehiculo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string numeroPlaca, [FromQuery] oPaginacion paginacion)
        {
            var vehiculos = await _bVehiculo.Listar(numeroPlaca, paginacion);

            if (vehiculos is not null)
            {
                return Ok(GenerarRespuesta(true, vehiculos));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Vehiculo, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bVehiculo.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
