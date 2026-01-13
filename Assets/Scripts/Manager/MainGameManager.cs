using Cysharp.Threading.Tasks;
using System.Threading;
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

            _ = heroManager.SpawmHero(E_HeroType.Doctor);
            _ = heroManager.SpawmHero(E_HeroType.Police);
            _ = heroManager.SpawmHero(E_HeroType.Girl);
            _ = heroManager.SpawmHero(E_HeroType.Boy);
            _ = heroManager.SpawmHero(E_HeroType.Soldier);




            //await UniTask.WaitUntil(() => 1 == 2);

            _ = monsterManager.SpawmMonster(E_MonsterType.Zombie);

        }

        void EndMainGame()
        { 
            
        }
    }
}

