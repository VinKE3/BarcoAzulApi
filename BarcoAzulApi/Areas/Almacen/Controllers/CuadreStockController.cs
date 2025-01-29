using BarcoAzul.Api.Logica;
using BarcoAzul.Api.Logica.Configuracion;
using BarcoAzul.Api.Logica.Almacen;
using BarcoAzul.Api.Modelos.DTOs;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio;
using BarcoAzul.Api.Utilidades;
using BarcoAzulApi.Configuracion;
using BarcoAzulApi.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarcoAzulApi.Areas.Almacen.Controllers
{
    [ApiController]
    [Area(NombreAreas.Almacen)]
    [Route("api/[area]/[controller]")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CuadreStockController : GlobalController
    {
        private readonly bCuadreStock _bCuadreStock;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _origen;

        public CuadreStockController(bCuadreStock bCuadreStock, oConfiguracionGlobal configuracionGlobal)
        {
            _bCuadreStock = bCuadreStock;
            _configuracionGlobal = configuracionGlobal;
            _origen = "Cuadre de Stock";
        }

        [HttpPost]
        [AuthorizeAction(NombresMenus.CuadreStock, UsuarioPermiso.Registrar)]
        public async Task<IActionResult> Registrar(CuadreStockDTO model)
        {
            if (ModelState.IsValid)
            {
                bool registrado = await _bCuadreStock.Registrar(model);
                AgregarMensajes(_bCuadreStock.Mensajes);

                if (registrado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: registrado exitosamente (Número: {model.Numero})."));
                    return CreatedAtAction(nameof(GetPorId), new { id = model.Id }, GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut]
        [AuthorizeAction(NombresMenus.CuadreStock, UsuarioPermiso.Modificar)]
        public async Task<IActionResult> Modificar(CuadreStockDTO model)
        {
            if (ModelState.IsValid)
            {
                if (!await _bCuadreStock.Existe(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return NotFound(GenerarRespuesta(false));
                }

                if (await _bCuadreStock.IsBloqueado(model.Id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
                }

                bool modificado = await _bCuadreStock.Modificar(model);
                AgregarMensajes(_bCuadreStock.Mensajes);

                if (modificado)
                {
                    AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: modificado exitosamente (Número: {model.Numero})."));
                    return Ok(GenerarRespuesta(true));
                }
            }

            AgregarErroresModeloEnMensajes(ModelState);
            return BadRequest(GenerarRespuesta(false));
        }

        [HttpDelete("{id}")]
        [AuthorizeAction(NombresMenus.CuadreStock, UsuarioPermiso.Eliminar)]
        public async Task<IActionResult> Eliminar(string id)
        {
            if (!await _bCuadreStock.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            if (await _bCuadreStock.IsBloqueado(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                return StatusCode(StatusCodes.Status403Forbidden, GenerarRespuesta(false));
            }

            bool eliminado = await _bCuadreStock.Eliminar(id);
            AgregarMensajes(_bCuadreStock.Mensajes);

            if (eliminado)
            {
                var splitId = bCuadreStock.SplitId(id);
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: eliminado exitosamente (Número: {splitId.Numero})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut($"{nameof(AbrirCerrar)}/{{id}}")]
        [AuthorizeAction(NombresMenus.CuadreStock, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> AbrirCerrar(string id, bool estado)
        {
            if (!await _bCuadreStock.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            //validar aqui
            // Obtener la fecha del cuadre
            DateTime? fechaCuadre = await _bCuadreStock.GetFechaCuadre(id);

            // Validar si existe la fecha del cuadre
            if (fechaCuadre == null)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: No se encontró la fecha del cuadre."));
                return BadRequest(GenerarRespuesta(false));
            }

            // Validar si el período está cerrado
            var (isValido, mensajeError) = await _bCuadreStock.VerificarPeriodoCerrado(fechaCuadre.Value);

            if (!isValido)
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: No se puede modificar el cuadre porque el período está cerrado. {mensajeError}"));
                return BadRequest(GenerarRespuesta(false));
            }


            bool abiertoCerrado = await _bCuadreStock.AbrirCerrar(id, estado);
            AgregarMensajes(_bCuadreStock.Mensajes);

            if (abiertoCerrado)
            {
                _configuracionGlobal.FechaUltimoCuadre = await _bCuadreStock.GetFechaUltimoCuadre();

                var splitId = bCuadreStock.SplitId(id);
                AgregarMensajeAlInicio(new oMensaje(MensajeTipo.Exito, $"{_origen}: {(estado ? "cerrado" : "abierto")} exitosamente (Número: {splitId.Numero})."));
                return Ok(GenerarRespuesta(true));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet("{id}")]
        [AuthorizeAction(NombresMenus.CuadreStock, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> GetPorId(string id, bool incluirReferencias = false)
        {
            if (!await _bCuadreStock.Existe(id))
            {
                AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                return NotFound(GenerarRespuesta(false));
            }

            var cuadreStock = await _bCuadreStock.GetPorId(id, incluirReferencias);
            AgregarMensajes(_bCuadreStock.Mensajes);

            if (cuadreStock is not null)
            {
                return Ok(GenerarRespuesta(true, cuadreStock));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(Listar))]
        [AuthorizeAction(NombresMenus.CuadreStock, UsuarioPermiso.Consultar)]
        public async Task<IActionResult> Listar(DateTime? fechaInicio, DateTime? fechaFin, [FromQuery] oPaginacion paginacion)
        {
            var cuadresStock = await _bCuadreStock.Listar(fechaInicio, fechaFin, paginacion);
            AgregarMensajes(_bCuadreStock.Mensajes);

            if (cuadresStock is not null)
            {
                return Ok(GenerarRespuesta(true, cuadresStock));
            }

            return StatusCode(StatusCodes.Status500InternalServerError, GenerarRespuesta(false));
        }

        [HttpGet(nameof(GetDetalles))]
        [AuthorizeAction(NombresMenus.CuadreStock, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> GetDetalles(string id)
        {
            var detalles = await _bCuadreStock.GetDetalles(id);
            AgregarMensajes(_bCuadreStock.Mensajes);

            if (detalles is not null)
            {
                return Ok(GenerarRespuesta(true, detalles));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpPut(nameof(RecalcularStock))]
        [AuthorizeAction(NombresMenus.CuadreStock, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> RecalcularStock(oRecalcularStock model)
        {
            var recalculado = await _bCuadreStock.RecalcularStock(model);
            AgregarMensajes(_bCuadreStock.Mensajes);

            if (recalculado)
            {
                return Ok(GenerarRespuesta(true, model));
            }

            return BadRequest(GenerarRespuesta(false));
        }

        [HttpGet(nameof(FormularioTablas))]
        [AuthorizeAction(NombresMenus.CuadreStock, UsuarioPermiso.Registrar | UsuarioPermiso.Modificar)]
        public async Task<IActionResult> FormularioTablas()
        {
            var tablas = await _bCuadreStock.FormularioTablas();
            return Ok(GenerarRespuesta(true, tablas));
        }

        [HttpGet(nameof(IsPermitido))]
        public async Task<IActionResult> IsPermitido(TipoAccion accion, string id = "")
        {
            if (accion == TipoAccion.Modificar || accion == TipoAccion.Eliminar)
            {
                if (!Comun.IsVentaIdValido(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el ID no es válido."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (!await _bCuadreStock.Existe(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro buscado no existe."));
                    return Ok(GenerarRespuesta(true, false));
                }

                if (await _bCuadreStock.IsBloqueado(id))
                {
                    AgregarMensaje(new oMensaje(MensajeTipo.Error, $"{_origen}: el registro está bloqueado."));
                    return Ok(GenerarRespuesta(true, false));
                }
            }

            return Ok(GenerarRespuesta(true, true));
        }
    }
}
