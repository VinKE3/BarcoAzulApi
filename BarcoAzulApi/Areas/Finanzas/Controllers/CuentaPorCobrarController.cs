using BarcoAzul.Api.Logica.Finanzas;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Finanzas.Controllers
{
    [ApiController]
    [Area(NombreAreas.Finanzas)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CuentaPorCobrarController : GlobalController
    {
        private readonly bCuentaPorCobrar _bCuentaPorCobrar;
        private readonly string _origen;

        public CuentaPorCobrarController(bCuentaPorCobrar bCuentaPorCobrar)
        {
            _bCuentaPorCobrar = bCuentaPorCobrar;
            _origen = "Cuenta por Cobrar";
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.CuentaPorCobrar, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id)
        {
            if (!await _bCuentaPorCobrar.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var cuentaPorCobrar = await _bCuentaPorCobrar.GetPorId(id);
            AgregarMensajes(_bCuentaPorCobrar.Mensajes);

            if (cuentaPorCobrar is not null)
            {
                return Ok(GenerarRespuesta(true, cuentaPorCobrar));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.CuentaPorCobrar, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(string tipoDocumentoId, DateTime? fechaInicio, DateTime? fechaFin, string clienteNombre, bool? isCancelado, [FromQuery] oPaginacion paginacion)
        {
            var cuentasPorCobrar = await _bCuentaPorCobrar.Listar(tipoDocumentoId, fechaInicio, fechaFin, clienteNombre, isCancelado, paginacion);
            AgregarMensajes(_bCuentaPorCobrar.Mensajes);

            if (cuentasPorCobrar is not null)
            {
                return Ok(GenerarRespuesta(true, cuentasPorCobrar));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FiltroTablas))]
        [AuthorizeAction(NombresMenus.CuentaPorCobrar, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> FiltroTablas()
        {
            var tablas = await _bCuentaPorCobrar.FiltroTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        //TODO: Revisar como validar que el usuario tenga permisos para usar esta opción
        [HttpGet(nameof(ListarPendientes))]
        public async Task<IActionResult> ListarPendientes(string numeroDocumento, [FromQuery] oPaginacion paginacion, string tipoDocumentoId = "", string clienteId = "")
        {
            var cuentasPorCobrarPendientes = await _bCuentaPorCobrar.ListarPendientes(numeroDocumento, paginacion, tipoDocumentoId, clienteId);
            AgregarMensajes(_bCuentaPorCobrar.Mensajes);

            if (cuentasPorCobrarPendientes is not null)
            {
                return Ok(GenerarRespuesta(true, cuentasPorCobrarPendientes));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
