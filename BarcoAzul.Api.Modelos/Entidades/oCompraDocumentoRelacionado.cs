namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oCompraDocumentoRelacionado
    {
        public string EmpresaId { get; set; }
        public string ProveedorId { get; set; }
        public string TipoDocumentoId { get; set; }
        public string Serie { get; set; }
        public string Numero { get; set; }
        public string Id { get; set; }
        public string NumeroDocumento { get; set; }
        public DateTime Fecha { get; set; }
    }
}
