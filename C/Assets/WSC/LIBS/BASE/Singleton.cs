using System;

namespace WSC
{
    public class Singleton<T> where T : class
    {
        private static T instance = null;
        private static object sync = new();

        public static T Instance
        {
            get
            {
                lock (sync)
                {
                    if (instance == null)
                    {
                        if (instance == null)
                            instance = (T)Activator.CreateInstance(typeof(T), true);

                        (instance as Singleton<T>).Awake();
                    }
                    return instance;
                }
            }
        }

        protected virtual void Awake()
        {
        }

        protected virtual void OnDestroy()
        {
            instance = null;
        }

        public static void Destroy()
        {
            if (instance != null)
            {
                (instance as Singleton<T>).OnDestroy();
                instance = null;
            }
        }

        public static bool IsNull => (instance == null);
        public static T i => Instance;
    }
}
