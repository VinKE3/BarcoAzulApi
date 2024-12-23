using BarcoAzul.Api.Modelos.Entidades;
using BarcoAzul.Api.Utilidades.Extensiones;
using Dapper;

namespace BarcoAzul.Api.Repositorio.Compra
{
    public class dDocumentoCompraOrdenCompraRelacionada : dComun
    {
        public dDocumentoCompraOrdenCompraRelacionada(string connectionString) : base(connectionString) { }

        #region CRUD
        public async Task Registrar(IEnumerable<oDocumentoCompraOrdenCompraRelacionada> detalles)
        {
            var documentosRelacionados = detalles.Select(x => new oCompraDocumentoRelacionado
            {
                EmpresaId = x.EmpresaId,
                ProveedorId = x.ProveedorId,
                TipoDocumentoId = x.TipoDocumentoId,
                Serie = x.Serie,
                Numero = x.Numero,
                Id = x.Id,
                NumeroDocumento = x.NumeroDocumento
            }).ToList();

            var clienteId = detalles.First().ClienteId;

            using (var db = GetConnection())
            {
                string queryFecha = @"   SELECT Com_Fecha FROM Compra WHERE Conf_Codigo = @EmpresaId AND Prov_Codigo = @ProveedorId AND TDoc_Codigo = @TipoDocumentoId 
                                    AND Com_Serie = @Serie AND Com_Numero = @Numero";

                string queryActualizar = @" UPDATE Compra SET Com_NroComp = @numeroDocumento WHERE Conf_Codigo = @EmpresaId AND Prov_Codigo = @ProveedorId AND TDoc_Codigo = @TipoDocumentoId 
                                            AND Com_Serie = @Serie AND Com_Numero = @Numero";

                foreach (var documentoRelacionado in documentosRelacionados)
                {
                    var splitId = dOrdenCompra.SplitId(documentoRelacionado.Id);

                    await db.ExecuteAsync(queryActualizar, new
                    {
                        numeroDocumento = $"FT-{documentoRelacionado.Serie}-{documentoRelacionado.Numero.Right(8)}",
                        empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                        tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                    });

                    documentoRelacionado.Id = documentoRelacionado.Id.Left(24);
                    documentoRelacionado.Fecha = await db.QueryFirstAsync<DateTime>(queryFecha, new
                    {
                        empresaId = new DbString { Value = splitId.EmpresaId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        proveedorId = new DbString { Value = splitId.ProveedorId, IsAnsi = true, IsFixedLength = true, Length = 6 },
                        tipoDocumentoId = new DbString { Value = splitId.TipoDocumentoId, IsAnsi = true, IsFixedLength = true, Length = 2 },
                        serie = new DbString { Value = splitId.Serie, IsAnsi = true, IsFixedLength = true, Length = 4 },
                        numero = new DbString { Value = splitId.Numero, IsAnsi = true, IsFixedLength = true, Length = 10 }
                    });
                }
            }

            dCompraDocumentoRelacionado dCompraDocumentoRelacionado = new(_connectionString);
            await dCompraDocumentoRelacionado.Registrar(documentosRelacionados);
        }

        public async Task Modificar(string documentoCompraId, IEnumerable<oDocumentoCompraOrdenCompraRelacionada> detalles)
        {
            await EliminarDeDocumentoCompra(documentoCompraId);

            if (detalles.Any())
            {
                await Registrar(detalles);
            }
        }

        public async Task EliminarDeDocumentoCompra(string id) => await new dCompraDocumentoRelacionado(_connectionString).Eliminar(id);
        #endregion

        #region Otros Métodos
        public async Task<IEnumerable<oDocumentoCompraOrdenCompraRelacionada>> ListarPorDocumentoCompra(string documentoCompraId)
        {
            dCompraDocumentoRelacionado dCompraDocumentoRelacionado = new(_connectionString);
            var documentosRelacionados = await dCompraDocumentoRelacionado.ListarPorCompra(documentoCompraId);

            return documentosRelacionados.Select(x => new oDocumentoCompraOrdenCompraRelacionada
            {
                EmpresaId = x.EmpresaId,
                ProveedorId = x.ProveedorId,
                TipoDocumentoId = x.TipoDocumentoId,
                Serie = x.Serie,
                Numero = x.Numero,
                Id = x.Id,
                NumeroDocumento = x.NumeroDocumento,
                Fecha = x.Fecha
            });
        }
        #endregion
    }
}
