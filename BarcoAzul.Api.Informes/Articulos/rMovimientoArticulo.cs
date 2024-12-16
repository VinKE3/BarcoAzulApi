using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.Specialized;
using System.Drawing;

namespace BarcoAzul.Api.Informes.Articulos
{
    public class rMovimientoArticulo
    {
        private readonly IEnumerable<oMovimientoArticulo> _registros;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly oParamMovimientoArticulo _parametros;
        private readonly string _rptPath;

        public rMovimientoArticulo(IEnumerable<oMovimientoArticulo> registros, oConfiguracionGlobal configuracionGlobal, oParamMovimientoArticulo parametros, string rptPath)
        {
            _registros = parametros.AgrupadoPor switch
            {
                "MA" => registros.OrderBy(x => x.MarcaNombre).ThenBy(x => x.SubLineaDescripcion).ThenBy(x => x.ArticuloDescripcion),
                "LI" => registros.OrderBy(x => x.LineaDescripcion).ThenBy(x => x.MarcaNombre).ThenBy(x => x.ArticuloDescripcion),
                _ => registros.OrderBy(x => x.LineaDescripcion).ThenBy(x => x.SubLineaDescripcion).ThenBy(x => x.ArticuloDescripcion).ThenBy(x => x.CodigoBarras)
            };

            _configuracionGlobal = configuracionGlobal;
            _parametros = parametros;

            string reporte = parametros.AgrupadoPor switch
            {
                "MA" => "RptMovimientoArticuloMarca.rdl",
                "LI" => "RptMovimientoArticuloLinea.rdl",
                _ => "RptMovimientoArticuloLineaSubLinea.rdl",
            };

            _rptPath = Path.Combine(rptPath, "Articulos", reporte);
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
                { nameof(oParamMovimientoArticulo.FechaInicio), _parametros.FechaInicio },
                { nameof(oParamMovimientoArticulo.FechaFin), _parametros.FechaFin },
                { nameof(oParamMovimientoArticulo.TipoExistenciaDescripcion), _parametros.TipoExistenciaDescripcion }
            };
        }
        #endregion

        #region Excel
        private byte[] Excel()
        {
            return _parametros.AgrupadoPor switch
            {
                "MA" => GetExcelPorMarca(),
                "LI" => GetExcelPorLinea(),
                _ => GetExcelPorLineaSubLinea()
            };
        }

        private byte[] GetExcelPorMarca()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Movimientos de Artículos");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "MOVIMIENTOS DE ARTICULOS";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = $"Desde: {_parametros.FechaInicio:dd/MM/yyyy} Hasta: {_parametros.FechaFin:dd/MM/yyyy}";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 11;

                row++;

                sheet.Cells[$"A{row}"].Value = $"TIPO DE EXISTENCIA: {_parametros.TipoExistenciaDescripcion}";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 11;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "SubLínea";
                sheet.Cells[$"C{row}"].Value = "Descripción";
                sheet.Cells[$"D{row}"].Value = "Unidad";
                sheet.Cells[$"E{row}"].Value = "Stock Inicial";
                sheet.Cells[$"F{row}"].Value = "Entradas";
                sheet.Cells[$"G{row}"].Value = "Salidas";
                sheet.Cells[$"H{row}"].Value = "Stock Final";

