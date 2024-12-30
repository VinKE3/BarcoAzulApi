using BarcoAzul.Api.Modelos.Otros.Informes;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Informes.Sistema
{
    public class dReporteProveedores : dComun
    {
        public dReporteProveedores(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oRegistroProveedor>> GetRegistros()
        {
            string query = @"   SELECT
	                                Ruc AS NumeroDocumentoIdentidad,
	                                Razon_Social AS Nombre,
	                                Direccion,
	                                Telefono
                                FROM
	                                v_lst_proveedor
                                ORDER BY
	                                Razon_Social";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oRegistroProveedor>(query);
            }
        }
    }
}
