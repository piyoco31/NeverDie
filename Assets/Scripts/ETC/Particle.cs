using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;

namespace ND.Particle
{
    using Manager;

    public class Particle : MonoBehaviour
    {
        [Inject] ParticleManager particleManager;

        [SerializeField] ParticleSystem particle;
        [SerializeField] AudioSource audioComp;

        public string ParticleKey { get; private set; } 

        Action onParticleFinish = null;

        void Awake()
        {
            audioComp = GetComponent<AudioSource>();
        }

        public void Init(bool isLoopAudio = false, GameObject particlePrefab = null, AudioClip audioClip = null)
        {
            if (audioClip)
            {
                audioComp.clip = audioClip;
                audioComp.loop = isLoopAudio;
                audioComp.Stop();
            }

            if (particlePrefab)
            {
                particlePrefab.transform.parent = transform;
                particle = GetComponentInChildren<ParticleSystem>();
                particle.Stop();
            }
        }

        public async void StartParticle(string key, Vector3 pos, float playTime, Transform parentTr = null, Action particleFinish = null)
        {
            StartParticle(key, pos, parentTr, particleFinish);

            await UniTask.Delay(TimeSpan.FromSeconds(playTime));

            StopParticle();
        }

        public void StartParticle(string key, Vector3 pos, Transform parentTr = null, Action particleFinish = null)
        {
            ParticleKey = key;
            onParticleFinish = particleFinish;

            if (audioComp.clip)
                audioComp.Play();

            if (particle)
                particle.Play();

            if (parentTr)
                transform.SetParent(parentTr);

            transform.localPosition = pos;

            transform.localRotation = Quaternion.identity;
        }

        public void StopParticle()
        {
            if (audioComp.clip)
                audioComp.Stop();

            if (particle)
                particle.Stop();

            if (onParticleFinish != null)
                onParticleFinish();

            _ = particleManager.DespawnParticle(this);
        }
    }
}

