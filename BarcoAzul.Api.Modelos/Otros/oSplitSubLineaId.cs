using BarcoAzul.Api.Utilidades.Extensiones;


namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitSubLineaId
    {
        private readonly string _subLineaId;
        private readonly string _lineaId;

        public oSplitSubLineaId(string id)
        {
            if (id is null || id.Length != 4)
                throw new Exception("El ID no cumple con el formato.");

            _lineaId = id.Left(2);
            _subLineaId = id.Right(2);
        }

        public string SubLineaId { get => _subLineaId; }
        public string LineaId { get => _lineaId; }
    }
}
