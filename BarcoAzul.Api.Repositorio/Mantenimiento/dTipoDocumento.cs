using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dTipoDocumento : dComun
    {
        public dTipoDocumento(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oTipoDocumento>> Listar(string[] tiposDocumento = null)
        {
            string query = "SELECT TDoc_Codigo AS Id, TDoc_Nombre AS Descripcion, TDoc_Abreviatura AS Abreviatura FROM Tipo_Documento";

            if (tiposDocumento != null && tiposDocumento.Length > 0)
                query += $" WHERE TDoc_Codigo IN ({JoinToQuery(tiposDocumento)})";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oTipoDocumento>(query);
            }
        }

        public async Task<oTipoDocumento> GetPorId(string id)
        {
            string query = "SELECT TDoc_Codigo AS Id, TDoc_Nombre AS Descripcion, TDoc_Abreviatura AS Abreviatura FROM Tipo_Documento WHERE TDoc_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oTipoDocumento>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 2 } });
            }
        }
    }
}
