﻿using BarcoAzul.Api.Informes;
using BarcoAzul.Api.Informes.Sistema;
using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Informes.Sistema;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Logica.Informes.Sistema
{
    public class bReporteProveedores : bComun
    {
        public bReporteProveedores(oConfiguracionGlobal configuracionGlobal, IConnectionManager connectionManager)
            : base(connectionManager, origen: "Reporte de Proveedores", configuracionGlobal: configuracionGlobal) { }

        public async Task<(string Nombre, byte[] Archivo)> Exportar(FormatoInforme formato)
        {
            try
            {
                dReporteProveedores dReporteProveedores = new(GetConnectionString());
                var registros = await dReporteProveedores.GetRegistros();

                if (registros is null || !registros.Any())
                {
                    Mensajes.Add(new oMensaje(MensajeTipo.Advertencia, $"{_origen}: sin registros."));
                    return (string.Empty, null);
                }

                var rInforme = new rReporteProveedores(registros, _configuracionGlobal, RptPath.RptInformesPath);
                return ($"ReporteProveedores_{DateTime.Now:yyyyMMddHHmmss}{FormatoUtilidades.GetExtension(formato)}", rInforme.Generar(formato));
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
