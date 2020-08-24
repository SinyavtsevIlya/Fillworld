using Zenject;

namespace CleanRx
{
    public static class DIContainerExtensions
    {
        public static void BindSubKernel(this DiContainer container)
        {
            container.Bind<Kernel>().AsSingle().OnInstantiated<Kernel>((a, b) => b.Initialize()).NonLazy();
        }
    }
}
