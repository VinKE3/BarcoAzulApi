using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using Dapper;


namespace BarcoAzul.Api.Repositorio.Informes.Cobranzas
{
    public class dInformeCobranza : dComun
    {
        public dInformeCobranza(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oInformeCobranza>> GetRegistros(oParamInformeCobranza parametros)
        {
            string query = @$"  SELECT 
	                                Fecha AS FechaEmision,
	                                Vencimiento AS FechaVencimiento,
	                                TipoDoc AS TipoDocumentoId,
	                                TDoc_Nombre AS TipoDocumentoDescripcion,
	                                Documento AS NumeroDocumento,
	                                Razon_Social AS ClienteNombre,
	                                Moneda AS MonedaId,
	                                Total,
	                                Abonado AS Abono,
	                                Saldo,
	                                Personal AS PersonalNombreCompleto
                                FROM 
	                                v_lst_venta
                                WHERE 
	                                (Fecha BETWEEN @fechaInicio AND @fechaFin)
	                                AND TipoDoc IN ('01', '03', '07', '08')
	                                AND Cancelado LIKE @cancelado
	                                AND Moneda = @monedaId";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oInformeCobranza>(query, new
                {
                    monedaId = new DbString { Value = parametros.MonedaId, IsAnsi = true, IsFixedLength = true, Length = 1 },
                    cancelado = new DbString { Value = parametros.Cancelado, IsAnsi = true, IsFixedLength = true, Length = 1 },
                    fechaInicio = parametros.FechaInicio,
                    fechaFin = parametros.FechaFin
                });
            }
        }
    }
}
