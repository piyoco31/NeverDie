using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace ND.UI
{
    using Manager;
    
    public class HeroSpawnButtonUI : MonoBehaviour
    {
        [Inject] HeroManager heroManager;
        [Inject] UserDataManager userDataManager;   

        [SerializeField] GameObject costButtonRoot;
        [SerializeField] GameObject selectButtonRoot;
        [SerializeField] TMP_Text costTxt;
        [SerializeField] List<HeroSelectButtonUI> selectButtonList;

        public int Idx { get; private set; }
        public bool IsSpawn { get; set; }
        

        InGameUI gameUI;
        int cost = 0;
        float heroUIPosY;

        public void Init(int idx, Vector3 pos, float posY)
        {
            Idx = idx;
            heroUIPosY = posY;
            IsSpawn = false;

            gameUI = GetComponentInParent<InGameUI>();

            var rootRectTr = gameUI.GetComponent<RectTransform>();
            var screenPos = Camera.main.WorldToScreenPoint(pos);

            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                rootRectTr,         // 캔버스 RectTransform
                screenPos,          // 변환할 스크린 좌표
                null,               // 이벤트 시스템 카메라 (없으면 null, 보통 Screen Space - Camera에서는 null)
                out localPoint      // 변환된 로컬 좌표
            );

            localPoint.y += 400;
            GetComponent<RectTransform>().anchoredPosition = localPoint;

            heroManager.SpawnCountRxProp.Subscribe(x =>
            {
                cost = x * 100;
                costTxt.text = "구매\n$" + cost;
            });

            OnClickCostButton(false);
        }

        public void OnClickCostButton(bool isSelectOpen)
        { 
            selectButtonRoot.SetActive(isSelectOpen);
            costButtonRoot.SetActive(!isSelectOpen);
            gameUI?.ActiveAllSelectButtons(!isSelectOpen, Idx);

            if (isSelectOpen)
                SetSelectButton();
        }

        void SetSelectButton()
        {
            var list = heroManager.HeroList;
            int totalCount = selectButtonList.Count;

            List<E_HeroType> spawnRequirelist = new();

            for (E_HeroType i = E_HeroType.Doctor; i < E_HeroType.NotHero; i++)
            {
                if (!heroManager.HeroList.Find(a => a.HeroType == i))
                    spawnRequirelist.Add(i);
            }

            for (int i = 0; i < totalCount; i++)
            {
                bool isActive = i < totalCount - heroManager.SpawnCount;

                selectButtonList[i].gameObject.SetActive(isActive);

                if (isActive)
                {
                    selectButtonList[i].SetSelectButton(spawnRequirelist[i], OnSpawnHero);
                }
            }
        }

        public async void OnSpawnHero(E_HeroType type)
        {
            if (cost <= userDataManager.Money)
            {
                userDataManager.Money -= cost;

                gameUI?.ActiveAllSelectButtons(false, -1);
                IsSpawn = true;

                var hero = await heroManager.SpawmHero(type, Idx);
                var UIObj = await Addressables.LoadAssetAsync<GameObject>("ND_UI_HeroUI");
                var heroUI = Instantiate(UIObj, gameUI?.HeroUITr).GetComponent<HeroUI>();
                heroUI.SetHeroUI(hero, GetComponent<RectTransform>().anchoredPosition, heroUIPosY);
                gameUI?.SetHeroDetailStat(hero);

                //OnClickCostButton(false);
                gameUI?.ActiveAllSelectButtons(true, Idx);
                //gameObject.SetActive(false);
            }
            else
                gameUI?.ShowMessage("소지금이 부족합니다.");
        }
    }
}
