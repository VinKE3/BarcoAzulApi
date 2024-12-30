using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Modelos.Otros.Informes
{
    public class oRegistroPersonal
    {
        public string Id { get; set; }
        public string NombreCompleto { get; set; }
        public string NumeroDocumentoIdentidad { get; set; }
        public string EstadoCivilDescripcion { get; set; }
        public DateTime? FechaNacimiento { get; set; }
        public string CargoDescripcion { get; set; }
        public bool IsActivo { get; set; }
    }
}
