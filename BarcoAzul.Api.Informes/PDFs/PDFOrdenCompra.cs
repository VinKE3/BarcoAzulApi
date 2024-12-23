using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Utilidades;
using System.Collections.Specialized;

namespace BarcoAzul.Api.Informes.PDFs
{
    public class PDFOrdenCompra
    {
        private readonly oOrdenCompra _ordenCompra;
        private readonly oConfiguracionEmpresa _configuracionEmpresa;
        private string _rptPath;

        public PDFOrdenCompra(oOrdenCompra ordenCompra, oConfiguracionEmpresa configuracionEmpresa, string rptPath)
        {
            _ordenCompra = ordenCompra;
            _configuracionEmpresa = configuracionEmpresa;
            _rptPath = rptPath;
            CompletarRptPath();
        }

        private void CompletarRptPath()
        {
            string nombreRpt = $"RptOrdenCompra_{_ordenCompra.Serie}.rdl";

            if (!File.Exists($"{_rptPath}/{nombreRpt}"))
                nombreRpt = "RptOrdenCompra.rdl";

            _rptPath = $"{_rptPath}/{nombreRpt}";
        }

        private ListDictionary GetParametrosRpt()
        {
            ListDictionary ld = PropertyConverter.ConvertClassToDictionary(_ordenCompra);
            ld.Add("EmpresaNumeroDocumentoIdentidad", _configuracionEmpresa.NumeroDocumentoIdentidad);
            ld.Add("EmpresaNombre", _configuracionEmpresa.Nombre);
            ld.Add("EmpresaTelefono", _configuracionEmpresa.Telefono);
            ld.Add("EmpresaDireccion", _configuracionEmpresa.Direccion);
            ld.Add("MontoLetras", Comun.ConvertirNumeroEnLetras(_ordenCompra.Total, _ordenCompra.MonedaId == "S" ? "PEN" : "USD"));

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
                    Datos = _ordenCompra.Detalles
                }
            };

            return GenerarPDF.GenerarRpt(_rptPath, parametros, datos);
        }
    }
}
