using BarcoAzul.Api.Utilidades.Extensiones;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitClienteContactoId
    {
        private readonly string _clienteId;
        private readonly int _contactoId;

        public oSplitClienteContactoId(string id)
        {
            if (id is null || id.Length < 7)
                throw new Exception("El ID no cumple con el formato.");

            _clienteId = id.Left(6);

            var contactoId = id.Mid(6);

            if (!int.TryParse(contactoId, out _contactoId))
                throw new Exception("El ID no cumple con el formato.");
        }

        public string ClienteId { get => _clienteId; }
        public int ContactoId { get => _contactoId; }
    }
}
