using Microsoft.AspNetCore.Mvc.Routing;

namespace BarcoAzulApi.Configuracion
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class SubAreaAttribute : RouteValueAttribute
    {
        public SubAreaAttribute(string subAreaName)
            : base("subarea", subAreaName)
        {
            if (string.IsNullOrEmpty(subAreaName))
            {
                throw new ArgumentException("Sub area name cannot be null or empty", nameof(subAreaName));
            }
        }
    }
}
