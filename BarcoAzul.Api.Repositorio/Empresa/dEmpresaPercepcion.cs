using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Empresa
{
    public class dEmpresaPercepcion : dComun
    {
        public dEmpresaPercepcion(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oConfiguracionEmpresaPercepcion> empresaPercepciones)
        {
            string query = "INSERT INTO Empresa_Percepcion (Conf_Codigo, Percep_Porcentaje, Percep_PorDefecto, Percep_Tipo) VALUES (@EmpresaId, @Porcentaje, @Default, @TipoPercepcion)";

            using (var db = GetConnection())
            {
                foreach (var empresaDetraccion in empresaPercepciones)
                {
                    await db.ExecuteAsync(query, new
                    {
                        empresaDetraccion.EmpresaId,
                        empresaDetraccion.Porcentaje,
                        Default = empresaDetraccion.Default ? "S" : "N",
                        empresaDetraccion.TipoPercepcion
                    });
                }
            }
        }

        public async Task Eliminar()
        {
            string query = "DELETE Empresa_Percepcion";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query);
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oConfiguracionEmpresaPercepcion>> ListarTodos()
        {
            string query = "SELECT Conf_Codigo AS EmpresaId, Percep_Porcentaje AS Porcentaje, Percep_Tipo AS TipoPercepcion, CAST(CASE WHEN Percep_PorDefecto = 'S' THEN 1 ELSE 0 END AS BIT) AS 'Default' FROM Empresa_Percepcion";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oConfiguracionEmpresaPercepcion>(query);
            }
        }
        #endregion
    }
}
