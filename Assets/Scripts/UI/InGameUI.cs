using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using System.Collections.Generic;

namespace ND.UI
{
    using Data;
    using ND.Manager;

    public class InGameUI : MonoBehaviour
    {
        [Inject] IObjectResolver container;
        [Inject] HeroManager heroManager;
        [SerializeField] Transform spawnButtonTr;

        List<HeroSpawnButtonUI> spawnButtonlist= new();

        public async UniTask Init()
        { 
            await InitSelectButtonUI();
        }

        async UniTask InitSelectButtonUI()
        {
            var positionDataSO = await Addressables.LoadAssetAsync<HeroPositionDataSO>("ND_SO_HeroPositionDataAsset");
            var poslist = positionDataSO.heroPositionDataList;
            var UIObj = await Addressables.LoadAssetAsync<GameObject>("ND_UI_SpawnButton");

            for (int i = 0; i < 5; i++)
            {
                var spawnButton = Instantiate(UIObj, spawnButtonTr).GetComponent<HeroSpawnButtonUI>();
                container.Inject(spawnButton);
                spawnButton.Init(i, poslist[i].Pos);
                spawnButtonlist.Add(spawnButton);
            }
        }

        public void ActiveAllSelectButtons(bool isActive, int currentSelectIdx = 0)
        {
            for (int i = 0; i < spawnButtonlist.Count; i++)
            {
                if (spawnButtonlist[i].IsSpawn) continue;

                spawnButtonlist[i].gameObject.SetActive(isActive ? true : i == currentSelectIdx ? true : false);
            }
        }
    }
}
