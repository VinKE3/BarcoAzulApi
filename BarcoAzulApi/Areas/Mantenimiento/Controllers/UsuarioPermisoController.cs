using BarcoAzul.Api.Logica.Empresa;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Mantenimiento.Controllers
{
    [ApiController]
    [Area(NombreAreas.Mantenimiento)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UsuarioPermisoController : GlobalController
    {
        private readonly bUsuarioPermiso _bUsuarioPermiso;
        private readonly string _origen;

        public UsuarioPermisoController(bUsuarioPermiso bUsuarioPermiso)
        {
            _bUsuarioPermiso = bUsuarioPermiso;
            _origen = "Usuario - Permisos";
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.Usuario, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Registrar(oUsuarioConfiguracionPermisos usuarioConfiguracionPermisos)
        {
            if (ModelState.IsValid)
            {
                bool registrado = await _bUsuarioPermiso.Registrar(usuarioConfiguracionPermisos);
                AgregarMensajes(_bUsuarioPermiso.Mensajes);

                if (registrado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: registrados exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        public async Task<IActionResult> Listar(string usuarioId)
        {
            var usuarioPermisos = await _bUsuarioPermiso.GetPermisos(usuarioId);
            AgregarMensajes(_bUsuarioPermiso.Mensajes);

            if (usuarioPermisos is null)
                return BadRequest(GenerarRespuesta(false));

            return Ok(GenerarRespuesta(true, usuarioPermisos));
        }

        [HttpGet(nameof(GetPorUsuarioYMenu))]
        public async Task<IActionResult> GetPorUsuarioYMenu(string usuarioId, string menuId)
        {
            var usuarioPermiso = await _bUsuarioPermiso.GetPorUsuarioYMenu(usuarioId, menuId);
            AgregarMensajes(_bUsuarioPermiso.Mensajes);

            if (usuarioPermiso is null)
                return BadRequest(GenerarRespuesta(false));

            return Ok(GenerarRespuesta(true, usuarioPermiso));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.SubLinea, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public IActionResult FormularioTablas()
        {
            var tablas = bUsuarioPermiso.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }
    }
}
