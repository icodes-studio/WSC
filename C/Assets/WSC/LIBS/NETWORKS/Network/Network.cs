#if UNITY_5_6_OR_NEWER
#define UNITY
#endif

using System.Threading;

namespace WSC
{
    public static class Network
    {
#if !UNITY
        private static Timer timer;
#endif
        public static void Initialize(IWebSocketFactory factory = null)
        {
            NetworkW3Client.i.Initialize();
            NetworkWSClient.i.Initialize(factory);
#if UNITY
            UnityEngine.Application.runInBackground = true;
#else
            timer = new((state) => { NetworkWSClient.i.Update(); }, null, 0, 100);
#endif
            RuntimeInitializeAttribute.Initialize();
        }

        public static void Release()
        {
#if !UNITY
            timer?.Dispose();
#endif
        }
    }
}
