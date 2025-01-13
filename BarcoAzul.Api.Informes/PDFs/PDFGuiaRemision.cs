using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Informes.PDFs
{
    public class PDFGuiaRemision
    {
        private readonly oGuiaRemision _guiaRemision;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private string _rptPath;

        public PDFGuiaRemision(oGuiaRemision guiaRemision, oConfiguracionGlobal configuracionGlobal, string rptPath)
        {
            _guiaRemision = guiaRemision;
            _configuracionGlobal = configuracionGlobal;
            _rptPath = rptPath;
            CompletarRptPath();
        }

        private void CompletarRptPath()
        {
            string nombreRpt = $"RptGuiaRemision_{_guiaRemision.Serie}.rdl";

            if (!File.Exists($"{_rptPath}/{nombreRpt}"))
                nombreRpt = "RptGuiaRemision.rdl";

            _rptPath = $"{_rptPath}/{nombreRpt}";
        }

        private ListDictionary GetParametrosRpt()
        {
            ListDictionary ld = PropertyConverter.ConvertClassToDictionary(_guiaRemision);
            ld.Add(nameof(oConfiguracionGlobal.EmpresaNumeroDocumentoIdentidad), _configuracionGlobal.EmpresaNumeroDocumentoIdentidad);
            ld.Add(nameof(oConfiguracionGlobal.EmpresaNombre), _configuracionGlobal.EmpresaNombre);
            ld.Add(nameof(oConfiguracionGlobal.EmpresaDireccion), _configuracionGlobal.EmpresaDireccion);

            return ld;
        }

        public byte[] Generar()
        {
            var parametros = GetParametrosRpt();
            var datos = new List<RptData>
            {
                new RptData
                {
                    Nombre = "Data",
                    Datos = _guiaRemision.Detalles
                }
            };

            return GenerarPDF.GenerarRpt(_rptPath, parametros, datos);
        }
    }
}
