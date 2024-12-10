using BarcoAzul.Api.Modelos.Entidades;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Empresa
{
    public class dEmpresaRetencion : dComun
    {
        public dEmpresaRetencion(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oConfiguracionEmpresaRetencion> empresaRetenciones)
        {
            string query = "INSERT INTO Empresa_Retencion (Conf_Codigo, Reten_Porcentaje, Reten_PorDefecto) VALUES (@EmpresaId, @Porcentaje, @Default)";

            using (var db = GetConnection())
            {
                foreach (var empresaRetencion in empresaRetenciones)
                {
                    await db.ExecuteAsync(query, new
                    {
                        empresaRetencion.EmpresaId,
                        empresaRetencion.Porcentaje,
                        Default = empresaRetencion.Default ? "S" : "N"
                    });
                }
            }
        }

        public async Task Eliminar()
        {
            string query = "DELETE Empresa_Retencion";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query);
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oConfiguracionEmpresaRetencion>> ListarTodos()
        {
            string query = "SELECT Conf_Codigo AS EmpresaId, Reten_Porcentaje AS Porcentaje, CAST(CASE WHEN Reten_PorDefecto = 'S' THEN 1 ELSE 0 END AS BIT) AS 'Default' FROM Empresa_Retencion";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oConfiguracionEmpresaRetencion>(query);
            }
        }
        #endregion
    }
}
