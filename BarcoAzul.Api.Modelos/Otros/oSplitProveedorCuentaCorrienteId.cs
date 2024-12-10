using BarcoAzul.Api.Utilidades.Extensiones;


namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitProveedorCuentaCorrienteId
    {
        private readonly string _proveedorId;
        private readonly int _cuentaCorrienteId;

        public oSplitProveedorCuentaCorrienteId(string id)
        {
            if (id is null || id.Length < 7)
                throw new Exception("El ID no cumple con el formato.");

            _proveedorId = id.Left(6);

            var contactoId = id.Mid(6);

            if (!int.TryParse(contactoId, out _cuentaCorrienteId))
                throw new Exception("El ID no cumple con el formato.");
        }

        public string ProveedorId { get => _proveedorId; }
        public int CuentaCorrienteId { get => _cuentaCorrienteId; }
    }
}
