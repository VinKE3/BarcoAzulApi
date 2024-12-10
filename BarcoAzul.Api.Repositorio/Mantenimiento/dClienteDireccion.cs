using BarcoAzul.Api.Modelos.Entidades;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dClienteDireccion : dComun
    {
        public dClienteDireccion(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task<int> Registrar(oClienteDireccion clienteDireccion)
        {
            string query = @"   INSERT INTO Cliente_Direccion (Cli_Codigo, Cli_Direccion, Dep_Codigo, Pro_Codigo, Dis_Codigo, Cli_Comentario, Cli_Activo, Cli_Tipo)
                                OUTPUT INSERTED.Id_columna
                                VALUES (@ClienteId, @Direccion, @DepartamentoId, @ProvinciaId, @DistritoId, @Comentario, @IsActivo, @TipoDireccionId)";

            using (var db = GetConnection())
            {
                return await db.ExecuteScalarAsync<int>(query, new
                {
                    clienteDireccion.ClienteId,
                    clienteDireccion.Direccion,
                    clienteDireccion.DepartamentoId,
                    clienteDireccion.ProvinciaId,
                    clienteDireccion.DistritoId,
                    clienteDireccion.Comentario,
                    IsActivo = clienteDireccion.IsActivo ? "S" : "N",
                    clienteDireccion.TipoDireccionId
                });
            }
        }

        public async Task Modificar(oClienteDireccion clienteDireccion)
        {
            string query = @"UPDATE Cliente_Direccion SET Cli_Direccion = @Direccion, Dep_Codigo = @DepartamentoId, Pro_Codigo = @ProvinciaId, 
                             Dis_Codigo = @DistritoId, Cli_Comentario = @Comentario, Cli_Activo = @IsActivo WHERE Id_Columna = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    clienteDireccion.Direccion,
                    clienteDireccion.DepartamentoId,
                    clienteDireccion.ProvinciaId,
                    clienteDireccion.DistritoId,
                    clienteDireccion.Comentario,
                    IsActivo = clienteDireccion.IsActivo ? "S" : "N",
                    clienteDireccion.Id
                });
            }
        }

        public async Task Eliminar(int id)
        {
            string query = "DELETE Cliente_Direccion WHERE Id_Columna = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id });
            }
        }

        public async Task EliminarDeCliente(string clienteId)
        {
            string query = "DELETE Cliente_Direccion WHERE Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oClienteDireccion> GetPorId(int id)
        {
            string query = @"   SELECT
	                                Id_columna AS Id,
	                                Cli_Codigo AS ClienteId,
	                                RTRIM(Cli_Direccion) AS Direccion,
	                                Dep_Codigo AS DepartamentoId,
	                                Pro_Codigo AS ProvinciaId,
	                                Dis_Codigo AS DistritoId,
	                                Cli_Comentario AS Comentario,
	                                CAST(CASE WHEN Cli_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo,
                                    Cli_Tipo AS TipoDireccionId
                                FROM 
                                    Cliente_Direccion
                                WHERE 
                                    Id_columna = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oClienteDireccion>(query, new { id });
            }
        }

        public async Task<IEnumerable<oClienteDireccion>> ListarPorCliente(string clienteId)
        {
            string query = @"   SELECT
	                                Id_columna AS Id,
	                                Cli_Codigo AS ClienteId,
	                                RTRIM(Cli_Direccion) AS Direccion,
	                                Dep_Codigo AS DepartamentoId,
	                                Pro_Codigo AS ProvinciaId,
	                                Dis_Codigo AS DistritoId,
	                                Cli_Comentario AS Comentario,
	                                CAST(CASE WHEN Cli_Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo,
                                    Cli_Tipo AS TipoDireccionId
                                FROM 
                                    Cliente_Direccion
                                WHERE 
                                    Cli_Codigo = @clienteId";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oClienteDireccion>(query, new { clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }

        public async Task<bool> Existe(int id)
        {
            string query = "SELECT COUNT(Id_Columna) FROM Cliente_Direccion WHERE Id_Columna = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id });
                return existe > 0;
            }
        }

        public async Task<int?> GetDireccionPrincipalId(string clienteId)
        {
            string query = "SELECT Id_Columna FROM Cliente_Direccion WHERE Cli_Codigo = @clienteId AND Cli_Tipo = '01'";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<int?>(query, new { clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }
        #endregion
    }
}
