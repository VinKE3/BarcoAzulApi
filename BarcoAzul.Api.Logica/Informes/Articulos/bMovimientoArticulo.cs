using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using BarcoAzul.Api.Repositorio.Informes.Articulos;
using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Informes.Articulos;

namespace BarcoAzul.Api.Logica.Informes.Articulos
{
    public class bMovimientoArticulo : bComun
    {
        public bMovimientoArticulo(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Movimiento de Artículo", configuracionGlobal: configuracionGlobal) { }

        public async Task<IEnumerable<oMovimientoArticulo>> Listar(DateTime? fechaInicio, DateTime? fechaFin, string estadoStock)
        {
            try
            {
                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;
                estadoStock ??= string.Empty;

                dMovimientoArticulo dMovimientoArticulo = new(GetConnectionString());
                return await dMovimientoArticulo.Listar(fechaInicio.Value, fechaFin.Value, estadoStock);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Listar);
                return null;
            }
        }

        public async Task<oKardexArticulo> GetKardexArticulo(string id, DateTime? fechaInicio, DateTime? fechaFin)
        {
            try
            {
                var splitArticuloId = dArticulo.SplitId(id);

                fechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                fechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dMovimientoArticulo dMovimientoArticulo = new(GetConnectionString());
                return await dMovimientoArticulo.GetKardexArticulo(splitArticuloId.LineaId, splitArticuloId.SubLineaId, splitArticuloId.ArticuloId, fechaInicio.Value, fechaFin.Value);
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }

        public async Task<(string Nombre, byte[] Archivo)> Exportar(oParamMovimientoArticulo parametros, FormatoInforme formato)
        {
            try
            {
                parametros.FechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                parametros.FechaFin ??= _configuracionGlobal.FiltroFechaFin;
                //parametros.TipoExistenciaId = string.IsNullOrWhiteSpace(parametros.TipoExistenciaId) ? "%" : parametros.TipoExistenciaId;

                dMovimientoArticulo dMovimientoArticulo = new(GetConnectionString());
                var registros = await dMovimientoArticulo.GetRegistros(parametros);

                if (registros is null || !registros.Any())
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Advertencia, $"{_origen}: sin registros."));
                    return (string.Empty, null);
                }

                //parametros.TipoExistenciaDescripcion = parametros.TipoExistenciaId == "%" ? "TODOS" : (await new dTipoExistencia(GetConnectionString()).GetPorId(parametros.TipoExistenciaId))?.Descripcion;

                var rInforme = new rMovimientoArticulo(registros, _configuracionGlobal, parametros, RptPath.RptInformesPath);
                return ($"MovimientoArticulo_{DateTime.Now:yyyyMMddHHmmss}{FormatoUtilidades.GetExtension(formato)}", rInforme.Generar(formato));
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Exportar);
                return (string.Empty, null);
            }
        }

        public async Task<object> FormularioTablas()
        {
            var formatos = FormatoUtilidades.ListarTodos();
            var estadosStock = new[]
            {
                new { Id = "", Descripcion = "TODOS" },
                new { Id = "AL01", Descripcion = "STOCK AGOTANDOSE" },
                new { Id = "AL02", Descripcion = "STOCK SUFICIENTE" },
                new { Id = "AL03", Descripcion = "STOCK EXCESIVO" },
                new { Id = "AL04", Descripcion = "SIN STOCK" },
            };


            return new
            {
                formatos,
                estadosStock
            };
        }
    }
}
