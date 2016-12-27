using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SpotiFind.ExtensionMethods
{
    public static class CustomExtensions
    {
        public static T TryParseValue<T>(this string s) where T : struct
        {
            var method = typeof(T).GetMethod(
                "TryParse",
                new[] { typeof(string), typeof(T).MakeByRefType() }
                );
            T result = default(T);
            var parameters = new object[] { s, result };
            method.Invoke(null, parameters);
            return (T)parameters[1];
        }
    }
}