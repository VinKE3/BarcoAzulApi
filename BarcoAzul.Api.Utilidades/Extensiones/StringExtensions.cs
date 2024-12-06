namespace BarcoAzul.Api.Utilidades.Extensiones
{
    public static class StringExtensions
    {
        public static string Left(this string text, int length)
        {
            if (text == null)
                return string.Empty;

            if (length > text.Length - 1)
                return text;

            return text.Substring(0, length);
        }

        public static string Mid(this string text, int indexStart)
        {
            if (text == null)
                return string.Empty;

            if (indexStart > text.Length - 1)
                return string.Empty;

            return text.Substring(indexStart);
        }

        public static string Mid(this string text, int indexStart, int length)
        {
            if (text == null)
                return string.Empty;

            if (indexStart > text.Length - 1)
                return string.Empty;

            if (indexStart + length > text.Length)
                return text.Substring(indexStart);

            return text.Substring(indexStart, length);
        }

        public static string Right(this string text, int length)
        {
            if (text == null)
                return string.Empty;

            if (length > text.Length - 1)
                return text;

            return text.Substring(text.Length - length);
        }
    }
}
