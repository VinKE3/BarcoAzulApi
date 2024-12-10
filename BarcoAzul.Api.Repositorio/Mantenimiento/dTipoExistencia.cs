using BarcoAzul.Api.Modelos.Entidades;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dTipoExistencia : dComun
    {
        public dTipoExistencia(string connectionString) : base(connectionString) { }

        public async Task<oTipoExistencia> GetPorId(string id)
        {
            string query = "SELECT TipE_Codigo AS Id, TipE_Nombre AS Descripcion FROM Tipo_Existencia WHERE TipE_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oTipoExistencia>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 } });
            }
        }

        public async Task<IEnumerable<oTipoExistencia>> ListarTodos()
        {
            string query = "SELECT TipE_Codigo AS Id, TipE_Nombre AS Descripcion FROM Tipo_Existencia";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oTipoExistencia>(query);
            }
        }
    }
}
