using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Informes.Ventas
{
    public class dRegistroVenta : dComun
    {
        public dRegistroVenta(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oRegistroVenta>> GetRegistros(oParamRegistroVenta parametros)
        {
            string query = @$"  SELECT 
	                                Fecha AS FechaEmision,
	                                Documento AS NumeroDocumento,
	                                Moneda AS MonedaId,
	                                (CASE WHEN Anulado = 'S' THEN 'A-N-U-L-A-D-O' ELSE Razon_Social END) AS ClienteNombre,
	                                (CASE WHEN TipoDoc = '07' THEN Total * (-1) ELSE Total END) AS Total,
	                                Ven_GuiaRemision AS GuiaRemision,
	                                Personal AS PersonalNombreCompleto,
	                                CAST(CASE WHEN Anulado = 'S' THEN 1 ELSE 0 END AS BIT) AS IsAnulado
                                FROM
	                                v_lst_venta
                                WHERE 
	                                Moneda = @monedaId
	                                AND (Fecha BETWEEN @fechaInicio AND @fechaFin)
	                                AND TipoDoc IN ('01', '03', '07', '08', '12', 'NV')";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oRegistroVenta>(query, new
                {
                    monedaId = new DbString { Value = parametros.MonedaId, IsAnsi = true, IsFixedLength = true, Length = 1 },
                    fechaInicio = parametros.FechaInicio,
                    fechaFin = parametros.FechaFin
                });
            }
        }
    }
}
