using BarcoAzul.Api.Utilidades.Extensiones;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitCuentaCorrienteId
    {
        public readonly string _empresaId;
        public readonly string _cuentaCorrienteId;

        public oSplitCuentaCorrienteId(string id)
        {
            if (id is null || id.Length != 4)
                throw new Exception("El ID no cumple con el formato.");

            _empresaId = id.Left(2);
            _cuentaCorrienteId = id.Right(2);
        }

        public string EmpresaId { get => _empresaId; }
        public string CuentaCorrienteId { get => _cuentaCorrienteId; }
    }
}
