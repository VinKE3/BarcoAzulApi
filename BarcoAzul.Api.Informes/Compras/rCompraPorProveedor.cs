using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.Specialized;
using System.Drawing;

namespace BarcoAzul.Api.Informes.Compras
{
    public class rCompraPorProveedor
    {
        private readonly IEnumerable<oCompraPorProveedor> _registros;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly oParamCompraPorProveedor _parametros;
        private readonly string _rptPath;

        public rCompraPorProveedor(IEnumerable<oCompraPorProveedor> registros, oConfiguracionGlobal configuracionGlobal, oParamCompraPorProveedor parametros, string rptPath)
        {
            _registros = parametros.TipoReporte == "S"
                ? registros.OrderBy(x => x.ProveedorNombre).ThenByDescending(x => x.FechaEmision)
                : registros.OrderBy(x => x.ProveedorNombre).ThenByDescending(x => x.FechaEmision).ThenBy(x => x.Item);

            _configuracionGlobal = configuracionGlobal;
            _parametros = parametros;
            string reporte = parametros.TipoReporte == "S" ? "RptCompraPorProveedor.rdl" : "RptCompraPorProveedorDetalle.rdl";
            _rptPath = Path.Combine(rptPath, "Compras", reporte);
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

            if (_parametros.TipoReporte == "C")
                AsignarTotalesProveedor();

            return GenerarPDF.GenerarRpt(_rptPath, parametros, datos);
        }

        private ListDictionary GetParametrosRpt()
        {
            var total = _registros.GroupBy(x => x.ProveedorNumeroDocumentoIdentidad + x.NumeroDocumento).Select(x => x.First()).Sum(x => x.Total);

            return new ListDictionary()
            {
                { nameof(oConfiguracionGlobal.EmpresaNumeroDocumentoIdentidad), _configuracionGlobal.EmpresaNumeroDocumentoIdentidad },
                { nameof(oConfiguracionGlobal.EmpresaNombre), _configuracionGlobal.EmpresaNombre },
                { nameof(oConfiguracionGlobal.EmpresaDireccion), _configuracionGlobal.EmpresaDireccion },
                { nameof(oParamCompraPorProveedor.FechaInicio), _parametros.FechaInicio },
                { nameof(oParamCompraPorProveedor.FechaFin), _parametros.FechaFin },
                { nameof(oParamCompraPorProveedor.MonedaId), _parametros.MonedaId },
                { "Total", total }
            };
        }

        private void AsignarTotalesProveedor()
        {
            var gruposProveedor = _registros.GroupBy(x => x.ProveedorNumeroDocumentoIdentidad);

            foreach (var grupoProveedor in gruposProveedor)
            {
                var gruposDocumento = grupoProveedor.GroupBy(x => x.NumeroDocumento);
                var totalProveedor = gruposDocumento.Select(x => x.First()).Sum(x => x.Total);

                foreach (var grupoDocumento in gruposDocumento)
                {
                    foreach (var item in grupoDocumento)
                    {
                        item.TotalProveedor = totalProveedor;
                    }
                }
            }
        }
        #endregion

        #region Excel
        public byte[] Excel() => _parametros.TipoReporte == "S" ? GetExcelSinDetalle() : GetExcelDetalle();

