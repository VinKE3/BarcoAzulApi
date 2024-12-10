using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dTipoDocumentoIdentidad : dComun
    {
        public dTipoDocumentoIdentidad(string connectionString) : base(connectionString) { }

        public async Task<oTipoDocumentoIdentidad> GetPorId(string id)
        {
            string query = @"SELECT Codigo AS Id, Descripcion, Abreviatura FROM Cliente_TipoDoc WHERE Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oTipoDocumentoIdentidad>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 } });
            }
        }

        public async Task<IEnumerable<oTipoDocumentoIdentidad>> ListarTodos()
        {
            string query = @"SELECT Codigo AS Id, Descripcion, Abreviatura FROM Cliente_TipoDoc";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oTipoDocumentoIdentidad>(query);
            }
        }
    }
}
