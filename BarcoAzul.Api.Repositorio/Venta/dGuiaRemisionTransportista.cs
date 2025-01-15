using Dapper;
using BarcoAzul.Api.Modelos.Entidades;

namespace BarcoAzul.Api.Repositorio.Venta
{
    public class dGuiaRemisionTransportista : dComun
    {
        public dGuiaRemisionTransportista(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oGuiaRemisionTransportista> transportistas)
        {
            string query = @"   INSERT INTO VentaTransportista (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, Ven_Item, Ven_Tipo, Tra_Codigo, tipodoc,
                                nrodocide, nombre, direccion, correo, apellidos, licencia, nroregistrotransp)
                                VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @Item, @TipoConductor , @TransportistaId, @TipoDocumentoIdentidadId,
                                @NumeroDocumentoIdentidad, @Nombre, @Direccion, @CorreoElectronico, @Apellidos, @LicenciaConducir, @NumeroRegistroMTC)";

            using (var db = GetConnection())
            {
                foreach (var transportista in transportistas)
                {

                    await db.ExecuteAsync(query, new
                    {
                        transportista.EmpresaId,
                        transportista.TipoDocumentoId,
                        transportista.Serie,
                        transportista.Numero,
                        transportista.Item,
                        transportista.TipoConductor,
                        transportista.TransportistaId,
                        transportista.TipoDocumentoIdentidadId,
                        transportista.NumeroDocumentoIdentidad,
                        transportista.Nombre,
                        transportista.Direccion,
                        transportista.CorreoElectronico,
                        transportista.Apellidos,
                        transportista.LicenciaConducir,
                        transportista.NumeroRegistroMTC
                    });
                }
            }
        }
        public async Task Modificar(string GuiaRemisionId, IEnumerable<oGuiaRemisionTransportista> detalles)
        {
            await EliminarDeGuiaRemision(GuiaRemisionId);

            if (detalles is not null && detalles.Any())
                await Registrar(detalles);
        }
        public async Task EliminarDeGuiaRemision(string GuiaRemisionId)
        {
            var splitId = dGuiaRemision.SplitId(GuiaRemisionId);
            string query = @"DELETE VentaTransportista WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

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
        public async Task<IEnumerable<oGuiaRemisionTransportista>> ListarPorGuiaRemision(string GuiaRemisionId)
        {
            var splitId = dGuiaRemision.SplitId(GuiaRemisionId);

            string query = @"   SELECT
                                    Ven_Item AS Item,
                                    Ven_Tipo AS TipoConductor ,
                                    Tra_Codigo AS TransportistaId,
                                    tipodoc AS TipoDocumentoIdentidadId ,
                                    nrodocide AS NumeroDocumentoIdentidad ,
                                    nombre AS Nombre ,
                                    direccion AS Direccion ,
                                    correo AS CorreoElectronico ,
                                    apellidos AS Apellidos ,
                                    licencia AS LicenciaConducir ,
                                    nroregistrotransp AS NumeroRegistroMTC
                                FROM 
                                    VentaTransportista
                                WHERE 
                                    Conf_Codigo = @empresaId
									AND TDoc_Codigo = @tipoDocumentoId
									AND Ven_Serie = @serie
									AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oGuiaRemisionTransportista>(query, new
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
