using BarcoAzul.Api.Utilidades.Extensiones;

namespace BarcoAzul.Api.Utilidades
{
    public class Comun
    {
        //No quise referenciar el projecto Repositorio en Modelos así que cree este método
        public static string GetTipoDocumentoAbreviatura(string tipoDocumentoId)
        {
            switch (tipoDocumentoId)
            {
                case "01": return "FT";
                case "03": return "BV";
                case "07": return "NC";
                case "08": return "ND";
                case "09": return "GU";
                default: return tipoDocumentoId;
            }
        }

        /// <summary>
        /// Convertir cualquier Id de la tabla Venta (Conf_Codigo, TDoc_Codigo, Ven_Serie, Ven_Numero) a un Documento (TT-SSSS-NNNNNNN)
        /// </summary>
        /// <returns></returns>
        public static string VentaIdADocumento(string ventaId)
        {
            ventaId ??= string.Empty;

            if (!IsVentaIdValido(ventaId))
                return string.Empty;

            var tipoDocumentoAbreviatura = GetTipoDocumentoAbreviatura(ventaId.Mid(2, 2));
            var serie = ventaId.Mid(4, 4);
            var numero = ventaId.Mid(10, 8);

            return $"{tipoDocumentoAbreviatura}-{serie}-{numero}";
        }

        public static string CompraIdADocumento(string compraId)
        {
            compraId ??= string.Empty;

            if (!IsCompraIdValido(compraId))
                return string.Empty;

            var tipoDocumentoAbreviatura = GetTipoDocumentoAbreviatura(compraId.Mid(8, 2));
            var serie = compraId.Mid(10, 4);
            var numero = compraId.Mid(16, 8);

            return $"{tipoDocumentoAbreviatura}-{serie}-{numero}";
        }

        public static bool IsVentaIdValido(string ventaId) => ventaId is not null && ventaId.Length == 18; // 2(EmpresaId) + 2(TipoDocumentoId) + 4(Serie) + 10(Numero)

        public static bool IsCompraIdValido(string compraId) => compraId is not null && (compraId.Length == 24 || compraId.Length == 30); // 2(EmpresaId) + 6(ProveedorId) + 2(TipoDocumentoId) + 4(Serie) + 10(Numero) + 6(ClienteId)

        public static bool IsMovimientoBancarioIdValido(string movimientoBancarioId) => movimientoBancarioId is not null && movimientoBancarioId.Length == 10;

        private static string ToText(double value)
        {

            string Num2Text = "";

            value = Math.Truncate(value);

            if (value == 0) Num2Text = "CERO";

            else if (value == 1) Num2Text = "UNO";

            else if (value == 2) Num2Text = "DOS";

            else if (value == 3) Num2Text = "TRES";

            else if (value == 4) Num2Text = "CUATRO";

            else if (value == 5) Num2Text = "CINCO";

            else if (value == 6) Num2Text = "SEIS";

            else if (value == 7) Num2Text = "SIETE";

            else if (value == 8) Num2Text = "OCHO";

            else if (value == 9) Num2Text = "NUEVE";

            else if (value == 10) Num2Text = "DIEZ";

            else if (value == 11) Num2Text = "ONCE";

            else if (value == 12) Num2Text = "DOCE";

            else if (value == 13) Num2Text = "TRECE";

            else if (value == 14) Num2Text = "CATORCE";

            else if (value == 15) Num2Text = "QUINCE";

            else if (value < 20) Num2Text = "DIECI" + ToText(value - 10);

            else if (value == 20) Num2Text = "VEINTE";

            else if (value < 30) Num2Text = "VEINTI" + ToText(value - 20);

            else if (value == 30) Num2Text = "TREINTA";

            else if (value == 40) Num2Text = "CUARENTA";

            else if (value == 50) Num2Text = "CINCUENTA";

            else if (value == 60) Num2Text = "SESENTA";

            else if (value == 70) Num2Text = "SETENTA";

            else if (value == 80) Num2Text = "OCHENTA";

            else if (value == 90) Num2Text = "NOVENTA";

            else if (value < 100) Num2Text = ToText(Math.Truncate(value / 10) * 10) + " Y " + ToText(value % 10);

            else if (value == 100) Num2Text = "CIEN";

            else if (value < 200) Num2Text = "CIENTO " + ToText(value - 100);

            else if ((value == 200) || (value == 300) || (value == 400) || (value == 600) || (value == 800)) Num2Text = ToText(Math.Truncate(value / 100)) + "CIENTOS";

            else if (value == 500) Num2Text = "QUINIENTOS";

            else if (value == 700) Num2Text = "SETECIENTOS";

            else if (value == 900) Num2Text = "NOVECIENTOS";

            else if (value < 1000) Num2Text = ToText(Math.Truncate(value / 100) * 100) + " " + ToText(value % 100);

            else if (value == 1000) Num2Text = "MIL";

            else if (value < 2000) Num2Text = "MIL " + ToText(value % 1000);

            else if (value < 1000000)
            {

                Num2Text = ToText(Math.Truncate(value / 1000)) + " MIL";

                if ((value % 1000) > 0) Num2Text = Num2Text + " " + ToText(value % 1000);

            }

            else if (value == 1000000) Num2Text = "UN MILLON";

            else if (value < 2000000) Num2Text = "UN MILLON " + ToText(value % 1000000);

            else if (value < 1000000000000)
            {

                Num2Text = ToText(Math.Truncate(value / 1000000)) + " MILLONES ";

                if ((value - Math.Truncate(value / 1000000) * 1000000) > 0) Num2Text = Num2Text + " " + ToText(value - Math.Truncate(value / 1000000) * 1000000);

            }

            else if (value == 1000000000000) Num2Text = "UN BILLON";

            else if (value < 2000000000000) Num2Text = "UN BILLON " + ToText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            else
            {

                Num2Text = ToText(Math.Truncate(value / 1000000000000)) + " BILLONES";

                if ((value - Math.Truncate(value / 1000000000000) * 1000000000000) > 0) Num2Text = Num2Text + " " + ToText(value - Math.Truncate(value / 1000000000000) * 1000000000000);

            }

            return Num2Text;

        }

        public static string ConvertirNumeroEnLetras(decimal num, string codMoneda = "")
        {

            string res, dec = "";

            Int64 entero;

            int decimales;

            decimal nro = num;


            entero = Convert.ToInt64(Math.Truncate(nro));

            decimales = Convert.ToInt32(Math.Round((nro - entero) * 100, 2));

            if (decimales > 0)
            {

                dec = " CON " + decimales.ToString("00.##") + "/100";

            }
            else
            {
                dec = " CON 00/100";
            }

            res = ToText(Convert.ToDouble(entero)) + dec;


            if (string.IsNullOrEmpty(codMoneda))
            {
                return res;
            }
            else
            {
                string strMoneda = "";

                if (codMoneda == "PEN")
                    strMoneda = "SOLES";

                if (codMoneda == "USD")
                    strMoneda = "DOLARES";

                return $"{res} {strMoneda}";
            }
        }
    }
}
