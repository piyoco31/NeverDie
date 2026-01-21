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
        Pool<DamageTextUI> damageTextPool;
        Transform poolParentTr;
        ParticleDataSO particleDataSO;

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
            damageTextPool = Pool.Create(damageObj.GetComponent<DamageTextUI>(), 10, poolParentTr).NonLazy();
        }

        public async UniTask SpawnParticle(string key, Vector3 pos, float playTime = 0, Transform parentTr = null, Action particleFinish = null)
        {
            var particle = particleDict[key].Get();
            container.Inject(particle);
            particle.StartParticle(key, pos, playTime, parentTr, particleFinish);

            await UniTask.CompletedTask;
        }

        public async UniTask DespawnParticle(Particle particle)
        {
            particleDict[particle.ParticleKey].Take(particle);
            particle.transform.SetParent(poolParentTr);

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
    }
}
