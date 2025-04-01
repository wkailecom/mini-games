using System;
using System.Reflection;

namespace Jam3d
{
    /// <summary>
    /// 鍗曚緥鍩虹被銆佁
    /// </summary>
    /// <typeparam name="T">鍗曚緥绫诲瀷銆侞/typeparam>
    public abstract class Singleton<T> where T : ISingleton
    {
        /// <summary>
        /// 鍗曚緥绾跨▼閿併佁
        /// </summary>
        private static object _locker = new object();

        private static T _singleton;

        /// <summary>
        /// 鍗曚緥绫荤殑瀹炰緥銆佁
        /// </summary>
        public static T GetSingleton()
        {
            lock (_locker)
            {
                if (_singleton != null) return _singleton;
                _singleton = SingletonFactory.CreateSingleton<T>();
                _singleton.OnInit();

                return _singleton;
            }
        }
    }

    internal static class SingletonFactory
    {
        public static T CreateSingleton<T>() where T : ISingleton
        {
            var constructorInfos = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
            var constructorInfo = Array.Find(constructorInfos, c => c.GetParameters().Length == 0);
            if (constructorInfo == null)
            {
                throw new Exception($"Failed to find non-parameter constructor in '{nameof(T)}'.");
            }

            if (!(constructorInfo.Invoke(null) is T singleton))
            {
                throw new Exception($"Failed to create singleton from '{nameof(T)}'.");
            }
            return singleton;
        }
    }
}