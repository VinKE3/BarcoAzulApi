using BarcoAzul.Api.Logica.Informes.Ventas;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarcoAzul.Api.Informes;

namespace BarcoAzulApi.Areas.Informes.Ventas.Controllers
{
    [ApiController]
    [Area(NombreAreas.Informes)]
    [SubArea(NombreSubAreasInformes.Ventas)]
    [Route("api/[area]/[subarea]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class RegistroVentaController : GlobalController
    {
        private readonly bRegistroVenta _bRegistroVenta;
        private readonly string _origen;

        public RegistroVentaController(bRegistroVenta bRegistroVenta)
        {
            _bRegistroVenta = bRegistroVenta;
            _origen = "Registro de Venta";
        }

        [HttpGet]
        [AuthorizeAction(NombresMenus.RptRegistroVenta, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Exportar([FromQuery] oParamRegistroVenta parametros, FormatoInforme formato)
        {
            var (nombreArchivo, archivo) = await _bRegistroVenta.Exportar(parametros, formato);
            AgregarMensajes(_bRegistroVenta.Mensajes);

            if (archivo is not null)
            {
                return File(new MemoryStream(archivo), GetContentType(nombreArchivo), nombreArchivo);
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.RptRegistroVenta, UsuarioPermiso.Consultar)]
        public IActionResult FormularioTablas()
        {
            var tablas = bRegistroVenta.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
