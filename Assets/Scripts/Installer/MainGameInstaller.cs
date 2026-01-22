using VContainer;
using VContainer.Unity;
using UnityEngine;

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
            builder.Register<ParticleManager>(Lifetime.Singleton);
            builder.RegisterComponentInHierarchy<InGameUI>();
            builder.RegisterComponentInHierarchy<Light>();

            builder.RegisterEntryPoint<MainGameManager>().AsSelf();
        }

        private void Start()
        {
            //DontDestroyOnLoad(this);
        }
    }
}