using Cysharp.Threading.Tasks;
using VContainer;
using VContainer.Unity;
using System.Threading;

namespace ND.Manager
{
    public class MainGameManager : IAsyncStartable
    {
        public int WaveCount { get; private set; } = 1;

        [Inject] HeroManager heroManager;
        [Inject] MonsterManager monsterManager;


        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await StartMainGame();
        }

        async UniTask StartMainGame()
        {
            await heroManager.Init();

            await UniTask.Delay(System.TimeSpan.FromSeconds(3));

            await monsterManager.Init();

            // await UniTask.Delay();

            await UniTask.WaitUntil(() => 1 == 2);

        }

        void EndMainGame()
        { 
            
        }
    }
}

