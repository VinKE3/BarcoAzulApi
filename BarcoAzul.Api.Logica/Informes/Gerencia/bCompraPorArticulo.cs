using BarcoAzul.Api.Informes.Gerencia;
using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Informes.Gerencia;

namespace BarcoAzul.Api.Logica.Informes.Gerencia
{
    public class bCompraPorArticulo : bComun
    {
        public bCompraPorArticulo(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Compras por Articulo", configuracionGlobal: configuracionGlobal) { }

        public async Task<(string Nombre, byte[] Archivo)> Exportar(oParamCompraPorArticulo parametros, FormatoInforme formato)
        {
            try
            {
                parametros.FechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                parametros.FechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dCompraPorArticulo dCompraPorArticulo = new(GetConnectionString());
                var registros = await dCompraPorArticulo.GetRegistros(parametros);

                if (registros is null || !registros.Any())
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Advertencia, $"{_origen}: sin registros."));
                    return (string.Empty, null);
                }

                var rInforme = new rCompraPorArticulo(registros, _configuracionGlobal, parametros, RptPath.RptInformesPath);
                return ($"{GetNombreReporte(parametros.TipoReporte)}_{DateTime.Now:yyyyMMddHHmmss}{FormatoUtilidades.GetExtension(formato)}", rInforme.Generar(formato));
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Exportar);
                return (string.Empty, null);
            }
        }

        public string GetNombreReporte(string tipoReporte)
        {
            return tipoReporte switch
            {
                "CA" => "CompraArticulos",
                "AC" => "ArticulosMasComprados",
                _ => "ArticuloProduccion",
            };
        }

        public static object FormularioTablas()
        {
            var formatos = FormatoUtilidades.ListarTodos();

            return new
            {
                formatos
            };
        }
    }
}
