using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros;
using OfficeOpenXml.Style;
using OfficeOpenXml;
using System.Collections.Specialized;
using System.Drawing;

namespace BarcoAzul.Api.Informes.Ventas
{
    public class rRegistroVenta
    {
        private readonly IEnumerable<oRegistroVenta> _registros;
        private readonly oConfiguracionGlobal _configuracionGlobal;
        private readonly oParamRegistroVenta _parametros;
        private readonly string _rptPath;

        public rRegistroVenta(IEnumerable<oRegistroVenta> registros, oConfiguracionGlobal configuracionGlobal, oParamRegistroVenta parametros, string rptPath)
        {
            _registros = registros.OrderBy(x => x.FechaEmision).ThenBy(x => x.NumeroDocumento);
            _configuracionGlobal = configuracionGlobal;
            _parametros = parametros;
            _rptPath = Path.Combine(rptPath, "Ventas", "RptRegistroVenta.rdl");
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
                { nameof(oParamRegistroVenta.FechaInicio), _parametros.FechaInicio },
                { nameof(oParamRegistroVenta.FechaFin), _parametros.FechaFin },
            };
        }
        #endregion

        #region Excel
        private byte[] Excel()
        {
            using (ExcelPackage ep = new())
            {
                ExcelWorksheet sheet = ep.Workbook.Worksheets.Add("Registro de Ventas");

                int row = 1, numeracion = 1;

                sheet.Cells[$"A{row}"].Value = "REGISTRO DE VENTAS";
                sheet.Cells[$"A{row}:J{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.Font.Size = 15;

                row++;

                sheet.Cells[$"A{row}"].Value = $"DESDE: {_parametros.FechaInicio:dd/MM/yyyy} HASTA: {_parametros.FechaFin:dd/MM/yyyy}";
                sheet.Cells[$"A{row}:J{row}"].Merge = true;
                sheet.Cells[$"A{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}"].Style.VerticalAlignment = ExcelVerticalAlignment.Center;

                row++;

                sheet.Cells[$"A{row}"].Value = "Item";
                sheet.Cells[$"B{row}"].Value = "Fecha";
                sheet.Cells[$"C{row}"].Value = "Documento";
                sheet.Cells[$"D{row}"].Value = "Razón Social";
                sheet.Cells[$"E{row}"].Value = "Tipo de Pago";
                sheet.Cells[$"F{row}"].Value = "Guía de Remisión";
                sheet.Cells[$"G{row}"].Value = "Tienda / Vendedor";
                sheet.Cells[$"H{row}"].Value = "Orden de Pedido";
                sheet.Cells[$"I{row}"].Value = "M";
                sheet.Cells[$"J{row}"].Value = "Total";

                sheet.Cells[$"A{row}:J{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"A{row}:J{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}:J{row}"].Style.Font.Color.SetColor(Color.White);
                sheet.Cells[$"A{row}:J{row}"].Style.Fill.PatternType = ExcelFillStyle.Solid;
                sheet.Cells[$"A{row}:J{row}"].Style.Fill.BackgroundColor.SetColor(Color.Black);

                row++;

                int rowInicio = row;

                foreach (var registro in _registros)
                {
                    sheet.Cells[$"A{row}"].Value = numeracion;
                    sheet.Cells[$"B{row}"].Value = registro.FechaEmision;
                    sheet.Cells[$"C{row}"].Value = registro.NumeroDocumento;
                    sheet.Cells[$"D{row}"].Value = registro.IsAnulado ? "A-N-U-L-A-D-O" : registro.ClienteNombre;
                    sheet.Cells[$"E{row}"].Value = registro.TipoCobroDescripcion;
                    sheet.Cells[$"F{row}"].Value = registro.GuiaRemision;
                    sheet.Cells[$"G{row}"].Value = registro.PersonalNombreCompleto;
                    sheet.Cells[$"H{row}"].Value = registro.OrdenPedido;
                    sheet.Cells[$"I{row}"].Value = registro.MonedaId == "S" ? "S/" : "US$";
                    sheet.Cells[$"J{row}"].Value = registro.Total;

                    numeracion++;
                    row++;
                }

                int rowFin = row - 1;

                sheet.Cells[$"B{rowInicio}:B{rowFin}"].Style.Numberformat.Format = "dd/MM/yyyy";
                sheet.Cells[$"F{rowInicio}:F{rowFin},H{rowInicio}:H{rowFin}"].Style.Numberformat.Format = "@";
                sheet.Cells[$"A{rowInicio}:C{rowFin},H{rowInicio}:I{rowFin}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                sheet.Cells[$"J{rowInicio}:J{rowFin}"].Style.Numberformat.Format = "#,###,##0.00";

                sheet.Cells[$"A{row}"].Value = "TOTAL VENTA:";
                sheet.Cells[$"J{row}"].Value = _registros.Sum(x => x.Total);

                sheet.Cells[$"A{row}:I{row}"].Merge = true;
                sheet.Cells[$"A{row}:J{row}"].Style.Font.Bold = true;
                sheet.Cells[$"A{row}"].Style.HorizontalAlignment = ExcelHorizontalAlignment.Right;
                sheet.Cells[$"J{row}:J{row}"].Style.Numberformat.Format = "#,###,##0.00";

                sheet.Cells["A:AZ"].AutoFitColumns();

                return ep.GetAsByteArray();
            }
        }
        #endregion
    }
}
