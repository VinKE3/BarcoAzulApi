using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dLugarEntrega : dComun
    {
        public dLugarEntrega(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oLugarEntrega>> ListarTodos()
        {
            string query = "SELECT Direccion, IsActivo FROM LugaresEntrega";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oLugarEntrega>(query);
            }
        }
    }
}
