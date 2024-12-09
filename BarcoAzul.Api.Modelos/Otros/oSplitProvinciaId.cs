using BarcoAzul.Api.Utilidades.Extensiones;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitProvinciaId
    {
        private readonly string _departamentoId;
        private readonly string _provinciaId;

        public oSplitProvinciaId(string id)
        {
            if (id is null || id.Length != 4)
                throw new Exception("El ID no cumple con el formato.");

            _departamentoId = id.Left(2);
            _provinciaId = id.Right(2);
        }

        public string DepartamentoId { get => _departamentoId; }
        public string ProvinciaId { get => _provinciaId; }
    }
}
