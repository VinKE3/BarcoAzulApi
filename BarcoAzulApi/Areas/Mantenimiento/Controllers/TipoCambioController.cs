using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using BarcoAzul.Api.Logica.Mantenimiento;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace BarcoAzulApi.Areas.Mantenimiento.Controllers
{
    [ApiController]
    [Area(NombreAreas.Mantenimiento)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TipoCambioController : GlobalController
    {
        private readonly bTipoCambio _bTipoCambio;
        private readonly string _origen;

        public TipoCambioController(bTipoCambio bTipoCambio)
        {
            _bTipoCambio = bTipoCambio;
            _origen = "Tipo de Cambio";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.TipoCambio, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(oTipoCambio model)
        {
            if (ModelState.IsValid)
            {
                if (await _bTipoCambio.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: ya existe un registro con la fecha ingresada."));
                    return Conflict(GenerarRespuesta(false));
                }

                bool registrado = await _bTipoCambio.Registrar(model);
                AgregarMensajes(_bTipoCambio.Mensajes);

                if (registrado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: registrado exitosamente (Fecha: {model.Id:dd/MM/yyyy})."));
                    return CreatedAtAction(nameof(GetPorId), new { id = model.Id.ToString("yyyy-MM-dd") }, GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.TipoCambio, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oTipoCambio model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bTipoCambio.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                bool modificado = await _bTipoCambio.Modificar(model);
                AgregarMensajes(_bTipoCambio.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente (Fecha: {model.Id:dd/MM/yyyy})."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id:datetime}")]
        [AuthorizeAction(NombresMenus.TipoCambio, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(DateTime id)
        {
            if (!await _bTipoCambio.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            bool eliminado = await _bTipoCambio.Eliminar(id);
            AgregarMensajes(_bTipoCambio.Mensajes);

            if (eliminado)
            {
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente (Fecha: {id:dd/MM/yyyy})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id:datetime}")]
        [AuthorizeAction(NombresMenus.TipoCambio, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(DateTime id)
        {
            if (!await _bTipoCambio.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var tipoCambio = await _bTipoCambio.GetPorId(id);
            AgregarMensajes(_bTipoCambio.Mensajes);

            if (tipoCambio is not null)
            {
                return Ok(GenerarRespuesta(true, tipoCambio));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet("Listar")]
        [AuthorizeAction(NombresMenus.TipoCambio, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(int? anio, int? mes, [FromQuery] oPaginacion paginacion)
        {
            var tiposCambio = await _bTipoCambio.Listar(anio, mes, paginacion);

            if (tiposCambio is not null)
            {
                return Ok(GenerarRespuesta(true, tiposCambio));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
