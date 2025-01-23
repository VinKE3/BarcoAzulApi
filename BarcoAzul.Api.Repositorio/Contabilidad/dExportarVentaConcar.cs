using BarcoAzul.Api.Repositorio.Configuracion;
using Dapper;
using System.Linq;

namespace BarcoAzul.Api.Repositorio.Contabilidad
{
    public class dExportarVentaConcar
    {
        public static void ExportarVenta(string documentoVentaId, string concarNombreBD, string concarEmpresaId, string accion, ref string concarVentaId)
        {
            using (var db = Conexion.Get())
            {
                db.Execute("SP_PROVVENTA", new { Codigo = documentoVentaId, BdConcar = concarNombreBD, Empresa = concarEmpresaId, accion }, commandType: System.Data.CommandType.StoredProcedure, commandTimeout: 300);

                string query = "SELECT Ven_SubDia AS SubDiario, Ven_CCompro AS CCompro FROM Venta WHERE Conf_Codigo + TDoc_Codigo + Ven_Serie + Ven_Numero = @documentoVentaId";

                var registro = db.Query(query, new { documentoVentaId }).FirstOrDefault();

                if (registro != null)
                {
                    concarVentaId = registro.SubDiario + "-" + registro.CCompro;
                }
            }
        }
    }
}
