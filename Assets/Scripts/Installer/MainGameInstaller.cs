using Redcode.Pools;
using VContainer;
using VContainer.Unity;

namespace ND.Installer
{
    using Manager;

    public class MainGameInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<HeroManager>(Lifetime.Singleton);
            builder.Register<MonsterManager>(Lifetime.Singleton);

            builder.RegisterEntryPoint<MainGameManager>().AsSelf();
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}