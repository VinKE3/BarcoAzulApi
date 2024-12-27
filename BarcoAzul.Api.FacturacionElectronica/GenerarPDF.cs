using fyiReporting.RDL;
using System.Collections.Specialized;

namespace BarcoAzul.Api.FacturacionElectronica
{
    public class GenerarPDF
    {
        protected static string GetSource(string file)
        {
            StreamReader fs = null;
            string prog = null;
            try
            {
                fs = new StreamReader(file);
                prog = fs.ReadToEnd();
            }
            finally
            {
                if (fs != null)
                    fs.Close();
            }
            return prog;
        }

        protected static Report GetReport(string prog, string file)
        {
            try
            {
                RDLParser rdlp;
                Report r;

                rdlp = new RDLParser(prog);
                string folder = Path.GetDirectoryName(file);
                if (folder == "")
                    folder = Environment.CurrentDirectory;
                rdlp.Folder = folder;

                r = rdlp.Parse();

                if (r != null)
                {
                    r.Folder = folder;
                    r.Name = Path.GetFileNameWithoutExtension(file);

                    return r;
                }
                else
                {
                    throw new Exception("No se pudo procesar el reporte...");
                }

            }
            catch (Exception e)
            {
                throw e;
            }
        }

        /// <summary>
        /// Método General para crear Reporte.
        /// </summary>
        /// <param name="rptPath">Ruta del reporte.</param>
        /// <param name="parametros">Datos de la cabecera o valor de parametros principales para el reporte.</param>
        /// <param name="detalles">Detalles del reporte.</param>
        /// <param name="nombreDataSource">Nombre del DataSource del reporte.</param>
        /// <returns></returns>
        public static byte[] GenerarRpt(string rptPath, ListDictionary parametros, List<RptData> datos, FormatoInforme formato = FormatoInforme.PDF)
        {
            string source = GetSource(rptPath);
            Report report = GetReport(source, rptPath);

            if (datos != null && datos.Count > 0)
            {
                foreach (var item in datos)
                {
                    DataSet ds = report.DataSets[item.Nombre];

                    if (ds != null)
                        ds.SetData(item.Datos);
                }
            }

            report.RunGetData(parametros);
            MemoryStreamGen sg = null;

            switch (formato)
            {
                case FormatoInforme.PDF:
                    sg = new MemoryStreamGen("", null, "pdf");
                    report.RunRender(sg, OutputPresentationType.PDF, "");
                    break;
                case FormatoInforme.Excel:
                    sg = new MemoryStreamGen("", null, "xlsx");
                    report.RunRender(sg, OutputPresentationType.Excel2007, "");
                    break;
                default:
                    throw new Exception($"El formato {formato} no está soportado.");
            }

            MemoryStream ms = sg.MemoryList[0] as MemoryStream;

            return ms.ToArray();
        }
    }

    public enum FormatoInforme
    {
        PDF,
        Excel
    }

    public class RptData
    {
        public string Nombre { get; set; }
        public IEnumerable<object> Datos { get; set; }
    }
}
