using ND.Manager;
using ND.UI;
using VContainer;
using VContainer.Unity;

namespace ND.Installer
{
    public class StartGameInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterComponentInHierarchy<TitleUI>();

            builder.RegisterEntryPoint<MainGameManager>().AsSelf();
        }

        private void Start()
        {
            //DontDestroyOnLoad(this);
        }
    }
}
