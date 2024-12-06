using BarcoAzul.Api.Utilidades.Extensiones;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitDocumentoVentaId
    {
        private readonly string _empresaId;
        private readonly string _tipoDocumentoId;
        private readonly string _serie;
        private readonly string _numero;

        public oSplitDocumentoVentaId(string id)
        {
            if (id is null || id.Length != 18)
                throw new Exception("El ID no cumple con el formato.");

            _empresaId = id.Mid(0, 2);
            _tipoDocumentoId = id.Mid(2, 2);
            _serie = id.Mid(4, 4);
            _numero = id.Mid(8, 10);
        }

        public string EmpresaId => _empresaId;
        public string TipoDocumentoId => _tipoDocumentoId;
        public string Serie => _serie;
        public string Numero => _numero;
    }
}
