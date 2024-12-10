using Dapper;

namespace BarcoAzul.Api.Repositorio.Configuracion
{
    public class dOrigen : dComun
    {
        public dOrigen(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<string>> Listar()
        {
            string query = "SELECT Url FROM Origenes WHERE IsActivo = 1";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<string>(query);
            }
        }
    }
}
