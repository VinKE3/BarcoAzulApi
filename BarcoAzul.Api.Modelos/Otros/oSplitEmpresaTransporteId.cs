using BarcoAzul.Api.Utilidades.Extensiones;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oSplitEmpresaTransporteId
    {
        private readonly string _empresaId;
        private readonly string _empresaTransporteId;

        public oSplitEmpresaTransporteId(string id)
        {
            if (id is null || id.Length < 3)
                throw new Exception("El ID no cumple con el formato.");

            _empresaId = id.Left(2);
            _empresaTransporteId = id.Mid(2);
        }

        public string EmpresaId { get => _empresaId; }
        public string EmpresaTransporteId { get => _empresaTransporteId; }
    }
}
