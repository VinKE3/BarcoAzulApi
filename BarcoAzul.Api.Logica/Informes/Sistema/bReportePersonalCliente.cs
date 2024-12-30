using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Informes.Sistema;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Informes.Sistema;
using BarcoAzul.Api.Repositorio.Mantenimiento;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Logica.Informes.Sistema
{
    public class bReportePersonalCliente : bComun
    {
        public bReportePersonalCliente(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Reporte de Personal y Cliente", configuracionGlobal: configuracionGlobal) { }

        public async Task<(string Nombre, byte[] Archivo)> Exportar(FormatoInforme formato)
        {
            try
            {
                dReportePersonalCliente dReportePersonalCliente = new(GetConnectionString());
                var registros = await dReportePersonalCliente.GetRegistros();

                if (registros is null || !registros.Any())
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Advertencia, $"{_origen}: sin registros."));
                    return (string.Empty, null);
                }

                var rInforme = new rReportePersonalCliente(registros, _configuracionGlobal, RptPath.RptInformesPath);
                return ($"ReportePersonalCliente_{DateTime.Now:yyyyMMddHHmmss}{FormatoUtilidades.GetExtension(formato)}", rInforme.Generar(formato));
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
            var personal = await new dPersonal(GetConnectionString()).ListarTodos();

            return new
            {
                formatos,
                personal
            };
        }
    }
}
