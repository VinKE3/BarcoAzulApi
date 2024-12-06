using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BarcoAzul.Api.Modelos.Otros
{
    public class oMensaje
    {
        public MensajeTipo _tipo;
        public string _texto;

        public oMensaje(MensajeTipo tipo, string texto)
        {
            _tipo = tipo;
            _texto = texto;
        }

        public MensajeTipo Tipo { get => _tipo; }
        public string Texto { get => _texto; }
    }

    public enum MensajeTipo
    {
        Exito,
        Error,
        Informacion,
        Advertencia
    }
}
