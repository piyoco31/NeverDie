using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace ND.Manager
{
    using Data;
    using Particle;
    using UI;

    public class ParticleManager : MonoBehaviour
    {
        [Inject] IObjectResolver container;

        Dictionary<string, Pool<Particle>> particleDict = new();
        List<Particle> spawnParticleList = new();
        Pool<DamageTextUI> damageTextPool;
        Pool<RewardUI> rewardTextPool;
        Transform poolParentTr;
        ParticleDataSO particleDataSO;
        Particle bgmSFX = null;

        public async UniTask Init()
        {
            GameObject trObj = new();
            trObj.name = "ParticlePool";
            poolParentTr = trObj.transform;

            particleDataSO = await Addressables.LoadAssetAsync<ParticleDataSO>("ND_SO_ParticleDataAsset");
            var originalObj = await Addressables.LoadAssetAsync<GameObject>("ND_FX_Particle");

            foreach (var data in particleDataSO.particleDataList)
            {
                var key = data.name;
                var obj = Instantiate(originalObj);
                var particle = obj.GetComponent<Particle>();

                var particlePrefab = string.IsNullOrEmpty(data.prefab) ? null : await Addressables.LoadAssetAsync<GameObject>($"ND_FX_{data.prefab}");
                var audioClip = string.IsNullOrEmpty(data.audio) ? null : await Addressables.LoadAssetAsync<AudioClip>($"ND_SFX_{data.audio}");

                if (particlePrefab)
                {
                    particlePrefab = Instantiate(particlePrefab);
                    particlePrefab.transform.localScale = new Vector3(data.scaleX, data.scaleY, data.scaleZ);
                }

                particle.Init(data.isLoopAudio, particlePrefab, audioClip);

                if (!particleDict.ContainsKey(key))
                {
                    Pool<Particle> pool = Pool.Create(particle, data.poolCount, poolParentTr).NonLazy();
                    
                    particleDict.Add(key, pool);
                }

                Destroy(obj);
                obj = null;
            }

            var damageObj = await Addressables.LoadAssetAsync<GameObject>("ND_UI_DamageText");
            damageTextPool = Pool.Create(damageObj.GetComponent<DamageTextUI>(), 30, poolParentTr).NonLazy();

            var rewardObj = await Addressables.LoadAssetAsync<GameObject>("ND_UI_RewardText");
            rewardTextPool = Pool.Create(rewardObj.GetComponent<RewardUI>(), 10, poolParentTr).NonLazy();
        }

        public async UniTask SpawnParticle(string key, Vector3 pos, float playTime = 0, Transform parentTr = null, Action particleFinish = null)
        {
            var particle = particleDict[key].Get();
            container.Inject(particle);
            spawnParticleList.Add(particle);

            if (playTime > 0)
                particle.StartParticle(key, pos, playTime, parentTr, particleFinish);
            else
                particle.StartParticle(key, pos, parentTr, particleFinish);

            await UniTask.CompletedTask;
        }

        public async UniTask DespawnParticle(Particle particle)
        {
            particleDict[particle.ParticleKey].Take(particle);
            particle.transform.SetParent(poolParentTr);
            spawnParticleList.Remove(particle);

            // Debug.Log($"PoolCount:{particleDict[particle.ParticleKey].Count}");
            await UniTask.CompletedTask;
        }

        public async UniTask SpawnDamageText(Transform parentTr, float damage)
        {
            var damageText = damageTextPool.Get();
            container.Inject(damageText);
            damageText.Init(damage, parentTr);

            await UniTask.CompletedTask;
        }

        public async UniTask DespawnDamageText(DamageTextUI damageText)
        {
            damageTextPool.Take(damageText);
            damageText.transform.SetParent(poolParentTr);

            await UniTask.CompletedTask;
        }

        public async UniTask SpawnRewardText(Vector3 pos, float damage)
        {
            var rewardText = rewardTextPool.Get();
            container.Inject(rewardText);
            rewardText.Init(damage, pos);

            await UniTask.CompletedTask;
        }

        public async UniTask DespawnRewardText(RewardUI rewardText)
        {
            rewardTextPool.Take(rewardText);

            await UniTask.CompletedTask;
        }

        public async UniTask SpawnRainyParticle(bool isSpawn)
        {
            if (isSpawn)
            {
                await SpawnParticle("RainSplashes", Vector3.zero);
                await SpawnParticle("RainFalling", new Vector3(0.0f, 5.0f, 0.0f));
            }
            else
            {
                var list = spawnParticleList.FindAll(x => x.ParticleKey == "RainSplashes" || x.ParticleKey == "RainFalling");

                foreach (var item in list)
                {
                    await DespawnParticle(item);
                }
            }
        }

        public void PlayBGM(string name)
        {
            StopBGM();

            var particle = particleDict[name].Get();
            container.Inject(particle);
            spawnParticleList.Add(particle);
            bgmSFX = particle;

            particle.StartParticle(name, Vector3.zero);
        }

        public void StopBGM()
        {
            if (bgmSFX)
            {
                bgmSFX.StopParticle();
                bgmSFX = null;
            }
        }
    }
}
