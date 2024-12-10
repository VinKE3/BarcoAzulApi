using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Mantenimiento
{
    public class dCargo : dComun
    {
        public dCargo(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task<int> Registrar(oCargo cargo)
        {
            string query = "INSERT INTO Cargo (Car_Nombre, Car_Sueldo) OUTPUT INSERTED.Car_Codigo VALUES (@Descripcion, @Sueldo)";

            using (var db = GetConnection())
            {
                return await db.ExecuteScalarAsync<int>(query, cargo);
            }
        }

        public async Task Modificar(oCargo cargo)
        {
            string query = @"UPDATE Cargo SET Car_Nombre = @Descripcion, Car_Sueldo = @Sueldo WHERE Car_Codigo = @Id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, cargo);
            }
        }

        public async Task Eliminar(int id)
        {
            string query = "DELETE Cargo WHERE Car_Codigo = @id";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new { id });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<oCargo> GetPorId(int id)
        {
            string query = @"   SELECT
                                    Car_Codigo AS Id,
                                    Car_Nombre AS Descripcion,
                                    Car_Sueldo AS Sueldo
                                FROM
                                    Cargo
                                WHERE
                                    Car_Codigo = @id";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oCargo>(query, new { id });
            }
        }

        public async Task<IEnumerable<oCargo>> ListarTodos()
        {
            string query = @"   SELECT 
                                    Car_Codigo AS Id,
                                    Car_Nombre AS Descripcion,
                                    Car_Sueldo AS Sueldo
                                FROM 
                                    Cargo
                                ORDER BY
                                    Car_Nombre";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oCargo>(query);
            }
        }

        public async Task<oPagina<oCargo>> Listar(string descripcion, oPaginacion paginacion)
        {
            string query = $@"   SELECT 
                                    Car_Codigo AS Id,
                                    Car_Nombre AS Descripcion,
                                    Car_Sueldo AS Sueldo
                                FROM 
                                    Cargo
                                WHERE 
                                    Car_Nombre LIKE '%' + @descripcion + '%'
                                ORDER BY
                                    Car_Nombre
                                {GetPaginacionQuery(paginacion)}";

            query += GetCountQuery(query);

            oPagina<oCargo> pagina;

            using (var db = GetConnection())
            {
                using (var result = await db.QueryMultipleAsync(query, new { descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 60 } }))
                {
                    pagina = new oPagina<oCargo>
                    {
                        Data = await result.ReadAsync<oCargo>(),
                        Total = result.ReadFirst<int>()
                    };
                }
            }

            return pagina;
        }

        public async Task<bool> Existe(int id)
        {
            string query = @"SELECT COUNT(Car_Codigo) FROM Cargo WHERE Car_Codigo = @id";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new { id });
                return existe > 0;
            }
        }

        public async Task<bool> DatosRepetidos(int? id, string descripcion)
        {
            string query = @$"SELECT COUNT(Car_Codigo) FROM Cargo WHERE {(id is null ? string.Empty : "Car_Codigo <> @id AND")} Car_Nombre = @descripcion";

            using (var db = GetConnection())
            {
                int existe = await db.QueryFirstAsync<int>(query, new
                {
                    id,
                    descripcion = new DbString { Value = descripcion, IsAnsi = true, IsFixedLength = false, Length = 60 }
                });
                return existe > 0;
            }
        }
        #endregion
    }
}
