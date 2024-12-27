using BarcoAzul.Api.Utilidades;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oDocumentoReferenciaVenta
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public string NumeroDocumento => Comun.VentaIdADocumento(Id);
    }

    public class oDocumentoReferenciaCompra
    {
        public string Id { get; set; }
        public DateTime FechaEmision { get; set; }
        public DateTime FechaContable { get; set; }
        public string NumeroDocumento => Comun.CompraIdADocumento(Id);
    }
}
