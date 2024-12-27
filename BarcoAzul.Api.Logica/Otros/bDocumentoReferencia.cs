using BarcoAzul.Api.Modelos.Interfaces;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Repositorio.Otros;

namespace BarcoAzul.Api.Logica.Otros
{
    public class bDocumentoReferencia : bComun
    {
        public bDocumentoReferencia(IConnectionManager connectionManager)
            : base(connectionManager, origen: "Documento de Referencia") { }

        public async Task<IEnumerable<oDocumentoReferenciaVenta>> ListarPorCliente(string clienteId) => await new dDocumentoReferencia(GetConnectionString()).ListarPorCliente(clienteId);

        public async Task<IEnumerable<oDocumentoReferenciaCompra>> ListarPorProveedor(string proveedorId) => await new dDocumentoReferencia(GetConnectionString()).ListarPorProveedor(proveedorId);
    }
}
