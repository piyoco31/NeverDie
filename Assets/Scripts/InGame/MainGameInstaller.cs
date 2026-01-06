using VContainer;
using VContainer.Unity;

namespace ND.Installer
{
    using Hero;
    using Monster;

    public class MainGameInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<Hero>().AsSelf();
            builder.Register<Monster>(Lifetime.Singleton);
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}