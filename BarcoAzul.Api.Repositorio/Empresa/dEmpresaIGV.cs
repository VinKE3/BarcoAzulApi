using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Empresa
{
    public class dEmpresaIGV : dComun
    {
        public dEmpresaIGV(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oConfiguracionEmpresaIGV> empresaIGVs)
        {
            string query = "INSERT INTO Empresa_Igv (Conf_Codigo, Igv_Porcentaje, Igv_PorDefecto) VALUES (@EmpresaId, @Porcentaje, @Default)";

            using (var db = GetConnection())
            {
                foreach (var empresaIGV in empresaIGVs)
                {
                    await db.ExecuteAsync(query, new
                    {
                        empresaIGV.EmpresaId,
                        empresaIGV.Porcentaje,
                        Default = empresaIGV.Default ? "S" : "N"
                    });
                }
            }
        }

        public async Task Eliminar()
        {
            string query = "DELETE Empresa_Igv";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query);
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oConfiguracionEmpresaIGV>> ListarTodos()
        {
            string query = "SELECT Conf_Codigo AS EmpresaId, Igv_Porcentaje AS Porcentaje, CAST(CASE WHEN Igv_PorDefecto = 'S' THEN 1 ELSE 0 END AS BIT) AS 'Default' FROM Empresa_Igv";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oConfiguracionEmpresaIGV>(query);
            }
        }
        #endregion
    }
}
