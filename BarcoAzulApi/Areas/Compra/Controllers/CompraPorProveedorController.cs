using BarcoAzul.Api.Logica.Informes.Compras;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BarcoAzul.Api.Informes;

namespace BarcoAzulApi.Areas.Compra.Controllers
{
    [ApiController]
    [Area(NombreAreas.Informes)]
    [SubArea(NombreSubAreasInformes.Compras)]
    [Route("api/[area]/[subarea]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CompraPorProveedorController : GlobalController
    {
        private readonly bCompraPorProveedor _bCompraPorProveedor;
        private readonly string _origen;

        public CompraPorProveedorController(bCompraPorProveedor bCompraPorProveedor)
        {
            _bCompraPorProveedor = bCompraPorProveedor;
            _origen = "Compras por Proveedor";
        }

        [HttpGet]
        [AuthorizeAction(NombresMenus.RptCompraPorProveedor, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Exportar([FromQuery] oParamCompraPorProveedor parametros, FormatoInforme formato)
        {
            var (nombreArchivo, archivo) = await _bCompraPorProveedor.Exportar(parametros, formato);
            AgregarMensajes(_bCompraPorProveedor.Mensajes);

            if (archivo is not null)
            {
                return File(new MemoryStream(archivo), GetContentType(nombreArchivo), nombreArchivo);
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.RptCompraPorProveedor, UsuarioPermiso.Consultar)]
        public IActionResult FormularioTablas()
        {
            var tablas = bCompraPorProveedor.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
