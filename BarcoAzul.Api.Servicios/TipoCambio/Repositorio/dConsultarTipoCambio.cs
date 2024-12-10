using BarcoAzul.Api.Servicios.TipoCambio.Modelos;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json;
using RestSharp;
using System.Net.Security;
using System.Net;
using System.Security.Cryptography.X509Certificates;

namespace BarcoAzul.Api.Servicios.TipoCambio.Repositorio
{
    public class dConsultarTipoCambio
    {
        public async static Task<oConsultarTipoCambioRespuesta> Consultar(string url, oConsultarTipoCambioSolicitud solicitud)
        {
            RestClient restClient = new(url);
            RestRequest restRequest = new((string)null, Method.Post)
            {
                RequestFormat = DataFormat.Json
            };

            ServicePointManager.SecurityProtocol |= (SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12);
            ServicePointManager.ServerCertificateValidationCallback = ((object param0, X509Certificate param1, X509Chain param2, SslPolicyErrors param3) => true);
            ServicePointManager.SecurityProtocol &= ~SecurityProtocolType.Ssl3;

            restRequest.AddHeader("content-type", "application/json; charset=utf-8");
            restRequest.AddBody(JsonConvert.SerializeObject(solicitud));

            RestResponse restResponse = await restClient.ExecuteAsync(restRequest);

            if (restResponse.StatusCode == HttpStatusCode.OK)
                return JsonConvert.DeserializeObject<oConsultarTipoCambioRespuesta>(restResponse.Content, new IsoDateTimeConverter { DateTimeFormat = "dd/MM/yyyy" });
            else
                return null;
        }
    }
}
