using BarcoAzul.Api.Modelos.Otros.Informes;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Informes.Sistema
{
    public class dReportePersonalCliente : dComun
    {
        public dReportePersonalCliente(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oRegistroPersonalCliente>> GetRegistros(string personalId = "")
        {
            string query = @$"	SELECT
									CP.Per_Codigo AS PersonalId,
									CP.Per_ApeNombres AS PersonalNombreCompleto,
									C.Cli_Ruc AS ClienteNumeroDocumentoIdentidad,
									CP.Cli_RazonSocial AS ClienteNombre,
									C.Cli_Direccion AS ClienteDireccion,
									C.Cli_Telefono AS ClienteTelefono,
									C.Cli_Correo AS ClienteCorreoElectronico
								FROM
									Cliente_Personal CP
									INNER JOIN Cliente C ON C.Cli_Codigo = CP.Cli_Codigo
								WHERE
									CP.DVend_PorDefecto = 'S'
									{(string.IsNullOrWhiteSpace(personalId) ? string.Empty : "AND CP.Per_Codigo = @personalId")}
								ORDER BY
									CP.Per_Codigo DESC,
									C.Cli_RazonSocial";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oRegistroPersonalCliente>(query);
            }
        }
    }
}
