using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.Specialized;
using System.Drawing;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;

namespace BarcoAzul.Api.Informes.Gerencia
{
    public class rCompraPorArticulo
    {
        private readonly IEnumerable<oCompraPorArticulo> _registros;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly oParamCompraPorArticulo _parametros;
        private readonly string _rptPath;

        public rCompraPorArticulo(IEnumerable<oCompraPorArticulo> registros, oConfiguracionGlobal configuracionGlobal, oParamCompraPorArticulo parametros, string rptPath)
        {
            _registros = parametros.TipoReporte == "AC"
                ? registros.OrderByDescending(x => x.Cantidad) :
                registros.OrderBy(x => x.SubLineaDescripcion).ThenBy(x => x.ArticuloDescripcion).ThenBy(x => x.FechaEmision).ThenBy(x => x.NumeroDocumento);
            _configuracionGlobal = configuracionGlobal;
            _parametros = parametros;
            string reporte = parametros.TipoReporte switch
            {
                "CA" => "RptCompraPorArticuloDetalle.rdl",
                "AC" => "RptCompraPorArticuloResumen.rdl",
                _ => "RptCompraPorArticuloProduccion.rdl"
            };
            _rptPath = Path.Combine(rptPath, "Gerencia", reporte);
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
                { nameof(oParamCompraPorArticulo.FechaInicio), _parametros.FechaInicio },
                { nameof(oParamCompraPorArticulo.FechaFin), _parametros.FechaFin },
            };
        }
        #endregion

        #region Excel
        private byte[] Excel() => _parametros.TipoReporte == "AC" ? GetExcelResumen() : GetExcelDetalle();

        private byte[] GetExcelDetalle()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add(_parametros.TipoReporte == "CA" ? "Compra por Artículo" : "Artículo Producción");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = _parametros.TipoReporte == "CA" ? "COMPRA POR ARTICULO" : "ARTICULO PRODUCCION";
                sheet.Cells[$"A{row}:G{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = $"DESDE: {_parametros.FechaInicio:dd/MM/yyyy} HASTA: {_parametros.FechaFin:dd/MM/yyyy}";
                sheet.Cells[$"A{row}:G{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "Fecha";
                sheet.Cells[$"C{row}"].Value = "Documento";
                sheet.Cells[$"D{row}"].Value = "Razón Social / Apellidos y Nombres";
                sheet.Cells[$"E{row}"].Value = "RUC / DNI";
                sheet.Cells[$"F{row}"].Value = "Unidad";
                sheet.Cells[$"G{row}"].Value = "Cantidad";

                sheet.Cells[$"A{row}:F{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"A{row}:G{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:G{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:G{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:G{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                var grupos = _registros.GroupBy(x => x.ArticuloDescripcion);

                foreach (var grupo in grupos)
                {
                    sheet.Cells[$"A{row}"].Value = grupo.First().ArticuloDescripcion;

                    sheet.Cells[$"A{row}:G{row}"].Merge = true;
                    sheet.Cells[$"A{row}:G{row}"].Style.Font.Bold = true;

                    row++;

                    int rowInicio = row;

                    foreach (var item in grupo)
                    {
                        sheet.Cells[$"A{row}"].Value = numeracion;
                        sheet.Cells[$"B{row}"].Value = item.FechaEmision;
                        sheet.Cells[$"C{row}"].Value = item.NumeroDocumento;
                        sheet.Cells[$"D{row}"].Value = item.ProveedorNombre;
                        sheet.Cells[$"E{row}"].Value = item.ProveedorNumeroDocumentoIdentidad;
                        sheet.Cells[$"F{row}"].Value = item.UnidadMedidaAbreviatura;
                        sheet.Cells[$"G{row}"].Value = item.Cantidad;

                        numeracion++;
                        row++;
                    }

                    int rowFin = row - 1;

                    sheet.Cells[$"A{rowInicio}:C{rowFin}, E{rowInicio}:F{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[$"B{rowInicio}:B{rowFin}"].Style.Numberformat.Format = "dd/MM/yyyy";
                    sheet.Cells[$"E{rowInicio}:E{rowFin}"].Style.Numberformat.Format = "@";
                    sheet.Cells[$"G{rowInicio}:G{rowFin}"].Style.Numberformat.Format = "#,###,##0.00";

                    sheet.Cells[$"A{row}"].Value = "TOTAL ARTICULO:";
                    sheet.Cells[$"G{row}"].Value = grupo.Sum(x => x.Cantidad);
                    sheet.Cells[$"A{row}:F{row}"].Merge = true;
                    sheet.Cells[$"A{row}:G{row}"].Style.Font.Bold = true;
                    sheet.Cells[$"A{row}:G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    sheet.Cells[$"G{row}"].Style.Numberformat.Format = "#,###,##0.00";
                    sheet.Cells[$"A{row}:G{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    row++;
                }

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }

        private byte[] GetExcelResumen()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Artículos más Comprados");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "ARTICULOS MAS COMPRADOS";
                sheet.Cells[$"A{row}:G{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = $"DESDE: {_parametros.FechaInicio:dd/MM/yyyy} HASTA: {_parametros.FechaFin:dd/MM/yyyy}";
                sheet.Cells[$"A{row}:G{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "Código";
                sheet.Cells[$"C{row}"].Value = "Descripción";
                sheet.Cells[$"D{row}"].Value = "Unidad";
                sheet.Cells[$"E{row}"].Value = "Precio";
                sheet.Cells[$"F{row}"].Value = "Cantidad";
                sheet.Cells[$"G{row}"].Value = "Importe";

                sheet.Cells[$"A{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"E{row}:G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"A{row}:G{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:G{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:G{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:G{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                int rowInicio = row;

                foreach (var item in _registros)
                {
                    sheet.Cells[$"A{row}"].Value = numeracion;
                    sheet.Cells[$"B{row}"].Value = item.ArticuloId;
                    sheet.Cells[$"C{row}"].Value = item.ArticuloDescripcion;
                    sheet.Cells[$"D{row}"].Value = item.UnidadMedidaAbreviatura;
                    sheet.Cells[$"E{row}"].Value = item.PrecioUnitario;
                    sheet.Cells[$"F{row}"].Value = item.Cantidad;
                    sheet.Cells[$"G{row}"].Value = item.Importe;

                    numeracion++;
                    row++;
                }

                int rowFin = row - 1;

                sheet.Cells[$"A{rowInicio}:B{rowFin}, D{rowInicio}:D{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"B{rowInicio}:B{rowFin}"].Style.Numberformat.Format = "@";
                sheet.Cells[$"E{rowInicio}:G{rowFin}"].Style.Numberformat.Format = "#,###,##0.00";

                sheet.Cells[$"A{row}"].Value = "TOTAL IMPORTE >>>>";
                sheet.Cells[$"G{row}"].Value = _registros.Sum(x => x.Importe);
                sheet.Cells[$"A{row}:F{row}"].Merge = true;
                sheet.Cells[$"A{row}:G{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:G{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"G{row}"].Style.Numberformat.Format = "#,###,##0.00";
                sheet.Cells[$"A{row}:G{row}"].Style.Border.Top.Style = ExcelBorderStyle.Thin;

                row++;


                sheet.Cells["A:AZ"].AutoFitColumns();
                return ep.GetAsByteArray();
            }

        }
        #endregion
    }
}
