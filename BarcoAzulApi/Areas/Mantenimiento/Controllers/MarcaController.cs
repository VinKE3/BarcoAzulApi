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
    public class MarcaController : GlobalController
    {
        private readonly bMarca _bMarca;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public MarcaController(bMarca bMarca, oConfiguracionGlobal configuracionGlobal)
        {
            _bMarca = bMarca;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Marca";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Marca, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(oMarca model)
        {
            if (ModelState.IsValid)
            {
                if (await _bMarca.DatosRepetidos(null, model.Nombre))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nombre ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bMarca.Registrar(model);
                AgregarMensajes(_bMarca.Mensajes);

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
        [AuthorizeAction(NombresMenus.Marca, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oMarca model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == _configuracionGlobal.DefaultMarcaId)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede modificar la marca usada por defecto en el sistema."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bMarca.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bMarca.DatosRepetidos(model.Id, model.Nombre))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el nombre ingresado."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bMarca.Modificar(model);
                AgregarMensajes(_bMarca.Mensajes);

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
        [AuthorizeAction(NombresMenus.Marca, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id == _configuracionGlobal.DefaultMarcaId)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede eliminar la marca usada por defecto en el sistema."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            if (!await _bMarca.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bMarca.Eliminar(id);
            AgregarMensajes(_bMarca.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminada exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:int}")]
        [AuthorizeAction(NombresMenus.Marca, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(int id)
        {
            if (!await _bMarca.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var marca = await _bMarca.GetPorId(id);
            AgregarMensajes(_bMarca.Mensajes);

            if (marca is not null)
            {
                return Ok(GenerarRespuesta(true, marca));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.Marca, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string nombre, [FromQuery] oPaginacion paginacion)
        {
            var marcas = await _bMarca.Listar(nombre, paginacion);

            if (marcas is not null)
            {
                return Ok(GenerarRespuesta(true, marcas));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
