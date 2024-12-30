using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.Specialized;
using System.Drawing;

namespace BarcoAzul.Api.Informes.Sistema
{
    public class rReportePersonalCliente
    {
        private readonly IEnumerable<oRegistroPersonalCliente> _registros;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly string _rptPath;

        public rReportePersonalCliente(IEnumerable<oRegistroPersonalCliente> registros, oConfiguracionGlobal configuracionGlobal, string rptPath)
        {
            _registros = registros.OrderByDescending(x => x.PersonalId).ThenBy(x => x.ClienteNombre);
            _configuracionGlobal = configuracionGlobal;
            _rptPath = Path.Combine(rptPath, "Sistema", "RptPersonalCliente.rdl");
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
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Reporte de Clientes por Vendedor");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "REPORTE DE CLIENTES POR VENDEDOR";
                sheet.Cells[$"A{row}:E{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = "RUC / DNI";
                sheet.Cells[$"B{row}"].Value = "Razón Social";
                sheet.Cells[$"C{row}"].Value = "Dirección";
                sheet.Cells[$"D{row}"].Value = "Teléfono";
                sheet.Cells[$"E{row}"].Value = "Correo";

                sheet.Cells[$"A{row}:E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}:E{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:E{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:E{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:E{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                var grupos = _registros.GroupBy(x => x.PersonalId);

                foreach (var grupo in grupos)
                {
                    sheet.Cells[$"A{row}"].Value = grupo.First().PersonalNombreCompleto;
                    sheet.Cells[$"A{row}:C{row}"].Merge = true;
                    sheet.Cells[$"A{row}"].Style.Font.Bold = true;

                    sheet.Cells[$"D{row}"].Value = grupo.First().PersonalId;
                    sheet.Cells[$"D{row}:E{row}"].Merge = true;
                    sheet.Cells[$"D{row}"].Style.Font.Bold = true;

                    row++;

                    int rowInicio = row;

                    foreach (var item in grupo)
                    {
                        sheet.Cells[$"A{row}"].Value = item.ClienteNumeroDocumentoIdentidad;
                        sheet.Cells[$"B{row}"].Value = item.ClienteNombre;
                        sheet.Cells[$"C{row}"].Value = item.ClienteDireccion;
                        sheet.Cells[$"D{row}"].Value = item.ClienteTelefono;
                        sheet.Cells[$"E{row}"].Value = item.ClienteCorreoElectronico;

                        numeracion++;
                        row++;
                    }

                    int rowFin = row - 1;

                    sheet.Cells[$"A{rowInicio}:A{rowFin},D{rowInicio}:D{rowFin}"].Style.Numberformat.Format = "@";
                    sheet.Cells[$"A{rowInicio}:A{rowFin},D{rowInicio}:D{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }
        #endregion
    }
}
