﻿
namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oTipoDocumentoIdentidad
    {
        public string Id { get; set; }
        public string Descripcion { get; set; }
        public string Abreviatura { get; set; }

        public static implicit operator string(oTipoDocumentoIdentidad v)
        {
            throw new NotImplementedException();
        }
    }
}
