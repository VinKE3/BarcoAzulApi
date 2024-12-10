using BarcoAzul.Api.Utilidades.Extensiones;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitClientePersonalId
    {
        private readonly string _clienteId;
        private readonly string _personalId;

        public oSplitClientePersonalId(string id)
        {
            if (id is null || id.Length != 14)
                throw new Exception("El ID no cumple con el formato.");

            _clienteId = id.Left(6);
            _personalId = id.Right(8);
        }

        public string ClienteId { get => _clienteId; }
        public string PersonalId { get => _personalId; }
    }
}
