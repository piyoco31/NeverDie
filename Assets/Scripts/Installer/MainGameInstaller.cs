using VContainer;
using VContainer.Unity;

namespace ND.Installer
{
    using Manager;
    using UI;

    public class MainGameInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<UserDataManager>(Lifetime.Singleton);
            builder.Register<HeroManager>(Lifetime.Singleton);
            builder.Register<MonsterManager>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<InGameUI>();

            builder.RegisterEntryPoint<MainGameManager>().AsSelf();
        }

        private void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}