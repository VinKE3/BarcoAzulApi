using BarcoAzul.Api.Utilidades.Extensiones;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitDistritoId
    {
        private readonly string _departamentoId;
        private readonly string _provinciaId;
        private readonly string _distritoId;

        public oSplitDistritoId(string id)
        {
            if (id is null || id.Length != 6)
                throw new Exception("El ID no cumple con el formato.");

            _departamentoId = id.Left(2);
            _provinciaId = id.Mid(2, 2);
            _distritoId = id.Right(2);
        }

        public string DepartamentoId { get => _departamentoId; }
        public string ProvinciaId { get => _provinciaId; }
        public string DistritoId { get => _distritoId; }
    }
}
