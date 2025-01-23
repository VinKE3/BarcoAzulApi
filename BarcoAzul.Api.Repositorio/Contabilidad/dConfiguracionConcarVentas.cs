using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Configuracion;
using Dapper;
using System.Linq;

namespace BarcoAzul.Api.Repositorio.Contabilidad
{
    public class dConfiguracionConcarVentas
    {
        #region CRUD
        public static void Modificar(oConfiguracionConcarVentas configuracionConcarVentas)
        {
            string query = @"   UPDATE Conf_Empresa SET conf_concarBD = @BaseDatosNombre, conf_concarEmpresaId = @EmpresaId, conf_subventas = @SubDiario,
                                conf_ctavtasol = @CuentaSoles, conf_ctavtadol = @CuentaDolares, conf_ctaigv = @CuentaIgv, conf_ctacble = @CuentaContable, 
                                conf_exonerado = @CuentaInafectoExonerado";

            using (var db = Conexion.Get())
            {
                db.Execute(query, configuracionConcarVentas);
            }
        }
        #endregion

        #region Otros Métodos
        public static oConfiguracionConcarVentas GetConfiguracion()
        {
            string query = @"   SELECT
	                                conf_concarBD AS BaseDatosNombre,
	                                conf_concarEmpresaId AS EmpresaId,
	                                conf_subventas AS SubDiario,
	                                conf_ctavtasol AS CuentaSoles,
	                                conf_ctavtadol AS CuentaDolares,
	                                conf_ctaigv AS CuentaIgv,
	                                conf_ctacble AS CuentaContable,
	                                conf_exonerado AS CuentaInafectoExonerado
                                FROM
	                                Conf_Empresa";

            using (var db = Conexion.Get())
            {
                return db.Query<oConfiguracionConcarVentas>(query).FirstOrDefault();
            }
        }
        #endregion
    }
}
