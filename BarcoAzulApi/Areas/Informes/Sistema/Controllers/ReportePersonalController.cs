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
    public class ReportePersonalController : GlobalController
    {
        private readonly bReportePersonal _bReportePersonal;
        private readonly string _origen;

        public ReportePersonalController(bReportePersonal bReportePersonal)
        {
            _bReportePersonal = bReportePersonal;
            _origen = "Reporte de Personal";
        }

        [HttpGet]
        [AuthorizeAction(NombresMenus.RptPersonal, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Exportar(FormatoInforme formato)
        {
            var (nombreArchivo, archivo) = await _bReportePersonal.Exportar(formato);
            AgregarMensajes(_bReportePersonal.Mensajes);

            if (archivo is not null)
            {
                return File(new MemoryStream(archivo), GetContentType(nombreArchivo), nombreArchivo);
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.RptPersonal, UsuarioPermiso.Consultar)]
        public IActionResult FormularioTablas()
        {
            var tablas = bReportePersonal.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
