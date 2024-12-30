using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.Specialized;
using System.Drawing;

namespace BarcoAzul.Api.Informes.Cobranzas
{
    public class rInformeCobranza
    {
        private readonly IEnumerable<oInformeCobranza> _registros;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly oParamInformeCobranza _parametros;
        private readonly string _rptPath;

        public rInformeCobranza(IEnumerable<oInformeCobranza> registros, oConfiguracionGlobal configuracionGlobal, oParamInformeCobranza parametros, string rptPath)
        {
            _registros = registros.OrderBy(x => x.TipoDocumentoId).ThenBy(x => x.FechaEmision).ThenBy(x => x.NumeroDocumento);
            _configuracionGlobal = configuracionGlobal;
            _parametros = parametros;
            _rptPath = Path.Combine(rptPath, "Cobranzas", "RptInformeCobranza.rdl");
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
                { nameof(oConfiguracionGlobal.EmpresaDireccion), _configuracionGlobal.EmpresaDireccion },
                { nameof(oParamInformeCobranza.FechaInicio), _parametros.FechaInicio },
                { nameof(oParamInformeCobranza.FechaFin), _parametros.FechaFin },
                { nameof(oParamInformeCobranza.MonedaId), _parametros.MonedaId }
            };
        }
        #endregion

        #region Excel
        private byte[] Excel()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Informe de Cobranzas");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "INFORME DE COBRANZAS";
                sheet.Cells[$"A{row}:I{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = $"DESDE: {_parametros.FechaInicio:dd/MM/yyyy} HASTA: {_parametros.FechaFin:dd/MM/yyyy}";
                sheet.Cells[$"A{row}:I{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                row++;

                sheet.Cells[$"A{row}"].Value = $"MONEDA: {(_parametros.MonedaId == "S" ? "SOLES" : "DÓLARES AMERICANOS")}";
                sheet.Cells[$"A{row}:I{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "Fecha";
                sheet.Cells[$"C{row}"].Value = "Personal";
                sheet.Cells[$"D{row}"].Value = "Documento N°";
                sheet.Cells[$"E{row}"].Value = "Vencimiento";
                sheet.Cells[$"F{row}"].Value = "Razón Social";
                sheet.Cells[$"G{row}"].Value = "Total";
                sheet.Cells[$"H{row}"].Value = "Abonado";
                sheet.Cells[$"I{row}"].Value = "Saldo";

                sheet.Cells[$"A{row}:I{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"G{row}:I{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"A{row}:I{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:I{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:I{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:I{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                var gruposTipoDocumento = _registros.GroupBy(x => x.TipoDocumentoId);

                foreach (var grupoTipoDocumento in gruposTipoDocumento)
                {
                    sheet.Cells[$"A{row}"].Value = grupoTipoDocumento.First().TipoDocumentoDescripcion;
                    sheet.Cells[$"A{row}:I{row}"].Merge = true;
                    sheet.Cells[$"A{row}"].Style.Font.Bold = true;

                    row++;

                    int rowInicio = row;

                    foreach (var item in grupoTipoDocumento)
                    {
                        sheet.Cells[$"A{row}"].Value = numeracion;
                        sheet.Cells[$"B{row}"].Value = item.FechaEmision;
                        sheet.Cells[$"C{row}"].Value = item.PersonalNombreCompleto;
                        sheet.Cells[$"D{row}"].Value = item.NumeroDocumento;
                        sheet.Cells[$"E{row}"].Value = item.FechaVencimiento;
                        sheet.Cells[$"F{row}"].Value = item.ClienteNombre;
                        sheet.Cells[$"G{row}"].Value = item.Total;
                        sheet.Cells[$"H{row}"].Value = item.Abono;
                        sheet.Cells[$"I{row}"].Value = item.Saldo;

                        numeracion++;
                        row++;
                    }

                    int rowFin = row - 1;

                    sheet.Cells[$"B{rowInicio}:B{rowFin},E{rowInicio}:E{rowFin}"].Style.Numberformat.Format = "dd/MM/yyyy";
                    sheet.Cells[$"D{rowInicio}:D{rowFin}"].Style.Numberformat.Format = "@";
                    sheet.Cells[$"A{rowInicio}:B{rowFin},D{rowInicio}:E{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[$"G{rowInicio}:I{rowFin}"].Style.Numberformat.Format = "#,###,##0.00";

                    sheet.Cells[$"A{row}"].Value = $"TOTAL {grupoTipoDocumento.First().TipoDocumentoDescripcion} >>>";
                    sheet.Cells[$"G{row}"].Value = grupoTipoDocumento.Sum(x => x.Total);
                    sheet.Cells[$"H{row}"].Value = grupoTipoDocumento.Sum(x => x.Abono);
                    sheet.Cells[$"I{row}"].Value = grupoTipoDocumento.Sum(x => x.Saldo);

                    sheet.Cells[$"A{row}:F{row}"].Merge = true;
                    sheet.Cells[$"A{row}:I{row}"].Style.Font.Bold = true;
                    sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    sheet.Cells[$"G{row}:I{row}"].Style.Numberformat.Format = "#,###,##0.00";
                    sheet.Cells[$"A{row}:I{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    row++;
                }

                sheet.Cells[$"A{row}"].Value = "TOTALES >>>";
                sheet.Cells[$"G{row}"].Value = _registros.Sum(x => x.Total);
                sheet.Cells[$"H{row}"].Value = _registros.Sum(x => x.Abono);
                sheet.Cells[$"I{row}"].Value = _registros.Sum(x => x.Saldo);

                sheet.Cells[$"A{row}:F{row}"].Merge = true;
                sheet.Cells[$"A{row}:I{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"G{row}:I{row}"].Style.Numberformat.Format = "#,###,##0.00";

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }
        #endregion
    }
}
