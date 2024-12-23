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
    public class CuentaPorPagarController : GlobalController
    {
        private readonly bCuentaPorPagar _bCuentaPorPagar;
        private readonly string _origen;

        public CuentaPorPagarController(bCuentaPorPagar bCuentaPorPagar)
        {
            _bCuentaPorPagar = bCuentaPorPagar;
            _origen = "Cuenta por Pagar";
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.CuentaPorPagar, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id)
        {
            if (!await _bCuentaPorPagar.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var cuentaPorPagar = await _bCuentaPorPagar.GetPorId(id);
            AgregarMensajes(_bCuentaPorPagar.Mensajes);

            if (cuentaPorPagar is not null)
            {
                return Ok(GenerarRespuesta(true, cuentaPorPagar));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.CuentaPorPagar, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, string proveedorNombre, bool? isCancelado, [FromQuery] oPaginacion paginacion)
        {
            var cuentasPorPagar = await _bCuentaPorPagar.Listar(fechaInicio, fechaFin, proveedorNombre, isCancelado, paginacion);
            AgregarMensajes(_bCuentaPorPagar.Mensajes);

            if (cuentasPorPagar is not null)
            {
                return Ok(GenerarRespuesta(true, cuentasPorPagar));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        //TODO: Revisar como validar que el usuario tenga permisos para usar esta opción
        [HttpGet(nameof(ListarPendientes))]
        public async Task<IActionResult> ListarPendientes(string numeroDocumento, [FromQuery] oPaginacion paginacion, string tipoDocumentoId = "", string proveedorId = "")
        {
            var cuentasPorPagarPendientes = await _bCuentaPorPagar.ListarPendientes(numeroDocumento, paginacion, tipoDocumentoId, proveedorId);
            AgregarMensajes(_bCuentaPorPagar.Mensajes);

            if (cuentasPorPagarPendientes is not null)
            {
                return Ok(GenerarRespuesta(true, cuentasPorPagarPendientes));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
