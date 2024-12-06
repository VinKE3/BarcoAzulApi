

namespace BarcoAzul.Api.Modelos.Interfaces
{
    public interface ILogicaServiceFactory
    {
        ILogicaService GetInstance(string area, string tipo);
    }
}
