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
        public static void Initialize(IWeb www = null, IWebSocketFactory factory = null)
        {
            NetworkW3Client.i.Initialize(www);
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
