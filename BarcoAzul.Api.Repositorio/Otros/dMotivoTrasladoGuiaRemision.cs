using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dMotivoTrasladoGuiaRemision : dComun
    {
        public dMotivoTrasladoGuiaRemision(string connectionString) : base(connectionString) { }

        public async Task<oMotivoTrasladoGuiaRemision> GetPorId(string id)
        {
            string query = @"   SELECT
	                                codigo AS Id,
	                                descripcion AS Descripcion,
	                                ctrlstock AS Tipo
                                FROM
	                                Tipo_Operacion
	                            WHERE
	                                codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oMotivoTrasladoGuiaRemision>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 } });
            }
        }

        public async Task<IEnumerable<oMotivoTrasladoGuiaRemision>> ListarTodos()
        {
            string query = @"   SELECT
	                                codigo AS Id,
	                                descripcion AS Descripcion,
	                                ctrlstock AS Tipo
                                FROM
	                                motivo_traslado";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oMotivoTrasladoGuiaRemision>(query);
            }
        }
    }
}
