using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using VContainer.Unity;

namespace ND.Manager
{
    using Data;
    using UI;

    public class MainGameManager : IAsyncStartable
    {
        private ReactiveProperty<int> waveCountRxProp = new(0);
        public ReactiveProperty<int> WaveCountRxProp { get => waveCountRxProp; }
        public int WaveCount { get { return waveCountRxProp.Value; } private set { waveCountRxProp.Value = value; } }
        public int TotalWaveMonsterCount { get; set; } = 0;

        [Inject] HeroManager heroManager;
        [Inject] MonsterManager monsterManager;
        [Inject] UserDataManager userDataManager;
        [Inject] ParticleManager particleManager;
        [Inject] InGameUI gameUI;

        List<WaveData> waveDataList;

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await StartMainGame();
        }

        async UniTask StartMainGame()
        {
            WaveCount = 0;

            var waveDataSO = await Addressables.LoadAssetAsync<WaveDataSO>("ND_SO_WaveDataAsset");
            waveDataList = waveDataSO.waveDataList;

            await userDataManager.Init();
            await particleManager.Init();
            await heroManager.Init(this);
            await monsterManager.Init(this);
            await gameUI.Init(this);

            await UniTask.WaitUntil(() => heroManager.SpawnCount > 0);

            await UniTask.Delay(System.TimeSpan.FromSeconds(3));

            MonsterWave();
        }

        async void MonsterWave()
        {
            int waveIdx = WaveCount >= waveDataList.Count ? waveDataList.Count - 1 : WaveCount;
            var dataList = waveDataList[waveIdx];
            dataList.waveMonsterList.ForEach(x =>
            {
                TotalWaveMonsterCount += x.count;
            });

            foreach (var data in dataList.waveMonsterList)
            {
                for (int i = 0; i < data.count; i++)
                {
                    await monsterManager.SpawmMonster(data.type);
                }

                await UniTask.Delay(System.TimeSpan.FromSeconds(10));
            }

            await UniTask.WaitUntil(() => TotalWaveMonsterCount <= 0);
            await gameUI.OpenUpgradeShop();
            await UniTask.WaitUntil(() => !gameUI.IsUpgradeShopOpen);

            WaveCount++;

            MonsterWave();
        }

        void EndMainGame()
        {

        }
    }
}

