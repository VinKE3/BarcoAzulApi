using System.ComponentModel.DataAnnotations;

namespace BarcoAzul.Api.Modelos.Atributos
{
    public class NoSpacesAttribute : ValidationAttribute
    {
        public NoSpacesAttribute()
        {
            ErrorMessage = "The string cannot contain spaces";
        }

        public override bool IsValid(object value)
        {
            if (value is null)
                return true;

            string strValue = value.ToString();
            return !strValue.Contains(" ");
        }
    }
}
