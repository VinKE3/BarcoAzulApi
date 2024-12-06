namespace BarcoAzul.Api.Modelos.Otros
{
    public class oConfiguracionGlobal
    {
        public string EmpresaId { get; set; }
        public string EmpresaNumeroDocumentoIdentidad { get; set; }
        public string EmpresaNombre { get; set; }
        public string EmpresaDireccion { get; set; }
        public string EmpresaDepartamentoId { get; set; }
        public string EmpresaProvinciaId { get; set; }
        public string EmpresaDistritoId { get; set; }
        public DateTime FiltroFechaInicio { get; set; }
        public DateTime FiltroFechaFin { get; set; }
        public string DefaultUsuarioId { get; set; }
        public decimal DefaultPorcentajeIgv { get; set; }
        public string DefaultClienteId { get; set; }
        public string DefaultProveedorId { get; set; }
        public string DefaultPersonalId { get; set; }
        public string DefaultArticuloId { get; set; }
        public string DefaultLineaId { get; set; }
        public string DefaultSubLineaId { get; set; }
        public string DefaultGuiaConductorId { get; set; }
        public string DefaultGuiaTransportistaId { get; set; }
        public int DefaultMarcaId { get; set; }
        public string DefaultConductorId { get; set; }
        public int AnioHabilitado1 { get; set; }
        public int AnioHabilitado2 { get; set; }
        public string MesesHabilitados { get; set; }
        public DateTime? FechaUltimoCuadre { get; set; }

        public void ActualizarValores(oConfiguracionGlobal configuracionGlobal)
        {
            if (configuracionGlobal is null)
                return;

            EmpresaId = configuracionGlobal.EmpresaId;
            EmpresaNumeroDocumentoIdentidad = configuracionGlobal.EmpresaNumeroDocumentoIdentidad;
            EmpresaNombre = configuracionGlobal.EmpresaNombre;
            EmpresaDireccion = configuracionGlobal.EmpresaDireccion;
            FiltroFechaInicio = configuracionGlobal.FiltroFechaInicio;
            FiltroFechaFin = configuracionGlobal.FiltroFechaFin;
            DefaultUsuarioId = configuracionGlobal.DefaultUsuarioId;
            DefaultPorcentajeIgv = configuracionGlobal.DefaultPorcentajeIgv;
            DefaultClienteId = configuracionGlobal.DefaultClienteId;
            DefaultProveedorId = configuracionGlobal.DefaultProveedorId;
            DefaultPersonalId = configuracionGlobal.DefaultPersonalId;
            DefaultArticuloId = configuracionGlobal.DefaultArticuloId;
            DefaultLineaId = configuracionGlobal.DefaultLineaId;
            DefaultSubLineaId = configuracionGlobal.DefaultSubLineaId;
            DefaultGuiaConductorId = configuracionGlobal.DefaultGuiaConductorId;
            DefaultGuiaTransportistaId = configuracionGlobal.DefaultGuiaTransportistaId;
            DefaultMarcaId = configuracionGlobal.DefaultMarcaId;
            DefaultConductorId = configuracionGlobal.DefaultConductorId;
            AnioHabilitado1 = configuracionGlobal.AnioHabilitado1;
            AnioHabilitado2 = configuracionGlobal.AnioHabilitado2;
            MesesHabilitados = configuracionGlobal.MesesHabilitados;
            FechaUltimoCuadre = configuracionGlobal.FechaUltimoCuadre;
        }
    }
}
