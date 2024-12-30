using BarcoAzul.Api.Logica.Informes.Gerencia;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarcoAzul.Api.Informes;

namespace BarcoAzulApi.Areas.Gerencia.Controllers
{
    [ApiController]
    [Area(NombreAreas.Informes)]
    [SubArea(NombreSubAreasInformes.Gerencia)]
    [Route("api/[area]/[subarea]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompraPorArticuloController : GlobalController
    {
        private readonly bCompraPorArticulo _bCompraPorArticulo;
        private readonly string _origen;

        public CompraPorArticuloController(bCompraPorArticulo bCompraPorArticulo)
        {
            _bCompraPorArticulo = bCompraPorArticulo;
            _origen = "Compras por Artículo";
        }

        [HttpGet]
        [AuthorizeAction(NombresMenus.RptCompraPorArticulo, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Exportar([FromQuery] oParamCompraPorArticulo parametros, FormatoInforme formato)
        {
            var (nombreArchivo, archivo) = await _bCompraPorArticulo.Exportar(parametros, formato);
            AgregarMensajes(_bCompraPorArticulo.Mensajes);

            if (archivo is not null)
            {
                return File(new MemoryStream(archivo), GetContentType(nombreArchivo), nombreArchivo);
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.RptCompraPorArticulo, UsuarioPermiso.Consultar)]
        public IActionResult FormularioTablas()
        {
            var tablas = bCompraPorArticulo.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
