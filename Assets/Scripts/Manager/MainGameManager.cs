using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ND.Manager
{
    using UI;

    public class MainGameManager : IAsyncStartable
    {
        public int WaveCount { get; private set; } = 0;

        [Inject] HeroManager heroManager;
        [Inject] MonsterManager monsterManager;
        [Inject] UserDataManager userDataManager;
        [Inject] InGameUI gameUI;

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await StartMainGame();
        }

        async UniTask StartMainGame()
        {
            await heroManager.Init();

            await monsterManager.Init();

            await gameUI.Init();

            await UniTask.WaitUntil(() => heroManager.SpawnCount > 0);

            await UniTask.Delay(System.TimeSpan.FromSeconds(3));

            for (int i = 0; i < 50; i++)
            {
                await UniTask.Delay(System.TimeSpan.FromSeconds(1));
                _ = monsterManager.SpawmMonster((E_MonsterType)Random.Range(0, (int)E_MonsterType.NotMonster));
            }
        }

        void EndMainGame()
        { 
            
        }
    }
}

