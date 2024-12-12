using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Logica.Empresa;
using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Empresa.Controllers
{
    [ApiController]
    [Area(NombreAreas.Empresa)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ConfiguracionController : GlobalController
    {
        private readonly bEmpresa _bEmpresa;
        private readonly IConnectionManager _connectionManager;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public ConfiguracionController(bEmpresa bEmpresa, IConnectionManager connectionManager, oConfiguracionGlobal configuracionGlobal)
        {
            _bEmpresa = bEmpresa;
            _connectionManager = connectionManager;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Empresa - Configuración";
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.EmpresaConfiguracion, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(oConfiguracionEmpresa model)
        {
            if (ModelState.IsValid)
            {
                bool modificado = await _bEmpresa.Modificar(model);
                AgregarMensajes(_bEmpresa.Mensajes);

                if (modificado)
                {
                    bConfiguracionGlobal bConfiguracionGlobal = new(_connectionManager);
                    var configuracionGlobal = await bConfiguracionGlobal.Get();
                    _configuracionGlobal.ActualizarValores(configuracionGlobal);

                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificada exitosamente."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet]
        [AuthorizeAction(NombresMenus.EmpresaConfiguracion, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar | UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Get()
        {
            var empresaConfiguracion = await _bEmpresa.Get();
            AgregarMensajes(_bEmpresa.Mensajes);

            if (empresaConfiguracion is not null)
            {
                return Ok(GenerarRespuesta(true, empresaConfiguracion));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.EmpresaConfiguracion, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bEmpresa.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpPut(nameof(ActualizarFechaFiltro))]
        public async Task<IActionResult> ActualizarFechaFiltro()
        {
            await _bEmpresa.ActualizarFechaFiltro();
            _configuracionGlobal.FiltroFechaFin = DateTime.Today;

            return Ok(GenerarRespuesta(true));
        }

        [HttpGet(nameof(GetSimplificado))]
        public async Task<IActionResult> GetSimplificado()
        {
            bConfiguracionGlobal bConfiguracionGlobal = new(_connectionManager);
            var configuracionSimplificado = await bConfiguracionGlobal.GetSimplificado(_configuracionGlobal);

            return Ok(GenerarRespuesta(true, configuracionSimplificado));
        }
    }
}
