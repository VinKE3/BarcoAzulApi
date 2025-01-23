namespace BarcoAzul.Api.Modelos.Otros
{
    public class oConfiguracionConcarVentas
    {
        public string BaseDatosNombre { get; set; }
        public string EmpresaId { get; set; }
        public string SubDiario { get; set; }
        public string CuentaSoles { get; set; }
        public string CuentaDolares { get; set; }
        public string CuentaIgv { get; set; }
        public string CuentaContable { get; set; }
        public string CuentaInafectoExonerado { get; set; }

        public void ProcesarDatos()
        {
            BaseDatosNombre = BaseDatosNombre?.Trim();
            EmpresaId = EmpresaId?.Trim();
            SubDiario = SubDiario?.Trim();
            CuentaSoles = CuentaSoles?.Trim();
            CuentaDolares = CuentaDolares?.Trim();
            CuentaIgv = CuentaIgv?.Trim();
            CuentaContable = CuentaContable?.Trim();
        }
    }
}
