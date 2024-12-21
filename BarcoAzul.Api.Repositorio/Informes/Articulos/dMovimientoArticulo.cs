using BarcoAzul.Api.Modelos.Otros.Informes;
using BarcoAzul.Api.Modelos.Otros.Informes.Parametros;
using Dapper;
using System.Data;


namespace BarcoAzul.Api.Repositorio.Informes.Articulos
{
    public class dMovimientoArticulo : dComun
    {
        public dMovimientoArticulo(string connectionString) : base(connectionString) { }

        public async Task<IEnumerable<oMovimientoArticulo>> GetRegistros(oParamMovimientoArticulo parametros)
            => await Listar(parametros.FechaInicio.Value, parametros.FechaFin.Value, parametros.EstadoStock);

        public async Task<IEnumerable<oMovimientoArticulo>> Listar(DateTime fechaInicio, DateTime fechaFin, string estadoStock)
        {
            using (var db = GetConnection())
            {
                return await db.QueryAsync<oMovimientoArticulo>("SP_MovArticulo_V2", new
                {
                    Inicio = fechaInicio,
                    Final = fechaFin,
                    EstadoStock = estadoStock
                }, commandType: CommandType.StoredProcedure, commandTimeout: 600);
            }
        }

        public async Task<oKardexArticulo> GetKardexArticulo(string lineaId, string subLineaId, string articuloId, DateTime fechaInicio, DateTime fechaFin)
        {
            using (var db = GetConnection())
            {
                var detalles = await db.QueryAsync<oKardexArticuloDetalle>("SP_VentasPromedio_V2", new
                {
                    LineaId = new DbString { Value = lineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    SubLineaId = new DbString { Value = subLineaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    ArticuloId = new DbString { Value = articuloId, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    FechaInicio = fechaInicio,
                    FechaFin = fechaFin
                }, commandType: CommandType.StoredProcedure, commandTimeout: 600);

                return new oKardexArticulo(detalles);
            }
        }
    }
}
