using System.Threading;

namespace WSC.DEMO
{
    internal class System<T> : Singleton<T> where T : class
    {
        private static Timer timer;

        protected override void Awake()
        {
            base.Awake();
            timer = new((state) => Update(), null, 0, 100);
        }

        protected virtual void Update()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            timer?.Dispose();
        }
    }
}
