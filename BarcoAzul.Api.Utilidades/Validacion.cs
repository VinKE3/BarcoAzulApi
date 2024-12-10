using System.Text.RegularExpressions;

namespace BarcoAzul.Api.Utilidades
{
    public class Validacion
    {
        public static bool ValidarRuc(string Ruc)
        {
            if (IsInteger(Ruc) && Ruc.Length == 11)
            {
                return ValRucAlgorithm(Ruc);
            }

            return false;
        }

        public static bool IsInteger(string input)
        {
            if (input == null)
                return false;

            Regex regex = new Regex(@"^[0-9]+$");
            return regex.IsMatch(input);
        }

        private static bool ValRucAlgorithm(string valor)
        {
            valor = valor.Trim();
            if (valor.Length == 8)
            {
                var suma = 0;
                for (int i = 0; i < valor.Length - 1; i++)
                {
                    var digito = valor[i] - '0';
                    if (i == 0) suma += (digito * 2);
                    else suma += (digito * (valor.Length - i));
                }
                var resto = suma % 11;
                if (resto == 1) resto = 11;
                if (resto + (valor[valor.Length - 1] - '0') == 11)
                {
                    return true;
                }
            }
            else if (valor.Length == 11)
            {
                var suma = 0;
                var x = 6;
                for (int i = 0; i < valor.Length - 1; i++)
                {
                    if (i == 4) x = 8;
                    var digito = valor[i] - '0';
                    x--;
                    if (i == 0) suma += (digito * x);
                    else suma += (digito * x);
                }
                var resto = suma % 11;
                resto = 11 - resto;

                if (resto >= 10) resto = resto - 10;
                if (resto == valor[valor.Length - 1] - '0')
                {
                    return true;
                }
            }

            return false;
        }
    }
}
