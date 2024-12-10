using BarcoAzul.Api.Modelos.Entidades;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dZona : dComun
    {
        public dZona(string connectionString) : base(connectionString) { }

        public async Task<oZona> GetPorId(string id)
        {
            string query = @"SELECT Zon_Codigo AS Id, RTRIM(Zon_Nombre) AS Nombre FROM Zona WHERE Zon_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oZona>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 3 } });
            }
        }

        public async Task<IEnumerable<oZona>> ListarTodos()
        {
            string query = @"SELECT Zon_Codigo AS Id, RTRIM(Zon_Nombre) AS Nombre FROM Zona";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oZona>(query);
            }
        }
    }
}
