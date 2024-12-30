using BarcoAzul.Api.Informes.Cobranzas;
using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Otros;
using BarcoAzul.Api.Repositorio.Informes.Cobranzas;

namespace BarcoAzul.Api.Logica.Informes.Cobranzas
{
    public class bInformeCobranza : bComun
    {
        public bInformeCobranza(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Informe de Cobranzas", configuracionGlobal: configuracionGlobal) { }

        public async Task<(string Nombre, byte[] Archivo)> Exportar(oParamInformeCobranza parametros, FormatoInforme formato)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parametros.MonedaId))
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Advertencia, $"{_origen}: la moneda es requerida."));
                    return (string.Empty, null);
                }

                parametros.Cancelado = string.IsNullOrWhiteSpace(parametros.Cancelado) ? "%" : parametros.Cancelado;
                parametros.FechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                parametros.FechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dInformeCobranza dInformeCobranza = new(GetConnectionString());
                var registros = await dInformeCobranza.GetRegistros(parametros);

                if (registros is null || !registros.Any())
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Advertencia, $"{_origen}: sin registros."));
                    return (string.Empty, null);
                }

                var rInforme = new rInformeCobranza(registros, _configuracionGlobal, parametros, RptPath.RptInformesPath);
                return ($"InformeCobranza_{DateTime.Now:yyyyMMddHHmmss}{FormatoUtilidades.GetExtension(formato)}", rInforme.Generar(formato));
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Exportar);
                return (string.Empty, null);
            }
        }

        public static object FormularioTablas()
        {
            var formatos = FormatoUtilidades.ListarTodos();
            var monedas = dMoneda.ListarTodos();

            return new
            {
                formatos,
                monedas
            };
        }
    }
}
