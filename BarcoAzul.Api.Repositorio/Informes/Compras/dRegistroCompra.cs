using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Informes.Compras
{
    public class dRegistroCompra : dComun
    {
        public dRegistroCompra(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oRegistroCompra>> GetRegistros(oParamRegistroCompra parametros)
        {
            string query = @$"	SELECT 
                                    TipoDoc AS TipoDocumentoId,
                                    Fecha AS FechaContable,
									Emision AS FechaEmision,
									Documento AS NumeroDocumento,
									Proveedor AS ProveedorNombre,
									Ruc AS ProveedorNumeroDocumentoIdentidad,
									Moneda AS MonedaId,
									Total
								FROM
									v_lst_compra
								WHERE
									Moneda = @monedaId
									AND (Emision BETWEEN @fechaInicio AND @fechaFin)
									AND TipoDoc IN ('04', '01', '03', '12', 'RC', '07', '08', 'NV', 'PR', 'CR', 'CV', 'QC')";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oRegistroCompra>(query, new
                {
                    monedaId = new DbString { Value = parametros.MonedaId, IsAnsi = true, IsFixedLength = true, Length = 1 },
                    fechaInicio = parametros.FechaInicio,
                    fechaFin = parametros.FechaFin
                });
            }
        }
    }
}
