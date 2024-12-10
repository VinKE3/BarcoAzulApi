using BarcoAzul.Api.Servicios.RucDni.Modelos;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace BarcoAzul.Api.Servicios.RucDni.Repositorio
{
    public class dConsultarRucDni
    {
        private readonly string _url;
        private readonly string _token;
        private string _mensaje;
        private oConsultarRucDniRespuesta _rucDniRespuesta;

        public string Mensaje { get => _mensaje; }
        public oConsultarRucDniRespuesta RucDniRespuesta { get => _rucDniRespuesta; }

        public dConsultarRucDni(string url, string token)
        {
            _url = url;
            _token = token;
        }

        public async Task<bool> Get(string tipo, string numeroDocumentoIdentidad)
        {
            try
            {
                A:
                RestClient restClient = new(string.Format(_url, tipo, numeroDocumentoIdentidad));
                RestRequest restRequest = new((string)null, Method.Get)
                {
                    RequestFormat = DataFormat.Json
                };

                ServicePointManager.SecurityProtocol |= (SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
                ServicePointManager.ServerCertificateValidationCallback = ((object param0, X509Certificate param1, X509Chain param2, SslPolicyErrors param3) => true);
                ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;

                restRequest.AddHeader("content-type", "application/json; charset=utf-8");
                restRequest.AddHeader("Authorization", $"Bearer {_token}");
                RestResponse restResponse = await restClient.ExecuteAsync(restRequest);

                if (restResponse.StatusCode == HttpStatusCode.OK)
                    _rucDniRespuesta = JsonConvert.DeserializeObject<oConsultarRucDniRespuesta>(restResponse.Content);
                else if (restResponse.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    Thread.Sleep(20000);
                    goto A;
                }
                else
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                _mensaje = $"Error. Motivo: {ex.Message}";
                return false;
            }
        }
    }
}