                sheet.Cells[$"A{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"E{row}:H{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"A{row}:H{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:H{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:H{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:H{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                var grupos = _registros.GroupBy(x => x.MarcaNombre);

                foreach (var grupo in grupos)
                {
                    sheet.Cells[$"A{row}"].Value = $"MARCA: {grupo.First().MarcaNombre}";
                    sheet.Cells[$"A{row}:H{row}"].Merge = true;
                    sheet.Cells[$"A{row}"].Style.Font.Bold = true;

                    row++;

                    int rowInicio = row;

                    foreach (var item in grupo)
                    {
                        sheet.Cells[$"A{row}"].Value = numeracion;
                        sheet.Cells[$"B{row}"].Value = item.SubLineaDescripcion;
                        sheet.Cells[$"C{row}"].Value = item.ArticuloDescripcion;
                        sheet.Cells[$"D{row}"].Value = item.UnidadMedidaAbreviatura;
                        sheet.Cells[$"E{row}"].Value = item.StockInicial;
                        sheet.Cells[$"F{row}"].Value = item.CantidadEntrada;
                        sheet.Cells[$"G{row}"].Value = item.CantidadSalida;
                        sheet.Cells[$"H{row}"].Value = item.SaldoFinal;

                        numeracion++;
                        row++;
                    }

                    int rowFin = row - 1;

                    sheet.Cells[$"E{rowInicio}:H{rowFin}"].Style.Numberformat.Format = "#,###,##0.00";
                    sheet.Cells[$"A{rowInicio}:A{rowFin},D{rowInicio}:D{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }

        private byte[] GetExcelPorLinea()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Movimientos de Artículos");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "MOVIMIENTOS DE ARTICULOS";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = $"Desde: {_parametros.FechaInicio:dd/MM/yyyy} Hasta: {_parametros.FechaFin:dd/MM/yyyy}";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 11;

                row++;

                sheet.Cells[$"A{row}"].Value = $"TIPO DE EXISTENCIA: {_parametros.TipoExistenciaDescripcion}";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 11;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "Marca";
                sheet.Cells[$"C{row}"].Value = "Descripción";
                sheet.Cells[$"D{row}"].Value = "Unidad";
                sheet.Cells[$"E{row}"].Value = "Stock Inicial";
                sheet.Cells[$"F{row}"].Value = "Entradas";
                sheet.Cells[$"G{row}"].Value = "Salidas";
                sheet.Cells[$"H{row}"].Value = "Stock Final";

                sheet.Cells[$"A{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"E{row}:H{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"A{row}:H{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:H{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:H{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:H{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                var grupos = _registros.GroupBy(x => x.LineaDescripcion);

                foreach (var grupo in grupos)
                {
                    sheet.Cells[$"A{row}"].Value = $"LINEA: {grupo.First().LineaDescripcion}";
                    sheet.Cells[$"A{row}:H{row}"].Merge = true;
                    sheet.Cells[$"A{row}"].Style.Font.Bold = true;

                    row++;

                    int rowInicio = row;

                    foreach (var item in grupo)
                    {
                        sheet.Cells[$"A{row}"].Value = numeracion;
                        sheet.Cells[$"B{row}"].Value = item.MarcaNombre;
                        sheet.Cells[$"C{row}"].Value = item.ArticuloDescripcion;
                        sheet.Cells[$"D{row}"].Value = item.UnidadMedidaAbreviatura;
                        sheet.Cells[$"E{row}"].Value = item.StockInicial;
                        sheet.Cells[$"F{row}"].Value = item.CantidadEntrada;
                        sheet.Cells[$"G{row}"].Value = item.CantidadSalida;
                        sheet.Cells[$"H{row}"].Value = item.SaldoFinal;

                        numeracion++;
                        row++;
                    }

                    int rowFin = row - 1;

                    sheet.Cells[$"E{rowInicio}:H{rowFin}"].Style.Numberformat.Format = "#,###,##0.00";
                    sheet.Cells[$"A{rowInicio}:A{rowFin},D{rowInicio}:D{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                }

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }

        private byte[] GetExcelPorLineaSubLinea()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Movimientos de Artículos");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "MOVIMIENTOS DE ARTICULOS";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = $"Desde: {_parametros.FechaInicio:dd/MM/yyyy} Hasta: {_parametros.FechaFin:dd/MM/yyyy}";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 11;

                row++;

                sheet.Cells[$"A{row}"].Value = $"TIPO DE EXISTENCIA: {_parametros.TipoExistenciaDescripcion}";
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 11;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "Código";
                sheet.Cells[$"C{row}"].Value = "Descripción";
                sheet.Cells[$"D{row}"].Value = "Unidad";
                sheet.Cells[$"E{row}"].Value = "Stock Inicial";
                sheet.Cells[$"F{row}"].Value = "Entradas";
                sheet.Cells[$"G{row}"].Value = "Salidas";
                sheet.Cells[$"H{row}"].Value = "Stock Final";

                sheet.Cells[$"A{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"E{row}:H{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"A{row}:H{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:H{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:H{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:H{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                var gruposLinea = _registros.GroupBy(x => x.LineaDescripcion);

                foreach (var grupoLinea in gruposLinea)
                {
                    sheet.Cells[$"A{row}"].Value = $"LINEA: {grupoLinea.First().LineaDescripcion}";
                    sheet.Cells[$"A{row}:H{row}"].Merge = true;
                    sheet.Cells[$"A{row}"].Style.Font.Bold = true;

                    row++;

                    var gruposSubLinea = grupoLinea.GroupBy(x => x.SubLineaDescripcion);

                    foreach (var grupoSubLinea in gruposSubLinea)
                    {
                        sheet.Cells[$"A{row}"].Value = $"    {grupoSubLinea.First().SubLineaDescripcion}";
                        sheet.Cells[$"A{row}:H{row}"].Merge = true;
                        sheet.Cells[$"A{row}"].Style.Font.Bold = true;

                        row++;

                        int rowInicio = row;

                        foreach (var item in grupoSubLinea)
                        {
                            sheet.Cells[$"A{row}"].Value = numeracion;
                            sheet.Cells[$"B{row}"].Value = item.CodigoBarras;
                            sheet.Cells[$"C{row}"].Value = item.ArticuloDescripcion;
                            sheet.Cells[$"D{row}"].Value = item.UnidadMedidaAbreviatura;
                            sheet.Cells[$"E{row}"].Value = item.StockInicial;
                            sheet.Cells[$"F{row}"].Value = item.CantidadEntrada;
                            sheet.Cells[$"G{row}"].Value = item.CantidadSalida;
                            sheet.Cells[$"H{row}"].Value = item.SaldoFinal;

                            numeracion++;
                            row++;
                        }

                        int rowFin = row - 1;

                        sheet.Cells[$"E{rowInicio}:H{rowFin}"].Style.Numberformat.Format = "#,##0.00";
                        sheet.Cells[$"A{rowInicio}:B{rowFin},D{rowInicio}:D{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    }
                }

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }
        #endregion
    }
}
