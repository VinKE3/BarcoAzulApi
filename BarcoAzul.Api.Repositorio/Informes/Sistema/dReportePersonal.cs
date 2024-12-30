using BarcoAzul.Api.Modelos.Otros.Informes;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Informes.Sistema
{
    public class dReportePersonal : dComun
    {
        public dReportePersonal(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oRegistroPersonal>> GetRegistros()
        {
            string query = @"   SELECT  
	                                Codigo AS Id,
	                                Apellidos_Nombres AS NombreCompleto,
	                                DNI AS NumeroDocumentoIdentidad,
	                                Estado_Civil AS EstadoCivilDescripcion,
	                                Nacimiento AS FechaNacimiento,
	                                Cargo AS CargoDescripcion,
	                                CAST(CASE WHEN Activo = 'S' THEN 1 ELSE 0 END AS BIT) AS IsActivo
                                FROM
	                                v_lst_personal
                                ORDER BY
                                    Codigo";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oRegistroPersonal>(query);
            }
        }
    }
}
