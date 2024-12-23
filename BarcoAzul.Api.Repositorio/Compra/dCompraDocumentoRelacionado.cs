using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Modelos.Otros;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Compra
{
    public class dCompraDocumentoRelacionado : dComun
    {
        public dCompraDocumentoRelacionado(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oCompraDocumentoRelacionado> documentosRelacionados)
        {
            string query = @"   INSERT INTO Compra_Guia (Conf_Codigo, Prov_Codigo, TDoc_Codigo, Com_Serie, Com_Numero, CodGuia, NumDocCompra, FechaGuia)
                                VALUES (@EmpresaId, @ProveedorId, @TipoDocumentoId, @Serie, @Numero, @Id, @NumeroDocumento, @Fecha)";

            using (var db = GetConnection())
            {
                foreach (var documentoRelacionado in documentosRelacionados)
                {
                    await db.ExecuteAsync(query, documentoRelacionado);
                }
            }
        }

        public async Task Eliminar(string id)
        {
            var splitId = SplitId(id);
            string query = @"DELETE Compra_Guia WHERE Conf_Codigo = @empresaId AND Prov_Codigo = @proveedorId AND TDoc_Codigo = @tipoDocumentoId AND Com_Serie = @serie AND Com_Numero = @numero";

            using (var db = GetConnection())
            {
                await db.ExecuteAsync(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oCompraDocumentoRelacionado>> ListarPorCompra(string compraId)
        {
            var splitId = SplitId(compraId);

            string query = @"   SELECT
	                                Conf_Codigo AS EmpresaId,
	                                Prov_Codigo AS ProveedorId,
	                                TDoc_Codigo AS TipoDocumentoId,
	                                Com_Serie AS Serie,
	                                Com_Numero AS Numero,
	                                CodGuia AS Id,
	                                NumDocCompra AS NumeroDocumento,
	                                FechaGuia AS Fecha
                                FROM
	                                Compra_Guia
                                WHERE
                                    Conf_Codigo = @empresaId
                                    AND Prov_Codigo = @proveedorId
                                    AND TDoc_Codigo = @tipoDocumentoId
                                    AND Com_Serie = @serie
                                    AND Com_Numero = @numero";

            using (var db = GetConnection())
            {
                return await db.QueryAsync<oCompraDocumentoRelacionado>(query, new
                {
                    empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                    tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                    serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                    numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                });
            }
        }

        private static oSplitDocumentoCompraId SplitId(string id) => new(id);
        #endregion
    }
}
