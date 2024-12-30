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
    public class ReporteClientesController : GlobalController
    {
        private readonly bReporteClientes _bReporteClientes;
        private readonly string _origen;

        public ReporteClientesController(bReporteClientes bReporteClientes)
        {
            _bReporteClientes = bReporteClientes;
            _origen = "Reporte de Clientes";
        }

        [HttpGet]
        [AuthorizeAction(NombresMenus.RptClientes, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Exportar(FormatoInforme formato)
        {
            var (nombreArchivo, archivo) = await _bReporteClientes.Exportar(formato);
            AgregarMensajes(_bReporteClientes.Mensajes);

            if (archivo is not null)
            {
                return File(new MemoryStream(archivo), GetContentType(nombreArchivo), nombreArchivo);
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.RptClientes, UsuarioPermiso.Consultar)]
        public IActionResult FormularioTablas()
        {
            var tablas = bReporteClientes.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