        private byte[] GetExcelSinDetalle()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Compras por Proveedor");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "COMPRAS POR PROVEEDOR";
                sheet.Cells[$"A{row}:F{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = $"DESDE: {_parametros.FechaInicio:dd/MM/yyyy} HASTA: {_parametros.FechaFin:dd/MM/yyyy}";
                sheet.Cells[$"A{row}:F{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                row++;

                sheet.Cells[$"A{row}"].Value = $"MONEDA: {(_parametros.MonedaId == "S" ? "SOLES" : "DOLARES AMERICANOS")}";
                sheet.Cells[$"A{row}:F{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "Emisión";
                sheet.Cells[$"C{row}"].Value = "Vencimiento";
                sheet.Cells[$"D{row}"].Value = "Documento";
                sheet.Cells[$"E{row}"].Value = "Personal";
                sheet.Cells[$"F{row}"].Value = "Total";

                sheet.Cells[$"A{row}:E{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"F{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"A{row}:F{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:F{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:F{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:F{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);
                row++;

                var grupos = _registros.GroupBy(x => x.ProveedorNumeroDocumentoIdentidad);

                foreach (var grupo in grupos)
                {
                    sheet.Cells[$"A{row}"].Value = $"{grupo.First().ProveedorNombre} - {grupo.First().ProveedorNumeroDocumentoIdentidad}";
                    sheet.Cells[$"A{row}:F{row}"].Merge = true;
                    sheet.Cells[$"A{row}:F{row}"].Style.Font.Bold = true;

                    row++;

                    int rowInicio = row;

                    foreach (var item in grupo)
                    {
                        sheet.Cells[$"A{row}"].Value = numeracion;
                        sheet.Cells[$"B{row}"].Value = item.FechaEmision;
                        sheet.Cells[$"C{row}"].Value = item.FechaVencimiento;
                        sheet.Cells[$"D{row}"].Value = item.NumeroDocumento;
                        sheet.Cells[$"E{row}"].Value = item.PersonalNombreCompleto;
                        sheet.Cells[$"F{row}"].Value = item.Total;

                        numeracion++;
                        row++;
                    }

                    int rowFin = row - 1;

                    sheet.Cells[$"B{rowInicio}:C{rowFin}"].Style.Numberformat.Format = "dd/MM/yyyy";
                    sheet.Cells[$"A{rowInicio}:D{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                    sheet.Cells[$"F{rowInicio}:F{rowFin}"].Style.Numberformat.Format = "#,###,##0.00";

                    sheet.Cells[$"A{row}"].Value = $"TOTAL {grupo.First().ProveedorNombre} >>>>>>";
                    sheet.Cells[$"F{row}"].Value = grupo.Sum(x => x.Total);
                    sheet.Cells[$"A{row}:E{row}"].Merge = true;
                    sheet.Cells[$"A{row}:F{row}"].Style.Font.Bold = true;
                    sheet.Cells[$"A{row}:F{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    sheet.Cells[$"F{row}"].Style.Numberformat.Format = "#,###,##0.00";
                    sheet.Cells[$"A{row}:F{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    row++;
                }

                sheet.Cells[$"A{row}"].Value = "TOTAL GENERAL >>>>>>";
                sheet.Cells[$"F{row}"].Value = _registros.Sum(x => x.Total);
                sheet.Cells[$"A{row}:E{row}"].Merge = true;
                sheet.Cells[$"A{row}:F{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:F{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"F{row}"].Style.Numberformat.Format = "#,###,##0.00";

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }

        private byte[] GetExcelDetalle()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Compras por Proveedor - Detallado");

                int row = 1;

                sheet.Cells[$"A{row}"].Value = "COMPRAS POR PROVEEDOR - DETALLADO";
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

                sheet.Cells[$"A{row}"].Value = $"MONEDA: {(_parametros.MonedaId == "S" ? "SOLES" : "DOLARES AMERICANOS")}";
                sheet.Cells[$"A{row}:I{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                row++;

                sheet.Cells[$"A{row}"].Value = "Emisión";
                sheet.Cells[$"C{row}"].Value = "Vencimiento";
                sheet.Cells[$"D{row}"].Value = "Documento";
                sheet.Cells[$"E{row}"].Value = "Personal";
                sheet.Cells[$"I{row}"].Value = "Total";

                sheet.Cells[$"A{row}:B{row},E{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}:I{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"I{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"A{row}:I{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:I{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:I{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:I{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                var grupos = _registros.GroupBy(x => x.ProveedorNumeroDocumentoIdentidad);

                foreach (var grupo in grupos)
                {
                    sheet.Cells[$"A{row}"].Value = $"{grupo.First().ProveedorNombre} - {grupo.First().ProveedorNumeroDocumentoIdentidad}";
                    sheet.Cells[$"A{row}"].Style.Numberformat.Format = "@";
                    sheet.Cells[$"A{row}:I{row}"].Merge = true;
                    sheet.Cells[$"A{row}"].Style.Font.Bold = true;

                    row++;

                    var gruposDocumento = grupo.GroupBy(x => x.NumeroDocumento);

                    foreach (var grupoDocumento in gruposDocumento)
                    {
                        sheet.Cells[$"A{row}"].Value = grupoDocumento.First().FechaEmision;
                        sheet.Cells[$"C{row}"].Value = grupoDocumento.First().FechaVencimiento;
                        sheet.Cells[$"D{row}"].Value = grupoDocumento.First().NumeroDocumento;
                        sheet.Cells[$"E{row}"].Value = grupoDocumento.First().PersonalNombreCompleto;
                        sheet.Cells[$"I{row}"].Value = grupoDocumento.First().Total;

                        sheet.Cells[$"A{row}:B{row},E{row}:H{row}"].Merge = true;
                        sheet.Cells[$"A{row}:C{row}"].Style.Numberformat.Format = "dd/MM/yyyy";
                        sheet.Cells[$"A{row}:D{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[$"I{row}"].Style.Numberformat.Format = "#,###,##0.00";

                        row++;

                        int rowInicio = row;

                        foreach (var item in grupoDocumento)
                        {
                            sheet.Cells[$"B{row}"].Value = item.Item;
                            sheet.Cells[$"C{row}"].Value = item.ArticuloDescripcion;
                            sheet.Cells[$"E{row}"].Value = item.UnidadMedidaDescripcion;
                            sheet.Cells[$"F{row}"].Value = item.Cantidad;
                            sheet.Cells[$"G{row}"].Value = item.PrecioUnitario;
                            sheet.Cells[$"H{row}"].Value = item.Importe;

                            sheet.Cells[$"C{row}:D{row}"].Merge = true;
                            sheet.Cells[$"B{row}:H{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                            row++;
                        }

                        int rowFin = row - 1;

                        sheet.Cells[$"B{rowInicio}:B{rowFin},E{rowInicio}:E{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                        sheet.Cells[$"F{rowInicio}:H{rowFin}"].Style.Numberformat.Format = "#,###,##0.00";
                    }

                    sheet.Cells[$"A{row}"].Value = $"TOTAL {grupo.First().ProveedorNombre} >>>>>>";
                    sheet.Cells[$"I{row}"].Value = gruposDocumento.Select(x => x.First()).Sum(x => x.Total);
                    sheet.Cells[$"A{row}:H{row}"].Merge = true;
                    sheet.Cells[$"A{row}:I{row}"].Style.Font.Bold = true;
                    sheet.Cells[$"A{row}:I{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                    sheet.Cells[$"I{row}"].Style.Numberformat.Format = "#,###,##0.00";
                    sheet.Cells[$"A{row}:I{row}"].Style.Border.Bottom.Style = ExcelBorderStyle.Thin;

                    row++;
                }

                sheet.Cells[$"A{row}"].Value = "TOTAL GENERAL >>>>>>";
                sheet.Cells[$"I{row}"].Value = _registros.GroupBy(x => x.ProveedorNumeroDocumentoIdentidad + x.NumeroDocumento).Select(x => x.First()).Sum(x => x.Total);
                sheet.Cells[$"A{row}:H{row}"].Merge = true;
                sheet.Cells[$"A{row}:I{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:I{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"I{row}"].Style.Numberformat.Format = "#,###,##0.00";

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }

        }
        #endregion
    }
}
