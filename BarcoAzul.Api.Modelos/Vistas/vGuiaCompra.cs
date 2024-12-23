namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vGuiaCompra
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaTraslado { get; set; }
        public string NumeroDocumento { get; set; }
        public string ProveedorNombre { get; set; }
        public bool IsBloqueado { get; set; }
        public bool AfectarStock { get; set; }
        public string MarcaPlaca { get; set; }
    }
}
