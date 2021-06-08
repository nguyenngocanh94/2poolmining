using System.Collections.Generic;
using System.Windows;

namespace _2pool
{
    public static class ObjectContainer
    {
        public static Dictionary<string, object> Container = new();

        public static void Set(string clazz, object instance)
        {
            Container[clazz] = instance;
        }

        public static object Get(string clazz)
        {
            return Container[clazz];
        }
    }
}