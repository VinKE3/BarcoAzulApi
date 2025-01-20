using Dapper;
using BarcoAzul.Api.Modelos.Entidades;

namespace BarcoAzul.Api.Repositorio.Venta
{
    public class dGuiaRemisionVehiculo : dComun
    {
        public dGuiaRemisionVehiculo (string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oGuiaRemisionVehiculo> vehiculos)
        {
            string query = @"   INSERT INTO VentaVehiculo (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Ven_Item, Veh_Codigo, Placa)
                                VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @Item, @VehiculoId, @NumeroPlaca)";

            using (var db = GetConnection())
            {
                foreach (var vehiculo in vehiculos)
                {

                    await db.ExecuteAsync(query, new
                    {
                        vehiculo.EmpresaId,
                        vehiculo.TipoDocumentoId,
                        vehiculo.Serie,
                        vehiculo.Numero,
                        vehiculo.Item,
                        vehiculo.VehiculoId,
                        vehiculo.NumeroPlaca
                    });
                }
            }
        }
        public async Task Modificar(string GuiaRemisionId, IEnumerable<oGuiaRemisionVehiculo> detalles)
        {
            await EliminarDeGuiaRemision(GuiaRemisionId);

            if (detalles is not null && detalles.Any())
                await Registrar(detalles);
        }
        public async Task EliminarDeGuiaRemision(string GuiaRemisionId)
        {
            var splitId = dGuiaRemision.SplitId(GuiaRemisionId);
            string query = @"DELETE VentaVehiculo WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oGuiaRemisionVehiculo>> ListarPorGuiaRemision(string GuiaRemisionId)
        {
            var splitId = dGuiaRemision.SplitId(GuiaRemisionId);

            string query = @"   SELECT
                                    Ven_Item AS Item,
                                    Veh_Codigo AS VehiculoId,
                                    Placa AS NumeroPlaca
                                FROM 
                                    VentaVehiculo
                                WHERE 
                                    Conf_Codigo = @empresaId
									AND TDoc_Codigo = @tipoDocumentoId
									AND Ven_Serie = @serie
									AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oGuiaRemisionVehiculo>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }
        #endregion
    }
}
