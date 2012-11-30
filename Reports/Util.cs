using System;

namespace ASR.Reports.Logs
{
    public static class Util
    {
        public static bool ContainsAny(this string text, string[] bits)
        {
            if (string.IsNullOrEmpty(text)) return false;
            foreach (var bit in bits)
            {
                if (text.Contains(bit)) return true;
            }
            return false;
        }

        public static bool ContainsAny(this string text, Sitecore.Collections.StringList bits)
        {
            if (string.IsNullOrEmpty(text)) return false;
            foreach (var bit in bits)
            {
                if (text.Contains(bit)) return true;
            }
            return false;
        }

        public static object CreateObject(this Type t)
        {
            foreach (var constructorInfo in t.GetConstructors(System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Public))
            {
                if (constructorInfo.GetParameters().Length == 0)
	            {
                    return constructorInfo.Invoke(null);
	            }
            }
            return null;
        }
    }
}
