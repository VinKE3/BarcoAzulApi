using BarcoAzul.Api.Informes.Sistema;
using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Informes.Sistema;

namespace BarcoAzul.Api.Logica.Informes.Sistema
{
    public class bReporteClientes : bComun
    {
        public bReporteClientes(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Reporte de Clientes", configuracionGlobal: configuracionGlobal) { }

        public async Task<(string Nombre, byte[] Archivo)> Exportar(FormatoInforme formato)
        {
            try
            {
                dReporteClientes dReporteClientes = new(GetConnectionString());
                var registros = await dReporteClientes.GetRegistros();

                if (registros is null || !registros.Any())
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Advertencia, $"{_origen}: sin registros."));
                    return (string.Empty, null);
                }

                var rInforme = new rReporteClientes(registros, _configuracionGlobal, RptPath.RptInformesPath);
                return ($"ReporteClientes_{DateTime.Now:yyyyMMddHHmmss}{FormatoUtilidades.GetExtension(formato)}", rInforme.Generar(formato));
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

            return new
            {
                formatos
            };
        }
    }
}
