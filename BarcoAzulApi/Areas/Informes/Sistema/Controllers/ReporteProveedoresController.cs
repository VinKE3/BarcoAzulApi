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
    public class ReporteProveedoresController : GlobalController
    {
        private readonly bReporteProveedores _bReporteProveedores;
        private readonly string _origen;

        public ReporteProveedoresController(bReporteProveedores bReporteProveedores)
        {
            _bReporteProveedores = bReporteProveedores;
            _origen = "Reporte de Proveedores";
        }

        [HttpGet]
        [AuthorizeAction(NombresMenus.RptProveedores, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Exportar(FormatoInforme formato)
        {
            var (nombreArchivo, archivo) = await _bReporteProveedores.Exportar(formato);
            AgregarMensajes(_bReporteProveedores.Mensajes);

            if (archivo is not null)
            {
                return File(new MemoryStream(archivo), GetContentType(nombreArchivo), nombreArchivo);
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.RptProveedores, UsuarioPermiso.Consultar)]
        public IActionResult FormularioTablas()
        {
            var tablas = bReporteProveedores.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
