using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vArticulo
    {
        public string Id { get; set; }
        public string CodigoBarras { get; set; }
        public string Descripcion { get; set; }
        public string UnidadMedidaAbreviatura { get; set; }
        public decimal Stock { get; set; }
        public string MonedaId { get; set; }
        public decimal PrecioCompra { get; set; }
        public decimal PrecioVenta { get; set; }
        public bool ControlarStock { get; set; }
        public bool ActualizarPrecio { get; set; }
        public bool IsActivo { get; set; }
    }
}
