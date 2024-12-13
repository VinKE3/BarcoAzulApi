using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Modelos.Vistas
{
    public class vConductor
    {
        public string Id { get; set; }
        public string TipoConductor { get; set; }
        public string Nombre { get; set; }
        public string NumeroDocumentoIdentidad { get; set; }
        public string LicenciaConducir { get; set; }
        public string NroRegistro { get; set; }
    }
}
