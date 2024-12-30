using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Logica.Informes.Sistema;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Informes.Sistema.Controllers
{
    [ApiController]
    [Area(NombreAreas.Informes)]
    [SubArea(NombreSubAreasInformes.Sistema)]
    [Route("api/[area]/[subarea]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ReportePersonalClienteController : GlobalController
    {
        private readonly bReportePersonalCliente _bReportePersonalCliente;
        private readonly string _origen;

        public ReportePersonalClienteController(bReportePersonalCliente bReportePersonalCliente)
        {
            _bReportePersonalCliente = bReportePersonalCliente;
            _origen = "Reporte de Personal y Cliente";
        }

        [HttpGet]
        [AuthorizeAction(NombresMenus.RptPersonalCliente, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Exportar(FormatoInforme formato)
        {
            var (nombreArchivo, archivo) = await _bReportePersonalCliente.Exportar(formato);
            AgregarMensajes(_bReportePersonalCliente.Mensajes);

            if (archivo is not null)
            {
                return File(new MemoryStream(archivo), GetContentType(nombreArchivo), nombreArchivo);
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.RptPersonalCliente, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bReportePersonalCliente.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
