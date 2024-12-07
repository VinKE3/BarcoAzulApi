using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Modelos.Vistas;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dEntidadBancaria : dComun
    {
        public dEntidadBancaria(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task<int> Registrar(oEntidadBancaria entidadBancaria)
        {
            string query = @"INSERT INTO Entidad_Bancaria (Ban_Nombre, Ban_Tipo, Ban_Ruc) OUTPUT INSERTED.Ban_Codigo VALUES (@Nombre, @Tipo, @NumeroDocumentoIdentidad)";

            using (var db = GetConnection())
            {
                return await db.ExecuteScalarAsync<int>(query, entidadBancaria);
            }
        }

        public async Task Modificar(oEntidadBancaria entidadBancaria)
        {
            string query = @"UPDATE Entidad_Bancaria SET Ban_Nombre = @Nombre, Ban_Tipo = @Tipo, Ban_Ruc = @NumeroDocumentoIdentidad WHERE Ban_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, entidadBancaria);
            }
        }

        public async Task Eliminar(int id)
        {
            string query = "DELETE Entidad_Bancaria WHERE Ban_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oEntidadBancaria> GetPorId(int id)
        {
            string query = @"   SELECT
	                                Ban_Codigo AS Id,
	                                Ban_Nombre AS Nombre,
	                                Ban_Tipo AS Tipo,
	                                Ban_Ruc AS NumeroDocumentoIdentidad
                                 FROM 
                                    Entidad_Bancaria
                                 WHERE 
                                    Ban_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oEntidadBancaria>(query, new { id });
            }
        }

        public async Task<IEnumerable<vEntidadBancaria>> ListarTodos()
        {
            string query = @"   SELECT
	                                Ban_Codigo AS Id,
	                                Ban_Nombre AS Nombre,
	                                Ban_Tipo AS Tipo,
	                                Ban_Ruc AS NumeroDocumentoIdentidad
                                 FROM 
                                    Entidad_Bancaria
                                ORDER BY
                                    Ban_Nombre";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<vEntidadBancaria>(query);
            }
        }

        public async Task<oPagina<vEntidadBancaria>> Listar(string nombre, oPaginacion paginacion)
        {
            string query = @$"  SELECT
                                    Ban_Codigo AS Id,
	                                Ban_Nombre AS Nombre,
	                                Ban_Tipo AS Tipo,
	                                Ban_Ruc AS NumeroDocumentoIdentidad
                                FROM 
                                    Entidad_Bancaria
                                WHERE 
                                    Ban_Nombre LIKE '%' + @nombre + '%'
                                ORDER BY
                                    Ban_Nombre
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<vEntidadBancaria> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<vEntidadBancaria>
                    {
                        Data = await result.ReadAsync<vEntidadBancaria>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(int id)
        {
            string query = @"SELECT COUNT(Ban_Codigo) FROM Entidad_Bancaria WHERE Ban_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(int? id, string nombre)
        {
            string query = $@"SELECT COUNT(Ban_Codigo) FROM Entidad_Bancaria WHERE {(id is null ? string.Empty : "Ban_Codigo <> @id AND")} Ban_Nombre = @nombre";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id,
                    nombre = new DbString { Value = nombre, IsAnsi = true, IsFixedLength = false, Length = 60 }
                });
                return existe > 0;
            }
        }
        #endregion
    }
}
