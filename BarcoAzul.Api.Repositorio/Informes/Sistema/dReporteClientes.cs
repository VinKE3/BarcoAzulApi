using BarcoAzul.Api.Modelos.Otros.Informes;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Informes.Sistema
{
    public class dReporteClientes : dComun
    {
        public dReporteClientes(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oRegistroCliente>> GetRegistros()
        {
            string query = @"   SELECT
	                                Ruc AS NumeroDocumentoIdentidad,
	                                Razon_Social AS Nombre,
	                                Direccion,
	                                Telefono
                                FROM
	                                v_lst_cliente
                                WHERE
	                                Razon_Social <> ''
                                ORDER BY
	                                Razon_Social";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oRegistroCliente>(query);
            }
        }
    }
}
