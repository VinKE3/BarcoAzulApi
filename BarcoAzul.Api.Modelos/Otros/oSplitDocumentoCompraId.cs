using BarcoAzul.Api.Utilidades.Extensiones;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitDocumentoCompraId
    {
        private readonly string _empresaId;
        private readonly string _proveedorId;
        private readonly string _tipoDocumentoId;
        private readonly string _serie;
        private readonly string _numero;
        private readonly string _clienteId;

        public oSplitDocumentoCompraId(string id)
        {
            if (id is null || !(id.Length == 24 || id.Length == 30))
                throw new Exception("El ID no cumple con el formato.");

            _empresaId = id.Mid(0, 2);
            _proveedorId = id.Mid(2, 6);
            _tipoDocumentoId = id.Mid(8, 2);
            _serie = id.Mid(10, 4);
            _numero = id.Mid(14, 10);
            _clienteId = id.Mid(24, 6);
        }

        public string EmpresaId { get => _empresaId; }
        public string ProveedorId { get => _proveedorId; }
        public string TipoDocumentoId { get => _tipoDocumentoId; }
        public string Serie { get => _serie; }
        public string Numero { get => _numero; }
        public string ClienteId { get => _clienteId; }
    }
}
