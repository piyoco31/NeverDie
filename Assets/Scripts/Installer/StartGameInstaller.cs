using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ND.Installer
{
    public class StartGameInstaller : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {

        }

        private void Start()
        {
            DontDestroyOnLoad(this);
        }
    }
}
