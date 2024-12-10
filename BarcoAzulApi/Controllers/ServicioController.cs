using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using BarcoAzulApi.Configuracion;
using BarcoAzul.Api.Logica.Servicio;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ServicioController : GlobalController
    {
        private readonly IConfiguration _configuration;

        public ServicioController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet(nameof(ConsultarRucDni))]
        [AuthorizeAction(NombresMenus.ConsultaRucDni, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> ConsultarRucDni(string tipo, string numeroDocumentoIdentidad)
        {
            var origen = "Consulta RUC - DNI";
            var tiposPermitidos = new string[] { "ruc", "dni" };

            if (!tiposPermitidos.Contains(tipo))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{origen}: solo están permitidos los tipos {string.Join(", ", tiposPermitidos)}."));
                return BadRequest(GenerarRespuesta(false));
            }

            if (tipo == "ruc")
            {
                if (numeroDocumentoIdentidad?.Length != 11)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{origen}: El RUC debe estar compuesto por 11 dígitos."));
                    return BadRequest(GenerarRespuesta(false));
                }
                else if (!Validacion.ValidarRuc(numeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{origen}: RUC no válido."));
                    return BadRequest(GenerarRespuesta(false));
                }
            }
            else
            {
                if (numeroDocumentoIdentidad?.Length != 8)
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{origen}: El DNI debe estar compuesto por 8 dígitos."));
                    return BadRequest(GenerarRespuesta(false));
                }
                else if (!Validacion.IsInteger(numeroDocumentoIdentidad))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{origen}: DNI no válido."));
                    return BadRequest(GenerarRespuesta(false));
                }
            }

            string url = _configuration["Services:RucDni:Url"];
            string token = _configuration["Services:RucDni:Token"];

            bRucDni bRucDni = new(url, token);
            var respuesta = await bRucDni.Consultar(tipo, numeroDocumentoIdentidad);

            AgregarMensajes(bRucDni.Mensajes);

            if (respuesta is not null)
            {
                return Ok(GenerarRespuesta(true, respuesta));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(ConsultarTipoCambio))]
        [AuthorizeAction(NombresMenus.ConsultaTipoCambio, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> ConsultarTipoCambio(DateTime? fecha)
        {
            if (fecha is null)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"Consulta Tipo de Cambio: debe ingresar la fecha que desea consultar."));
                return BadRequest(GenerarRespuesta(false));
            }
            else if (fecha.Value > DateTime.Today)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"Consulta Tipo de Cambio: la fecha de consulta debe ser menor o igual a la fecha actual."));
                return BadRequest(GenerarRespuesta(false));
            }

            string url = _configuration["Services:TipoCambio:Url"];
            string token = _configuration["Services:TipoCambio:Token"];

            bTipoCambio bTipoCambio = new(url, token);
            var respuesta = await bTipoCambio.Consultar(fecha.Value);
            AgregarMensajes(bTipoCambio.Mensajes);

            if (respuesta is not null)
            {
                return Ok(GenerarRespuesta(true, respuesta));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }
    }
}
