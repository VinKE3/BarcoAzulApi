using BarcoAzul.Api.Informes.Compras;
using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Informes.Compras;
using BarcoAzul.Api.Repositorio.Otros;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Logica.Informes.Compras
{
    public class bRegistroCompra : bComun
    {
        public bRegistroCompra(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Registro Compras", configuracionGlobal: configuracionGlobal) { }

        public async Task<(string Nombre, byte[] Archivo)> Exportar(oParamRegistroCompra parametros, FormatoInforme formato)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(parametros.MonedaId))
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Advertencia, $"{_origen}: la moneda es requerida."));
                    return (string.Empty, null);
                }

                parametros.FechaInicio ??= _configuracionGlobal.FiltroFechaInicio;
                parametros.FechaFin ??= _configuracionGlobal.FiltroFechaFin;

                dRegistroCompra dRegistroCompras = new(GetConnectionString());
                var registros = await dRegistroCompras.GetRegistros(parametros);

                if (registros is null || !registros.Any())
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Advertencia, $"{_origen}: sin registros."));
                    return (string.Empty, null);
                }

                var rInforme = new rRegistroCompra(registros, _configuracionGlobal, parametros, RptPath.RptInformesPath);
                return ($"RegistroCompras_{DateTime.Now:yyyyMMddHHmmss}{FormatoUtilidades.GetExtension(formato)}", rInforme.Generar(formato));
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
