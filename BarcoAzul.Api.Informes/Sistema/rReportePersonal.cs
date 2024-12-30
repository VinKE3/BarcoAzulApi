using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.Specialized;
using System.Drawing;

namespace BarcoAzul.Api.Informes.Sistema
{
    public class rReportePersonal
    {
        private readonly IEnumerable<oRegistroPersonal> _registros;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _rptPath;

        public rReportePersonal(IEnumerable<oRegistroPersonal> registros, oConfiguracionGlobal configuracionGlobal, string rptPath)
        {
            _registros = registros.OrderBy(x => x.Id);
            _configuracionGlobal = configuracionGlobal;
            _rptPath = Path.Combine(rptPath, "Sistema", "RptPersonal.rdl");
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
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Reporte de Personal");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "REPORTE DE PERSONAL";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "Código";
                sheet.Cells[$"C{row}"].Value = "Apellidos y Nombres";
                sheet.Cells[$"D{row}"].Value = "DNI";
                sheet.Cells[$"E{row}"].Value = "Estado Civil";
                sheet.Cells[$"F{row}"].Value = "Nacimiento";
                sheet.Cells[$"G{row}"].Value = "Cargo";
                sheet.Cells[$"H{row}"].Value = "Estado";

                sheet.Cells[$"A{row}:H{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}:H{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:H{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:H{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:H{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                int rowInicio = row;

                foreach (var registro in _registros)
                {
                    sheet.Cells[$"A{row}"].Value = numeracion;
                    sheet.Cells[$"B{row}"].Value = registro.Id;
                    sheet.Cells[$"C{row}"].Value = registro.NombreCompleto;
                    sheet.Cells[$"D{row}"].Value = registro.NumeroDocumentoIdentidad;
                    sheet.Cells[$"E{row}"].Value = registro.EstadoCivilDescripcion;
                    sheet.Cells[$"F{row}"].Value = registro.FechaNacimiento;
                    sheet.Cells[$"G{row}"].Value = registro.CargoDescripcion;
                    sheet.Cells[$"H{row}"].Value = registro.IsActivo ? "Activo" : "Inactivo";

                    numeracion++;
                    row++;
                }

                int rowFin = row - 1;

                sheet.Cells[$"D{rowInicio}:D{rowFin}"].Style.Numberformat.Format = "@";
                sheet.Cells[$"F{rowInicio}:F{rowFin}"].Style.Numberformat.Format = "dd/MM/yyyy";
                sheet.Cells[$"A{rowInicio}:B{rowFin},D{rowInicio}:F{rowFin},H{rowInicio}:H{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }
        #endregion
    }
}
