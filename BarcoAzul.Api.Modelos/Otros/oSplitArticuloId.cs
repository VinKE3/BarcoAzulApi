using BarcoAzul.Api.Utilidades.Extensiones;


namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitArticuloId
    {
        private readonly string _lineaId;
        private readonly string _subLineaId;
        private readonly string _articuloId;

        public oSplitArticuloId(string id)
        {
            if (id is null || id.Length != 8)
                throw new Exception("El ID no cumple con el formato.");

            _lineaId = id.Mid(0, 2);
            _subLineaId = id.Mid(2, 2);
            _articuloId = id.Mid(4, 4);
        }

        public string LineaId => _lineaId;
        public string SubLineaId => _subLineaId;
        public string ArticuloId => _articuloId;
    }
}
