using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using System.Linq;

namespace ND.UI
{
    using Data;
    using Manager;
   
    public class InGameUI : MonoBehaviour
    {
        [Inject] IObjectResolver container;
        [Inject] HeroManager heroManager;
        [Inject] UserDataManager userDataManager;

        [SerializeField] GameObject upgradeShop;
        [SerializeField] Transform spawnButtonTr;
        [SerializeField] TMP_Text userMoneyTxt;

        [SerializeField] List<UpgradeShopButtonUI> upgradeButtonList;

        List<HeroSpawnButtonUI> spawnButtonlist= new();
        List<UpgradeData> upgradeDataList;

        public async UniTask Init()
        {
            var dataSO = await Addressables.LoadAssetAsync<UpgradeDataSO>("ND_SO_UpgradeDataAsset");

            upgradeDataList = dataSO.upgradeDataList;

            await InitSelectButtonUI();

            userDataManager.MoneyRxProp.Subscribe(x =>
            {
                userMoneyTxt.text = x.ToString();
            });

            await OpenUpgradeShop();
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
                spawnButton.Init(i, new Vector3(poslist[i].posX, poslist[i].posY, poslist[i].posZ));
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

        public async UniTask OpenUpgradeShop()
        {
            var random = new System.Random();
            var list = upgradeDataList.OrderBy(x => random.Next()).ToList();

            for (int i = 0; i < upgradeButtonList.Count; i++)
            {
                upgradeButtonList[i].SetUpgradeButton(list[i]);
            }

            upgradeShop.SetActive(true);

            await UniTask.CompletedTask;
        }

        public void ReRollUpgradeShop()
        {
            //if (userDataManager.Money >= 100)
            //{
                userDataManager.Money -= 100;
                _ = OpenUpgradeShop();
            //}
        }

        public void CloseUpgradeShop()
        {
            upgradeShop.SetActive(false);
        }
    }
}
