using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Empresa
{
    public class dMenu : dComun
    {
        public dMenu(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oMenu> menus)
        {
            string query = "INSERT INTO Menu_Web (Menu_Codigo, Menu_Nombre, SistemaAreaId, IsActivo) VALUES (@Id, @Nombre, @SistemaAreaId, @IsActivo)";

            using (var db = GetConnection())
            {
                foreach (var menu in menus)
                {
                    await db.ExecuteAsync(query, menu);
                }

            }
        }
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oMenu>> Listar()
        {
            string query = @"   SELECT
	                                RTRIM(MW.Menu_Codigo) AS Id,
	                                RTRIM(MW.Menu_Nombre) AS Nombre,
	                                MW.IsActivo,
	                                MW.SistemaAreaId,
	                                SA.Nombre AS SistemaAreaNombre
                                FROM
	                                Menu_Web MW
	                                INNER JOIN SistemaAreas SA ON MW.SistemaAreaId = SA.Id
                                ORDER BY
	                                SA.Orden";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oMenu>(query);
            }
        }

        public async Task<bool> Existe(string id)
        {
            string query = "SELECT COUNT(Menu_Codigo) FROM Menu_Web WHERE Menu_Codigo = @id";

            using (var db = GetConnection())
            {
                var existe = await db.QueryFirstAsync<int>(query, new { id = new DbString { Value = id, IsAnsi = true, IsFixedLength = true, Length = 40 } });
                return existe > 0;
            }
        }
        #endregion
    }
}
