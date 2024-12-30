using BarcoAzul.Api.Logica.Informes.Cobranzas;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarcoAzul.Api.Informes;

namespace BarcoAzulApi.Areas.Cobranzas.Controllers
{
    [ApiController]
    [Area(NombreAreas.Informes)]
    [SubArea(NombreSubAreasInformes.Cobranzas)]
    [Route("api/[area]/[subarea]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class InformeCobranzaController : GlobalController
    {
        private readonly bInformeCobranza _bInformeCobranza;
        private readonly string _origen;

        public InformeCobranzaController(bInformeCobranza bInformeCobranza)
        {
            _bInformeCobranza = bInformeCobranza;
            _origen = "Informe de Cobranzas";
        }

        [HttpGet]
        [AuthorizeAction(NombresMenus.RptInformeCobranza, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Exportar([FromQuery] oParamInformeCobranza parametros, FormatoInforme formato)
        {
            var (nombreArchivo, archivo) = await _bInformeCobranza.Exportar(parametros, formato);
            AgregarMensajes(_bInformeCobranza.Mensajes);

            if (archivo is not null)
            {
                return File(new MemoryStream(archivo), GetContentType(nombreArchivo), nombreArchivo);
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.RptInformeCobranza, UsuarioPermiso.Consultar)]
        public IActionResult FormularioTablas()
        {
            var tablas = bInformeCobranza.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
