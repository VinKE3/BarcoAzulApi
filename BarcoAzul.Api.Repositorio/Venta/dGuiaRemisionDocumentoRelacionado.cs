using BarcoAzul.Api.Modelos.Entidades;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Venta
{
    public class dGuiaRemisionDocumentoRelacionado : dComun
    {
        public dGuiaRemisionDocumentoRelacionado(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oGuiaRemisionDocumentoRelacionado> documentosRelacionados)
        {
            string query1 = @"  INSERT INTO Venta_Guia (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero, CodPedido, NumDocVenta, FechaGuia)
                                VALUES (@EmpresaId, @TipoDocumentoId, @Serie, @Numero, @Id, @NumeroDocumento, @FechaEmision)";

            string query2 = @"  UPDATE Venta SET Ven_GuiaRemision = @guiaRelacionada WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Ven_Serie = @serie AND Ven_Numero = @numero";

            var splitId = dGuiaRemision.SplitId(documentosRelacionados.First().GuiaRemisionId);

            using (var db = GetConnection())
            {
                foreach (var documentoRelacionado in documentosRelacionados)
                {
                    await db.ExecuteAsync(query1, documentoRelacionado);

                    var splitDocumentoRelacionadoId = dDocumentoVenta.SplitId(documentoRelacionado.Id);

                    await db.ExecuteAsync(query2, new
                    {
                        guiaRelacionada = new DbString { Value = $"{splitId.Serie}-{int.Parse(splitId.Numero)}", IsAnsi = true, IsFixedLength = false, Length = 50 },
                        empresaId = new DbString { Value = splitDocumentoRelacionadoId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        tipoDocumentoId = new DbString { Value = splitDocumentoRelacionadoId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitDocumentoRelacionadoId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitDocumentoRelacionadoId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                    });
                }
            }
        }

        public async Task Modificar(string guiaRemisionId, IEnumerable<oGuiaRemisionDocumentoRelacionado> documentosRelacionados)
        {
            await EliminarDeGuiaRemision(guiaRemisionId);

            if (documentosRelacionados is not null && documentosRelacionados.Any())
                await Registrar(documentosRelacionados);
        }

        public async Task EliminarDeGuiaRemision(string guiaRemisionId)
        {
            var documentosRelacionados = await ListarPorGuiaRemision(guiaRemisionId);

            string query1 = "DELETE Venta_Guia WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";
            string query2 = "UPDATE Venta SET Ven_GuiaRemision = '' WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                foreach (var documentoRelacionado in documentosRelacionados)
                {
                    var splitDocumentoRelacionadoId = dDocumentoVenta.SplitId(documentoRelacionado.Id);

                    await db.ExecuteAsync(query2, new
                    {
                        empresaId = new DbString { Value = splitDocumentoRelacionadoId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        tipoDocumentoId = new DbString { Value = splitDocumentoRelacionadoId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitDocumentoRelacionadoId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitDocumentoRelacionadoId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                    });
                }

                var splitId = dGuiaRemision.SplitId(guiaRemisionId);

                await db.ExecuteAsync(query1, new
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
        public async Task<IEnumerable<oGuiaRemisionDocumentoRelacionado>> ListarPorGuiaRemision(string guiaRemisionId)
        {
            var splitId = dGuiaRemision.SplitId(guiaRemisionId);

            string query = @"   SELECT CodPedido AS Id, NumDocVenta AS NumeroDocumento FROM Venta_Guia WHERE Conf_Codigo = @empresaId AND TDoc_Codigo = @tipoDocumentoId 
                                AND Ven_Serie = @serie AND Ven_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oGuiaRemisionDocumentoRelacionado>(query, new
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
