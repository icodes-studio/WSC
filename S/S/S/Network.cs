namespace WSC.DEMO
{
    internal sealed class Network : System<Network>
    {
        public void Initialize(IWebProtocolFactory factory = null)
        {
            NetworkW3Client.i.Initialize(factory);
            NetworkWSClient.i.Initialize(factory);

            RuntimeInitializeAttribute.Initialize();
        }

        protected override void Update()
        {
            base.Update();

            NetworkW3Client.i.Update();
            NetworkWSClient.i.Update();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            NetworkW3Client.Destroy();
            NetworkWSClient.Destroy();
        }
    }
}
