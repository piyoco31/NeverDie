using Cysharp.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace ND.Manager
{
    public class MainGameManager : IAsyncStartable
    {
        public int WaveCount { get; private set; } = 0;

        [Inject] HeroManager heroManager;
        [Inject] MonsterManager monsterManager;


        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await StartMainGame();
        }

        async UniTask StartMainGame()
        {
            await heroManager.Init();

            await monsterManager.Init();

            await UniTask.Delay(System.TimeSpan.FromSeconds(1));

            var list = new List<int>() { 0, 1, 2, 3, 4 };

            var random = new System.Random();
            list = list.OrderBy(x => random.Next()).ToList();

            _ = heroManager.SpawmHero(E_HeroType.Doctor, list[0]);
            _ = heroManager.SpawmHero(E_HeroType.Police, list[1]);
            _ = heroManager.SpawmHero(E_HeroType.Girl, list[2]);
            _ = heroManager.SpawmHero(E_HeroType.Boy, list[3]);
            _ = heroManager.SpawmHero(E_HeroType.Soldier, list[4]);

            await UniTask.Delay(System.TimeSpan.FromSeconds(1));
            //await UniTask.WaitUntil(() => 1 == 2);

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

