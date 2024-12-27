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
    public class PDFNotaPedido
    {
        private readonly oNotaPedido _notaPedido;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private string _rptPath;

        public PDFNotaPedido(oNotaPedido notaPedido, oConfiguracionGlobal configuracionGlobal, string rptPath) {
        
            _notaPedido = notaPedido;
            _configuracionGlobal = configuracionGlobal;
            _rptPath = rptPath;
            CompletarRptPath();

        }

        private void CompletarRptPath()
        {
            string nombreRpt = $"RptNotaPedido_{_notaPedido.Serie}.rdl";

            if (!File.Exists($"{_rptPath}/{nombreRpt}"))
                nombreRpt = "RptNotaPedido";

            _rptPath = $"{_rptPath}/{nombreRpt}";
        }

        private ListDictionary GetParametrosRpt()
        {
            ListDictionary ld = PropertyConverter.ConvertClassToDictionary(_notaPedido);
            ld.Add(nameof(oConfiguracionGlobal.EmpresaNumeroDocumentoIdentidad), _configuracionGlobal.EmpresaNumeroDocumentoIdentidad);
            ld.Add("MontoLetras", Comun.ConvertirNumeroEnLetras(_notaPedido.Total, _notaPedido.MonedaId == "S" ? "PEN" : "USD"));

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
                    Datos = _notaPedido.Detalles
                }
            };

            return GenerarPDF.GenerarRpt(_rptPath, parametros, datos);
        }
    }
}
