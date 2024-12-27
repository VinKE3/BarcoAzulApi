using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Otros
{
    public class dDocumentoReferencia : dComun
    {
        public dDocumentoReferencia(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oDocumentoReferenciaVenta>> ListarPorCliente(string clienteId)
        {
            string query = $@"  SELECT 
	                                Conf_Codigo + TDoc_Codigo + Ven_Serie + Ven_Numero AS Id,
	                                Ven_Fecha AS FechaEmision
                                FROM
                                    Venta
                                WHERE 
	                                TDoc_Codigo IN ('01', '03', '08') 
                                    AND Ven_Anulado = 'N' 
                                    AND Cli_Codigo = @clienteId
                                ORDER BY
                                    FechaEmision DESC,
                                    Id DESC";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oDocumentoReferenciaVenta>(query, new { clienteId = new DbString { Value = clienteId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }

        public async Task<IEnumerable<oDocumentoReferenciaCompra>> ListarPorProveedor(string proveedorId)
        {
            string query = $@"   SELECT 
                                    Conf_Codigo + Prov_Codigo + TDoc_Codigo + Com_Serie + Com_Numero + Cli_Codigo AS Id,
	                                Com_Fecha AS FechaEmision,
	                                Com_FechaContable AS FechaContable
                                FROM 
	                                Compra
                                WHERE 
                                    TDoc_Codigo IN ('01', '02', '03', '04', '08', '14')
	                                AND Prov_Codigo = @proveedorId
                                ORDER BY
                                    FechaContable DESC,
                                    Id DESC";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oDocumentoReferenciaCompra>(query, new { proveedorId = new DbString { Value = proveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 } });
            }
        }
    }
}
