using BarcoAzul.Api.Modelos.Interfaces;

namespace BarcoAzulApi.Configuracion
{
    public class LogicaServiceFactory : ILogicaServiceFactory
    {
        private readonly IEnumerable<ILogicaService> _logicaServices;

        public LogicaServiceFactory(IEnumerable<ILogicaService> logicaServices)
        {
            _logicaServices = logicaServices;
        }

        public ILogicaService GetInstance(string area, string tipo)
        {
            Type type = Type.GetType($"MCWebAPI.Logica.{area}.b{tipo}, MCWebApi.Logica");

            if (type is null)
                throw new InvalidOperationException();

            return GetService(type);
        }

        public ILogicaService GetService(Type type) => _logicaServices.FirstOrDefault(x => x.GetType() == type);
    }
}
