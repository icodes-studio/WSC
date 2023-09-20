using UnityEngine;

namespace WSC.UNITY
{
    public sealed class Network : System<Network>
    {
        public void Initialize(IWebProtocolFactory factory = null)
        {
            Application.runInBackground = true;

            NetworkW3Client.i.Initialize(factory);
            NetworkWSClient.i.Initialize(factory);

            RuntimeInitializeAttribute.Initialize();
        }

        private void Update()
        {
            NetworkWSClient.i.Update();
            NetworkW3Client.i.Update();
        }
    }
}
