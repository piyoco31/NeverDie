using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VContainer;

namespace ND.UI
{
    using Character;
    using Manager;
    
    public class HeroUI : MonoBehaviour
    {
        [Inject] HeroManager heroManager;

        [SerializeField] GameObject respawnButton;
        [SerializeField] GameObject heroUIRoot;
        [SerializeField] TMP_Text nameTxt;
        [SerializeField] Slider hpBar;
        [SerializeField] Slider coolTimeBar;

        Character hero;
        bool isCoolTime;
        float coolTime = 0.0f;
        float coolTimeMax = 0.0f;


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
                nameTxt.text = x;
            }).AddTo(this);
            
            hero.CurrentHpRxProp.Subscribe(x =>
            {
                //Debug.Log($"HP:{x}");
                hpBar.value = x;
            }).AddTo(this);

            hero.MaxHpRxProp.Subscribe(x =>
            {
                hpBar.maxValue = x;
            }).AddTo(this);

            hero.DpsRxProp.Subscribe(x =>
            {
                coolTimeMax = x;
            }).AddTo(this);

            hero.IsCoolTimeRxProp.DistinctUntilChanged().Subscribe(x => 
            {
                isCoolTime = x;

                if (x)
                    coolTime = 0.0f;
                else
                    coolTime = coolTimeMax;
            }).AddTo(this);
        }
    }
}
