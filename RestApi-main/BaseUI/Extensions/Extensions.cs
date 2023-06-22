using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BaseUI.Extensions
{
    public static class Extensions
    {
        #region Extensiones de listas

        //Check if not null or empty list
        public static bool IsNotNullOrEmpty<T>(this IEnumerable<T> source)
        {
            if (source != null && source.Any())
                return true;
            else
                return false;
        }

        public static bool IsNotNullOrEmpty<T>(this IList<T> source)
        {
            if (source != null && source.Any())
                return true;
            else
                return false;
        }

        public static bool IsNotNullOrEmpty<T>(this List<T> source)
        {
            if (source != null && source.Any())
                return true;
            else
                return false;
        }


        #endregion

        public static string ConvertToPassword(this string source)
        {
            char[] characters = source.ToCharArray();
            foreach (var i in characters)
                source = source.Replace(i, '*');
            return source;
        }
    }
}