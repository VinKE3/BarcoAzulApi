using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using BarcoAzul.Api.Modelos.Otros.Informes;
using Dapper;
using System.Data;

namespace BarcoAzul.Api.Repositorio.Informes.Finanzas
{
    public class dReporteCuentaBancaria : dComun
    {
        public dReporteCuentaBancaria(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oRegistroCuentaBancaria>> GetRegistros(oParamReporteCuentaBancaria parametros)
        {
            using (var db = GetConnection())
            {
                return await db.QueryAsync<oRegistroCuentaBancaria>("Sp_Rpt_MovCtaCte_V2", new
                {
                    CC = new DbString { Value = parametros.CuentaCorrienteId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    Inicio = parametros.FechaInicio,
                    Final = parametros.FechaFin
                }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            }
        }
    }
}
