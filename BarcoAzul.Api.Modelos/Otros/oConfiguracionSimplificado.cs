using BarcoAzul.Api.Modelos.Entidades;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oConfiguracionSimplificado
    {
        public string EmpresaNumeroDocumentoIdentidad { get; set; }
        public string EmpresaNombre { get; set; }
        public string EmpresaDireccion { get; set; }
        public string EmpresaDepartamentoId { get; set; }
        public string EmpresaProvinciaId { get; set; }
        public string EmpresaDistritoId { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }
        public string UsuarioId { get; set; }
        public string ClienteId { get; set; }
        public string ProveedorId { get; set; }
        public string PersonalId { get; set; }
        public string LineaId { get; set; }
        public string SubLineaId { get; set; }
        public string ArticuloId { get; set; }
        public int MarcaId { get; set; }
        public string ConductorId { get; set; }
        public decimal PorcentajeIGV { get; set; }

        //TODO: DESCOMENTAR CUANDO AGREGUE CLIENTE Y PROVEEDOR
        //public oCliente Cliente { get; set; }
        //public oProveedor Proveedor { get; set; }
        public oArticulo Articulo { get; set; }
    }
}
