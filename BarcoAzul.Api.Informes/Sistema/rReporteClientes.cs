using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.Specialized;
using System.Drawing;


namespace BarcoAzul.Api.Informes.Sistema
{
    public class rReporteClientes
    {
        private readonly IEnumerable<oRegistroCliente> _registros;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _rptPath;

        public rReporteClientes(IEnumerable<oRegistroCliente> registros, oConfiguracionGlobal configuracionGlobal, string rptPath)
        {
            _registros = registros.OrderBy(x => x.Nombre);
            _configuracionGlobal = configuracionGlobal;
            _rptPath = Path.Combine(rptPath, "Sistema", "RptClientes.rdl");
        }

        public byte[] Generar(FormatoInforme formato)
        {
            return formato switch
            {
                FormatoInforme.PDF => PDF(),
                FormatoInforme.Excel => Excel(),
                _ => throw new NotImplementedException(),
            };
        }

        #region PDF
        private byte[] PDF()
        {
            var parametros = GetParametrosRpt();
            var datos = new List<RptData>
            {
                new RptData
                {
                     Nombre = "Data",
                     Datos = _registros
                }
            };

            return GenerarPDF.GenerarRpt(_rptPath, parametros, datos);
        }

        private ListDictionary GetParametrosRpt()
        {
            return new ListDictionary()
            {
                { nameof(oConfiguracionGlobal.EmpresaNumeroDocumentoIdentidad), _configuracionGlobal.EmpresaNumeroDocumentoIdentidad },
                { nameof(oConfiguracionGlobal.EmpresaNombre), _configuracionGlobal.EmpresaNombre },
                { nameof(oConfiguracionGlobal.EmpresaDireccion), _configuracionGlobal.EmpresaDireccion }
            };
        }
        #endregion

        #region Excel
        private byte[] Excel()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Reporte de Clientes");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "REPORTE DE CLIENTES";
                sheet.Cells[$"A{row}:E{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "Razón Social";
                sheet.Cells[$"C{row}"].Value = "RUC / DNI";
                sheet.Cells[$"D{row}"].Value = "Dirección";
                sheet.Cells[$"E{row}"].Value = "Teléfono";

                sheet.Cells[$"A{row}:E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}:E{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:E{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:E{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:E{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                int rowInicio = row;

                foreach (var registro in _registros)
                {
                    sheet.Cells[$"A{row}"].Value = numeracion;
                    sheet.Cells[$"B{row}"].Value = registro.Nombre;
                    sheet.Cells[$"C{row}"].Value = registro.NumeroDocumentoIdentidad;
                    sheet.Cells[$"D{row}"].Value = registro.Direccion;
                    sheet.Cells[$"E{row}"].Value = registro.Telefono;

                    numeracion++;
                    row++;
                }

                int rowFin = row - 1;

                sheet.Cells[$"C{rowInicio}:C{rowFin},E{rowInicio}:E{rowFin}"].Style.Numberformat.Format = "@";
                sheet.Cells[$"A{rowInicio}:A{rowFin},C{rowInicio}:C{rowFin},E{rowInicio}:E{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }
        #endregion
    }
}
