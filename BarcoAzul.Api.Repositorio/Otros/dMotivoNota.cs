using BarcoAzul.Api.Modelos.Entidades;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dMotivoNota : dComun
    {
        public dMotivoNota(string connectionString) : base(connectionString) { }

        public async Task<oMotivoNota> GetPorId(string tipoDocumentoId, string id)
        {
            string query = @"   SELECT
	                                Mot_Codigo AS Id,
	                                TDoc_Codigo AS TipoDocumentoId,
	                                Mot_Nombre AS Descripcion
                                FROM 
                                    Motivo_NotaCredito
                                WHERE 
                                    TDoc_Codigo = @tipoDocumentoId 
                                    AND Mot_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oMotivoNota>(query, new
                {
                    tipoDocumentoId = new DbString { Value = tipoDocumentoId, IsAnsi = true, IsFixedLength = false, Length = 2 },
                    id = new DbString { Value = id, IsAnsi = true, IsFixedLength = false, Length = 2 }
                });
            }
        }

        public async Task<IEnumerable<oMotivoNota>> ListarTodos()
        {
            string query = @"   SELECT
	                                Mot_Codigo AS Id,
	                                TDoc_Codigo AS TipoDocumentoId,
	                                Mot_Nombre AS Descripcion
                                FROM 
                                    Motivo_NotaCredito";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oMotivoNota>(query);
            }
        }
    }
}
