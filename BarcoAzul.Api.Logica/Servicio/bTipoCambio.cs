using BarcoAzul.Api.Modelos.Atributos;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Servicios.TipoCambio.Modelos;
using BarcoAzul.Api.Servicios.TipoCambio.Repositorio;

namespace BarcoAzul.Api.Logica.Servicio
{
    public class bTipoCambio : bComun
    {
        private readonly string _url;
        private readonly string _token;

        public bTipoCambio(string url, string token) : base(null, origen: "Consulta Tipo de Cambio")
        {
            _url = url;
            _token = token;
        }

        public async Task<object> Consultar(DateTime fecha)
        {
            try
            {
                var response = await dConsultarTipoCambio.Consultar(_url, new oConsultarTipoCambioSolicitud
                {
                    Token = _token,
                    TipoCambio = new oParametroTipoCambio
                    {
                        Moneda = "PEN",
                        FechaInicio = fecha.AddDays(-3).ToString("dd/MM/yyyy"),
                        FechaFin = fecha.ToString("dd/MM/yyyy"),
                    }
                });

                if (response == null || !response.Success || response.TiposCambio?.Count == 0)
                    throw new MensajeException(new oMensaje(MensajeTipo.Error, $"{_origen}: no se pudo obtener el tipo de cambio del día {fecha:dd/MM/yyyy}."));

                return response.TiposCambio.OrderByDescending(x => x.Fecha).First();  //Si es que no hay el TC del día, obtengo el más cercano al día buscado.
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }
    }
}
