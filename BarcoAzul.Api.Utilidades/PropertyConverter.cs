using System.Collections;
using System.Collections.Specialized;
using System.Reflection;

namespace BarcoAzul.Api.Utilidades
{
    public class PropertyConverter
    {
        public static ListDictionary ConvertClassToDictionary(object obj)
        {
            ListDictionary dictionary = new ListDictionary();
            ConvertClassProperties(obj, dictionary);
            return dictionary;
        }

        private static void ConvertClassProperties(object obj, ListDictionary dictionary, string prefix = "")
        {
            PropertyInfo[] properties = obj.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                object value = property.GetValue(obj, null);

                if (value is not null && !typeof(IEnumerable).IsAssignableFrom(property.PropertyType) && property.PropertyType.IsClass && property.PropertyType != typeof(string))
                {
                    string newPrefix = prefix + property.Name;
                    ConvertClassProperties(value, dictionary, newPrefix);
                }
                else
                {
                    string key = prefix + property.Name;

                    if (!dictionary.Contains(key))
                        dictionary.Add(key, value);
                }
            }
        }
    }
}
