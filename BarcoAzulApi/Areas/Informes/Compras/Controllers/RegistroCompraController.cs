using BarcoAzul.Api.Logica.Informes.Compras;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarcoAzul.Api.Informes;

namespace BarcoAzulApi.Areas.Informes.Compras.Controllers
{
    [ApiController]
    [Area(NombreAreas.Informes)]
    [SubArea(NombreSubAreasInformes.Compras)]
    [Route("api/[area]/[subarea]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RegistroCompraController : GlobalController
    {
        private readonly bRegistroCompra _bRegistroCompra;
        private readonly string _origen;

        public RegistroCompraController(bRegistroCompra bRegistroCompra)
        {
            _bRegistroCompra = bRegistroCompra;
            _origen = "Registro Compras";
        }

        [HttpGet]
        [AuthorizeAction(NombresMenus.RptRegistroCompra, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Exportar([FromQuery] oParamRegistroCompra parametros, FormatoInforme formato)
        {
            var (nombreArchivo, archivo) = await _bRegistroCompra.Exportar(parametros, formato);
            AgregarMensajes(_bRegistroCompra.Mensajes);

            if (archivo is not null)
            {
                return File(new MemoryStream(archivo), GetContentType(nombreArchivo), nombreArchivo);
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.RptRegistroCompra, UsuarioPermiso.Consultar)]
        public IActionResult FormularioTablas()
        {
            var tablas = bRegistroCompra.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
