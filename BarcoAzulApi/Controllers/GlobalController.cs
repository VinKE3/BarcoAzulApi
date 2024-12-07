using BarcoAzul.Api.Modelos.Otros;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.StaticFiles;

namespace BarcoAzulApi.Controllers
{
    public class GlobalController : ControllerBase
    {
        public List<oMensaje> Mensajes { get; private set; }

        public GlobalController()
        {
            Mensajes = new();
        }

        protected void AgregarMensajeAlInicio(oMensaje mensaje)
        {
            Mensajes.Insert(0, mensaje);
        }

        protected void AgregarMensaje(oMensaje mensaje)
        {
            Mensajes.Add(mensaje);
        }

        protected void AgregarMensajes(List<oMensaje> mensajes)
        {
            Mensajes.AddRange(mensajes);
        }

        protected void AgregarErroresModeloEnMensajes(ModelStateDictionary ModelStateValues)
        {
            var errores = new List<oMensaje>();

            foreach (var modelState in ModelStateValues.Values)
            {
                foreach (var modelError in modelState.Errors)
                {
                    errores.Add(new oMensaje(MensajeTipo.Error, modelError.ErrorMessage));
                }
            }

            AgregarMensajes(errores);
        }

        internal oRespuesta<T> GenerarRespuesta<T>(bool success, T data)
        {
            return new oRespuesta<T>
            {
                Success = success,
                Data = data,
                Messages = AgruparMensajes(Mensajes)
            };
        }

        private IEnumerable<oMensajeAgrupado> AgruparMensajes(IEnumerable<oMensaje> mensajes)
        {
            var tipos = mensajes.Select(x => x.Tipo).Distinct().OrderBy(x => x);

            foreach (var tipo in tipos)
            {
                yield return new oMensajeAgrupado
                {
                    Tipo = tipo,
                    Textos = mensajes.Where(x => x.Tipo == tipo).Select(x => x.Texto)
                };
            }
        }

        internal oRespuesta<object> GenerarRespuesta(bool success, object data = null) => GenerarRespuesta<object>(success, data);

        protected string GetContentType(string nombreArchivo)
        {
            new FileExtensionContentTypeProvider().TryGetContentType(nombreArchivo, out string contentType);
            return contentType ?? "application/octet-stream";
        }
    }
}
