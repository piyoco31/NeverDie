using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI
{
    using Character;
    
    public class HeroUI : MonoBehaviour
    {
        [SerializeField] GameObject respawnButton;
        [SerializeField] TMP_Text costTxt;
        [SerializeField] GameObject heroUIRoot;
        [SerializeField] TMP_Text nameTxt;
        [SerializeField] Slider hpBar;
        [SerializeField] Slider coolTimeBar;

        InGameUI gameUI;

        bool isCoolTime;
        float coolTime = 0.0f;
        float coolTimeMax = 0.0f;
        int spawnCost = 0;
        int heroPosIdx = 0;

        void Update()
        {
            if (isCoolTime)
            {
                coolTime += Time.deltaTime;

                if (coolTime >= coolTimeMax)
                    coolTime = coolTimeMax;

                coolTimeBar.value = coolTime;
            }
        }

        public void SetHeroUI(Character hero, Vector2 pos, float posY)
        {
            heroPosIdx = hero.PosIdx;
            gameUI = GetComponentInParent<InGameUI>();
            GetComponent<RectTransform>().anchoredPosition = pos;
            var localPos = heroUIRoot.transform.localPosition;
            heroUIRoot.transform.localPosition = new Vector3(localPos.x, posY, localPos.z);

            hpBar.minValue = 0.0f;
            hpBar.maxValue = hero.MaxHp;

            coolTimeBar.minValue = 0.0f;
            coolTimeBar.maxValue = hero.Dps;

            hero.IsDeadRxProp.Subscribe(x => 
            {
                respawnButton.SetActive(x);
                heroUIRoot.SetActive(!x);
            }).AddTo(this);

            hero.NameRxProp.Subscribe(x => 
            {
                nameTxt.text = x.Replace(" ", "\n");
            }).AddTo(this);
            
            hero.CurrentHpRxProp.Subscribe(x =>
            {
                hpBar.value = x;
            }).AddTo(this);

            hero.MaxHpRxProp.Subscribe(x =>
            {
                hpBar.maxValue = x;
            }).AddTo(this);

            hero.DpsRxProp.Subscribe(x =>
            {
                coolTimeMax = x;
                coolTimeBar.maxValue = x;
            }).AddTo(this);

            hero.IsCoolTimeRxProp.DistinctUntilChanged().Subscribe(x => 
            {
                isCoolTime = x;

                if (x)
                    coolTime = 0.0f;
                else
                    coolTime = coolTimeMax;
            }).AddTo(this);

            gameUI.HeroManagerInst.SpawnCountRxProp.Subscribe(x =>
            {
                spawnCost = (gameUI.HeroManagerInst.RespawnCount + x) * 100;
                costTxt.text = "부활\n$" + spawnCost;
            }).AddTo(this);

            gameUI.HeroManagerInst.RespawnCountRxProp.Subscribe(x =>
            {
                spawnCost = (gameUI.HeroManagerInst.SpawnCount + x) * 100;
                costTxt.text = "부활\n$" + spawnCost;
            }).AddTo(this);
        }

        public void RespawnHero()
        {
            if (gameUI.UserDataManagerInst.Money >= spawnCost)
            {
                gameUI.UserDataManagerInst.Money -= spawnCost;
                _ = gameUI.HeroManagerInst.ReSpawnHero(heroPosIdx);
            }
            else
            {
                gameUI.ShowMessage("소지금이 부족합니다.");
            }
        }
    }
}
