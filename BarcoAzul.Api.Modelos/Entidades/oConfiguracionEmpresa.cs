using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace BarcoAzul.Api.Modelos.Entidades
{
    public class oConfiguracionEmpresa
    {
        public string Id { get; set; }
        [Required(ErrorMessage = "El número de documento de identidad es requerido.")]
        public string NumeroDocumentoIdentidad { get; set; }
        [Required(ErrorMessage = "El nombre es requerido.")]
        public string Nombre { get; set; }
        [Required(ErrorMessage = "La dirección es requerida.")]
        public string Direccion { get; set; }
        [Required(ErrorMessage = "El departamento es requerido.")]
        public string DepartamentoId { get; set; }
        [Required(ErrorMessage = "La provincia es requerida.")]
        public string ProvinciaId { get; set; }
        [Required(ErrorMessage = "El distrito es requerido.")]
        public string DistritoId { get; set; }
        public string Telefono { get; set; }
        public string Celular { get; set; }
        public string CorreoElectronico { get; set; }
        public string Observacion { get; set; }
        public string ConcarEmpresaId { get; set; }
        public string ConcarEmpresaNombre { get; set; }
        public string ConcarUsuarioVenta { get; set; }
        public string ConcarUsuarioCompra { get; set; }
        public string ConcarUsuarioPago { get; set; }
        public string ConcarUsuarioCobro { get; set; }
        [Required(ErrorMessage = "La fecha de inicio del filtro es requerida.")]
        public DateTime FiltroFechaInicio { get; set; }
        [Required(ErrorMessage = "La fecha de fin del filtro es requerido.")]
        public DateTime FiltroFechaFin { get; set; }
        [Required(ErrorMessage = "El año 1 es requerido.")]
        public int AnioHabilitado1 { get; set; }
        [Required(ErrorMessage = "El año 2 es requerido.")]
        public int AnioHabilitado2 { get; set; }
        public string MesesHabilitados { get; set; }
        public IEnumerable<oConfiguracionEmpresaIGV> PorcentajesIGV { get; set; }
        public IEnumerable<oConfiguracionEmpresaRetencion> PorcentajesRetencion { get; set; }
        public IEnumerable<oConfiguracionEmpresaDetraccion> PorcentajesDetraccion { get; set; }
        public IEnumerable<oConfiguracionEmpresaPercepcion> PorcentajesPercepcion { get; set; }

        public void ProcesarDatos()
        {
            NumeroDocumentoIdentidad = NumeroDocumentoIdentidad?.Trim();
            Nombre = Nombre?.Trim();
            Direccion = Direccion?.Trim();
            Telefono = Telefono?.Trim();
            CorreoElectronico = CorreoElectronico?.Trim();
            Observacion = Observacion?.Trim();
        }

        public void CompletarDatosPorcentajesIGV()
        {
            if (PorcentajesIGV != null)
            {
                foreach (var porcentajeIGV in PorcentajesIGV)
                {
                    porcentajeIGV.EmpresaId = Id;
                }
            }
        }

        public void CompletarDatosPorcentajesRetencion()
        {
            if (PorcentajesRetencion != null)
            {
                foreach (var porcentajeRetencion in PorcentajesRetencion)
                {
                    porcentajeRetencion.EmpresaId = Id;
                }
            }
        }

        public void CompletarDatosPorcentajesDetraccion()
        {
            if (PorcentajesDetraccion != null)
            {
                foreach (var porcentajeDetraccion in PorcentajesDetraccion)
                {
                    porcentajeDetraccion.EmpresaId = Id;
                }
            }
        }

        public void CompletarDatosPorcentajesPercepcion()
        {
            if (PorcentajesPercepcion != null)
            {
                foreach (var porcentajePercepcion in PorcentajesPercepcion)
                {
                    porcentajePercepcion.EmpresaId = Id;
                }
            }
        }
    }

    public class oConfiguracionEmpresaPorcentaje
    {
        [JsonIgnore]
        public string EmpresaId { get; set; }
        public decimal Porcentaje { get; set; }
        public bool Default { get; set; }
        public string TipoPercepcion { get; set; }
    }

    public class oConfiguracionEmpresaPorcentajePercepcion
    {
        [JsonIgnore]
        public string EmpresaId { get; set; }
        public decimal Porcentaje { get; set; }
        public bool Default { get; set; }
        public string TipoPercepcion { get; set; }
    }

    public class oConfiguracionEmpresaIGV : oConfiguracionEmpresaPorcentaje { }

    public class oConfiguracionEmpresaRetencion : oConfiguracionEmpresaPorcentaje { }

    public class oConfiguracionEmpresaDetraccion : oConfiguracionEmpresaPorcentaje { }

    public class oConfiguracionEmpresaPercepcion : oConfiguracionEmpresaPorcentaje { }
}
