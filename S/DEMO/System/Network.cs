namespace WSC.DEMO
{
    public sealed class Network : System<Network>
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
    }
}
