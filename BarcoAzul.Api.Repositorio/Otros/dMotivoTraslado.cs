using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dMotivoTraslado : dComun
    {
        public dMotivoTraslado(string connectionString) : base(connectionString) { }

        public async Task<oMotivoTraslado> GetPorId(string id)
        {
            string query = @"   SELECT
	                                TipO_Codigo AS Id,
	                                TipO_Nombre AS Descripcion,
	                                CAST(CASE WHEN TipO_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo,
	                                TipO_Stock AS Tipo
                                FROM
	                                Tipo_Operacion
	                            WHERE
	                                TipO_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oMotivoTraslado>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 } });
            }
        }

        public async Task<IEnumerable<oMotivoTraslado>> ListarTodos()
        {
            string query = @"   SELECT
	                                TipO_Codigo AS Id,
	                                TipO_Nombre AS Descripcion,
	                                CAST(CASE WHEN TipO_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo,
	                                TipO_Stock AS Tipo
                                FROM
	                                Tipo_Operacion";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oMotivoTraslado>(query);
            }
        }
    }
}
