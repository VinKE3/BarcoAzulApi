using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Utilidades;
using System.Collections.Specialized;

namespace BarcoAzul.Api.Informes.PDFs
{
    public class PDFSalidaAlmacen
    {
        private readonly oSalidaAlmacen _salidaAlmacen;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private string _rptPath;

        public PDFSalidaAlmacen(oSalidaAlmacen salidaAlmacen, oConfiguracionGlobal configuracionGlobal, string rptPath)
        {
            _salidaAlmacen = salidaAlmacen;
            _configuracionGlobal = configuracionGlobal;
            _rptPath = rptPath;
            CompletarRptPath();
        }

        private void CompletarRptPath()
        {
            string nombreRpt = $"RptSalidaAlmacen_{_salidaAlmacen.Serie}.rdl";

            if (!File.Exists($"{_rptPath}/{nombreRpt}"))
                nombreRpt = "RptSalidaAlmacen.rdl";

            _rptPath = $"{_rptPath}/{nombreRpt}";
        }

        private ListDictionary GetParametrosRpt()
        {
            ListDictionary ld = PropertyConverter.ConvertClassToDictionary(_salidaAlmacen);
            ld.Add(nameof(oConfiguracionGlobal.EmpresaNumeroDocumentoIdentidad), _configuracionGlobal.EmpresaNumeroDocumentoIdentidad);
            ld.Add("TotalKilogramos", _salidaAlmacen.Detalles.Where(x => x.UnidadMedidaDescripcion == "KG").Sum(x => x.Cantidad));

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
                    Datos = _salidaAlmacen.Detalles
                }
            };

            return GenerarPDF.GenerarRpt(_rptPath, parametros, datos);
        }
    }
}
