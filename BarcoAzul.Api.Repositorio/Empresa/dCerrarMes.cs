using BarcoAzul.Api.Modelos.Entidades;
using Dapper;
using System.Data;

namespace BarcoAzul.Api.Repositorio.Empresa
{
    public class dCerrarMes : dComun
    {
        public dCerrarMes(string connectionString) : base(connectionString) { }

        #region CRUD
        #endregion

        #region Otros Métodos
        public async Task<oCerrarMes> GetPorAnioMes(int anio, int mes)
        {
            string query = @"   SELECT
	                                Cie_Anio AS Anio,
	                                Cie_Mes AS MesNumero,
	                                CAST(CASE WHEN Cie_Estado = 'C' THEN 1 ELSE 0 END AS BIT) AS IsCerrado
                                FROM
	                                Cierre_Mes
                                WHERE
	                                Cie_Anio = @anio 
                                    AND Cie_Mes = @mes";

            using (var db = GetConnection())
            {
                return await db.QueryFirstOrDefaultAsync<oCerrarMes>(query, new { anio, mes });
            }
        }

        public async Task<(bool Valido, string Mensaje)> VerificarPeriodoCerrado(DateTime fecha)
        {
            using (var db = GetConnection())
            {
                var parametros = new DynamicParameters();
                parametros.Add("@adt_fecha", fecha);
                parametros.Add("@mensaje", dbType: DbType.String, size: 255, direction: ParameterDirection.Output);

                var valido = await db.QueryFirstOrDefaultAsync<bool>(
                    "sp_verificar_periodo_cerrado",
                    parametros,
                    commandType: CommandType.StoredProcedure
                );

                // Obtener el valor del parámetro de salida
                string mensaje = parametros.Get<string>("@mensaje");

                return (valido, mensaje);
            }
        }

        #endregion
    }
}
