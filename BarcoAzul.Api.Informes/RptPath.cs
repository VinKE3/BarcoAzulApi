namespace BarcoAzul.Api.Informes
{
    public class RptPath
    {
        private static string _globalPath;

        public static string globalPath
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_globalPath))
                {
                    _globalPath = $"{AppDomain.CurrentDomain.BaseDirectory}Rpt";
                }

                return _globalPath;
            }
            set => _globalPath = value;
        }
        public static string RptCPEGREPath => $"{globalPath}/RptCPEGRE";
        public static string RptNotaPedidoPath => $"{globalPath}/RptNP";
        public static string RptOrdenCompraPath => $"{globalPath}/RptOC";
        public static string RptSalidaArticulosPath => $"{globalPath}/RptSA";
        public static string RptSalidaCilindrosPath => $"{globalPath}/RptSC";
        public static string RptLetraCambioVentaPath => $"{globalPath}/RptLC";
        public static string RptGuiaRemisionPath => $"{globalPath}/RptGU";
        public static string RptPlanillaCobroPath => $"{globalPath}/RptPL";
        public static string RptInformesPath => $"{globalPath}/Otros";
    }
}
