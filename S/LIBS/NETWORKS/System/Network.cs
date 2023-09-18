#if UNITY_5_6_OR_NEWER
#define UNITY
#else
using System.Threading;
#endif

namespace WSC
{
    public static class Network
    {
#if !UNITY
        private static Timer timer;
#endif
        public static void Initialize(IWebProtocolFactory factory = null)
        {
            if (factory != null)
                Factory = factory;

            NetworkW3Client.i.Initialize();
            NetworkWSClient.i.Initialize();
#if UNITY
            UnityEngine.Application.runInBackground = true;
#else
            timer = new((state) =>
            {
                NetworkWSClient.i.Update();
                NetworkW3Client.i.Update();
            }, null, 0, 100);
#endif
            RuntimeInitializeAttribute.Initialize();
        }

        internal static IWebProtocolFactory Factory
        {
            get;
            private set;
        } = new WebProtocolFactory();

        public static void Release()
        {
#if !UNITY
            timer?.Dispose();
#endif
        }
    }
}
