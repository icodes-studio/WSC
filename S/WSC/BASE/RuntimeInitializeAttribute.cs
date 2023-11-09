using System;
using System.Linq;
using System.Reflection;

namespace WSC
{
    public class RuntimeInitializeAttribute : Attribute
    {
        public static void Initialize()
        {
            var methods = AppDomain
                .CurrentDomain
                .GetAssemblies()
                .SelectMany(x => x.GetTypes())
                .Where(x => x.IsClass)
                .SelectMany(x => x.GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
                .Where(x => x.GetCustomAttributes(typeof(RuntimeInitializeAttribute), false).FirstOrDefault() != null);

            foreach (var method in methods)
                method.Invoke(null, null);
        }
    }
}
