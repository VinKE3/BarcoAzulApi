using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.Specialized;
using System.Drawing;

namespace BarcoAzul.Api.Informes.Compras
{
    public class rRegistroCompra
    {
        private readonly IEnumerable<oRegistroCompra> _registros;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly oParamRegistroCompra _parametros;
        private readonly string _rptPath;

        public rRegistroCompra(IEnumerable<oRegistroCompra> registros, oConfiguracionGlobal configuracionGlobal, oParamRegistroCompra parametros, string rptPath)
        {
            _registros = registros.OrderBy(x => x.FechaContable).ThenBy(x => x.FechaEmision);
            _configuracionGlobal = configuracionGlobal;
            _parametros = parametros;
            _rptPath = Path.Combine(rptPath, "Compras", "RptRegistroCompra.rdl");
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
                { nameof(oParamRegistroCompra.FechaInicio), _parametros.FechaInicio },
                { nameof(oParamRegistroCompra.FechaFin), _parametros.FechaFin }
            };
        }
        #endregion

        #region Excel
        public byte[] Excel()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Registro de Compras");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "REGISTRO DE COMPRAS";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = $"DESDE: {_parametros.FechaInicio:dd/MM/yyyy} HASTA: {_parametros.FechaFin:dd/MM/yyyy}";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "Fecha";
                sheet.Cells[$"C{row}"].Value = "Emisión";
                sheet.Cells[$"D{row}"].Value = "Documento";
                sheet.Cells[$"E{row}"].Value = "Razón Social";
                sheet.Cells[$"F{row}"].Value = "RUC / DNI";
                sheet.Cells[$"G{row}"].Value = "M";
                sheet.Cells[$"H{row}"].Value = "Total";

                sheet.Cells[$"A{row}:G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"H{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"A{row}:H{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:H{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:H{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:H{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                int rowInicio = row;

                foreach (var registro in _registros)
                {
                    sheet.Cells[$"A{row}"].Value = numeracion;
                    sheet.Cells[$"B{row}"].Value = registro.FechaContable;
                    sheet.Cells[$"C{row}"].Value = registro.FechaEmision;
                    sheet.Cells[$"D{row}"].Value = registro.NumeroDocumento;
                    sheet.Cells[$"E{row}"].Value = registro.ProveedorNombre;
                    sheet.Cells[$"F{row}"].Value = registro.ProveedorNumeroDocumentoIdentidad;
                    sheet.Cells[$"G{row}"].Value = registro.MonedaId == "S" ? "S/" : "US$";
                    sheet.Cells[$"H{row}"].Value = registro.TipoDocumentoId != "07" ? registro.Total : registro.Total * -1;

                    numeracion++;
                    row++;
                }

                int rowFin = row - 1;

                sheet.Cells[$"B{rowInicio}:C{rowFin}"].Style.Numberformat.Format = "dd/MM/yyyy";
                sheet.Cells[$"F{rowInicio}:F{rowFin}"].Style.Numberformat.Format = "@";
                sheet.Cells[$"A{rowInicio}:D{rowFin},F{rowInicio}:G{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"H{rowInicio}:H{rowFin}"].Style.Numberformat.Format = "#,###,##0.00";

                sheet.Cells[$"F{row}"].Value = "TOTAL COMPRA:";
                sheet.Cells[$"H{row}"].Value = _registros.Sum(x => x.TipoDocumentoId != "07" ? x.Total : x.Total * -1);

                sheet.Cells[$"F{row}:G{row}"].Merge = true;
                sheet.Cells[$"F{row}:H{row}"].Style.Font.Bold = true;
                sheet.Cells[$"F{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"H{row}:H{row}"].Style.Numberformat.Format = "#,###,##0.00";

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }
        #endregion
    }
}
