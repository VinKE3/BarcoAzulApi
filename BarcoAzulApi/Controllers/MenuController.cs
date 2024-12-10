using BarcoAzul.Api.Logica.Empresa;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class MenuController : GlobalController
    {
        private readonly bMenu _bMenu;

        public MenuController(bMenu bMenu)
        {
            _bMenu = bMenu;
        }

        [HttpGet(nameof(Listar))]
        public async Task<IActionResult> Listar()
        {
            var menus = await _bMenu.Listar();
            return Ok(GenerarRespuesta(true, menus));
        }
    }
}
