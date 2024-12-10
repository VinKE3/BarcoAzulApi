using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Empresa
{
    public class dEmpresaDetraccion : dComun
    {
        public dEmpresaDetraccion(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oConfiguracionEmpresaDetraccion> empresaDetracciones)
        {
            string query = "INSERT INTO Empresa_Detraccion (Conf_Codigo, Detr_Porcentaje, Detr_PorDefecto) VALUES (@EmpresaId, @Porcentaje, @Default)";

            using (var db = GetConnection())
            {
                foreach (var empresaDetraccion in empresaDetracciones)
                {
                    await db.ExecuteAsync(query, new
                    {
                        empresaDetraccion.EmpresaId,
                        empresaDetraccion.Porcentaje,
                        Default = empresaDetraccion.Default ? "S" : "N"
                    });
                }
            }
        }

        public async Task Eliminar()
        {
            string query = "DELETE Empresa_Detraccion";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query);
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oConfiguracionEmpresaDetraccion>> ListarTodos()
        {
            string query = "SELECT Conf_Codigo AS EmpresaId, Detr_Porcentaje AS Porcentaje, CAST(CASE WHEN Detr_PorDefecto = 'S' THEN 1 ELSE 0 END AS BIT) AS 'Default' FROM Empresa_Detraccion";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oConfiguracionEmpresaDetraccion>(query);
            }
        }
        #endregion
    }
}
