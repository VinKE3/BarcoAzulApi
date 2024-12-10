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
    public class ArticuloController : GlobalController
    {
        private readonly bArticulo _bArticulo;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public ArticuloController(bArticulo bArticulo, oConfiguracionGlobal configuracionGlobal)
        {
            _bArticulo = bArticulo;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Artículo";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.Articulo, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(ArticuloDTO model)
        {
            if (ModelState.IsValid)
            {
                var (existe, valorRepetido) = await _bArticulo.DatosRepetidos(null, model.CodigoBarras, model.Descripcion);

                if (existe)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el/la {valorRepetido} ingresado(a)."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bArticulo.Registrar(model);
                AgregarMensajes(_bArticulo.Mensajes);

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
        [AuthorizeAction(NombresMenus.Articulo, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(ArticuloDTO model)
        {
            if (ModelState.IsValid)
            {
                if (model.Id == _configuracionGlobal.DefaultLineaId + _configuracionGlobal.DefaultSubLineaId + _configuracionGlobal.DefaultArticuloId)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede modificar el artículo usado por defecto en el sistema."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                if (!await _bArticulo.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                var (existe, valorRepetido) = await _bArticulo.DatosRepetidos(model.Id, model.CodigoBarras, model.Descripcion);

                if (existe)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con el/la {valorRepetido} ingresado(a)."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool modificado = await _bArticulo.Modificar(model);
                AgregarMensajes(_bArticulo.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:length(8)}")]
        [AuthorizeAction(NombresMenus.Articulo, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (id == _configuracionGlobal.DefaultLineaId + _configuracionGlobal.DefaultSubLineaId + _configuracionGlobal.DefaultArticuloId)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: no se puede eliminar el artículo usado por defecto en el sistema."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            if (!await _bArticulo.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bArticulo.Eliminar(id);
            AgregarMensajes(_bArticulo.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:length(8)}")]
        [AuthorizeAction(NombresMenus.Articulo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bArticulo.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var articulo = await _bArticulo.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bArticulo.Mensajes);

            if (articulo is not null)
            {
                return Ok(GenerarRespuesta(true, articulo));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.Articulo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string codigoBarras, string descripcion, bool? isActivo, [FromQuery] oPaginacion paginacion)
        {
            var articulos = await _bArticulo.Listar(codigoBarras, descripcion, isActivo, paginacion);

            if (articulos is not null)
            {
                return Ok(GenerarRespuesta(true, articulos));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.Articulo, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bArticulo.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
