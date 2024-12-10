using BarcoAzul.Api.Modelos.Atributos;
using BarcoAzul.Api.Modelos.Otros;
using BarcoAzul.Api.Servicios.RucDni.Repositorio;

namespace BarcoAzul.Api.Logica.Servicio
{
    public class bRucDni : bComun
    {
        private readonly string _url;
        private readonly string _token;

        public bRucDni(string url, string token) : base(null, origen: "Consulta RUC - DNI")
        {
            _url = url;
            _token = token;
        }

        public async Task<oRucDni> Consultar(string tipo, string numeroDocumentoIdentidad)
        {
            try
            {
                dConsultarRucDni dRucDni = new(_url, _token);
                var success = await dRucDni.Get(tipo, numeroDocumentoIdentidad);

                if (!success)
                    throw new Exception(dRucDni.Mensaje);

                var response = dRucDni.RucDniRespuesta;

                if (response.Data == null)
                    throw new MensajeException(new oMensaje(MensajeTipo.Error, $"{_origen}: No se encontró datos con el número de documento identidad ingresado (Nro Doc.: {numeroDocumentoIdentidad})"));

                if (tipo == "ruc")
                {
                    return new oRucDni
                    {
                        NumeroDocumentoIdentidad = response.Data?.Ruc,
                        Nombre = response.Data?.RazonSocial,
                        Ubigeo = response.Data?.Ubigeo,
                        Direccion = numeroDocumentoIdentidad.StartsWith("10") ? "-" : response.Data?.Direccion,
                        DireccionCompleta = numeroDocumentoIdentidad.StartsWith("10") ? "-" : response.Data?.DireccionCompleta
                    };
                }
                else
                {
                    return new oRucDni
                    {
                        NumeroDocumentoIdentidad = response.Data?.Numero,
                        Nombre = response.Data?.NombreCompleto,
                        Ubigeo = new string[3] { "", "", "" },
                        Direccion = "-",
                        DireccionCompleta = "-",
                        Nombres = response.Data?.Nombres,
                        Apellidos = $"{response.Data?.ApellidoPaterno} {response.Data?.ApellidoMaterno}".Trim()
                    };
                }
            }
            catch (Exception ex)
            {
                ManejarExcepcion(ex, _origen, TipoAccion.Consultar);
                return null;
            }
        }
    }
}
