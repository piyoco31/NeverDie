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
        [Inject] Light light;

        List<WaveData> waveDataList;
        List<Material> skyBoxMatList = new();

        Color dayLightColor;
        bool isRainyDay;

        public async UniTask StartAsync(CancellationToken cancellation)
        {
            await StartMainGame();
        }

        async UniTask StartMainGame()
        {
            dayLightColor = light.color;
            WaveCount = 0;
            isRainyDay = false;

            var mat = await Addressables.LoadAssetAsync<Material>("ND_MT_Day");
            skyBoxMatList.Add(mat);
            mat = await Addressables.LoadAssetAsync<Material>("ND_MT_Night");
            skyBoxMatList.Add(mat);

            var waveDataSO = await Addressables.LoadAssetAsync<WaveDataSO>("ND_SO_WaveDataAsset");
            waveDataList = waveDataSO.waveDataList;

            await userDataManager.Init();
            await particleManager.Init();
            await heroManager.Init(this);
            await monsterManager.Init(this);
            await gameUI.Init(this);

            ChangeEnviroment();

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
                    await UniTask.Delay(System.TimeSpan.FromSeconds(2));
                }

                await UniTask.Delay(System.TimeSpan.FromSeconds(5));
            }

            await UniTask.WaitUntil(() => TotalWaveMonsterCount <= 0);
            gameUI.PlaySound("UpgradeShop");
            await gameUI.OpenUpgradeShop(true);
            
            WaveCount++;
            ChangeEnviroment();
            await UniTask.WaitUntil(() => !gameUI.IsUpgradeShopOpen);

            MonsterWave();
        }

        async void ChangeEnviroment()
        {
            bool isDay = WaveCount % 2 == 0;
            RenderSettings.skybox = skyBoxMatList[isDay ? 0 : 1];
            light.color = isDay ? dayLightColor : Color.black;

            if (Random.Range(0, 100) < 50)
            {
                if (!isRainyDay)
                {
                    await particleManager.SpawnRainyParticle(true);
                    isRainyDay = true;
                }
            }
            else if (isRainyDay)
            {
                await particleManager.SpawnRainyParticle(false);
            }
        }
    }
}

