using Cysharp.Threading.Tasks;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace ND.UI
{
    using Character;
   

    public class HeroDetailStatUI : MonoBehaviour
    {
        [SerializeField] Image heroIcon;
        [SerializeField] TMP_Text heroName;
        [SerializeField] TMP_Text heroLeftStat;
        [SerializeField] TMP_Text heroRightStat;

        public bool IsInitComplete { get; private set; } = false;

        public async void SetHeroDetailStat(Character hero)
        {
            gameObject.SetActive(true);
            IsInitComplete = true;

            var sprite = await Addressables.LoadAssetAsync<Sprite>($"ND_SP_{hero.HeroType}Portrait");
            heroIcon.sprite = sprite;

            string lineStr = hero.IsForward ? "전위" : "후위";
            string jobStr = hero.HeroType switch
            {
                E_HeroType.Police  => "원거리",
                E_HeroType.Soldier => "원거리",
                E_HeroType.Doctor => "힐러",
                _ => "근접"
            };

            heroName.text = $"{hero.Name} ({jobStr}/{lineStr})";

            hero.StateRxProp.Subscribe(x =>
            {
                SetHeroLeftStat(hero);
            }).AddTo(this);

            hero.CurrentHpRxProp.Subscribe(x =>
            {
                SetHeroLeftStat(hero);
            }).AddTo(this);

            hero.MaxHpRxProp.Subscribe(x =>
            {
                SetHeroLeftStat(hero);
            }).AddTo(this);

            hero.AtkRxProp.Subscribe(x =>
            {
                SetHeroLeftStat(hero);
            }).AddTo(this);

            hero.ArmorRxProp.Subscribe(x =>
            {
                SetHeroRightStat(hero);
            }).AddTo(this);

            hero.DpsRxProp.Subscribe(x =>
            {
                SetHeroRightStat(hero);
            }).AddTo(this);

            hero.RangeRxProp.Subscribe(x =>
            {
                SetHeroRightStat(hero);
            }).AddTo(this);
        }

        // 상태, 공격력, HP
        void SetHeroLeftStat(Character hero)
        {
            var isDoctor = hero.HeroType == E_HeroType.Doctor;
            string stateStr = hero.IsDead ? "사망" : hero.State switch
            {
                E_CharacterState.Idle => isDoctor ? "치료준비" : "공격준비",
                E_CharacterState.Run => "이동중",
                _ => isDoctor ? "치료중" : "공격중"
            };

            string atkStr = isDoctor ? "치유력" : "ATK";
            heroLeftStat.text = $"상태 : {stateStr}\nHP : {hero.CurrentHp} / {hero.MaxHp}\n{atkStr} : {hero.Atk}";
        }

        // 아머, DPS, 사거리
        void SetHeroRightStat(Character hero)
        {
            heroRightStat.text = $"ARMOR : {hero.Armor}\nDPS : {hero.Dps}\nRANGE : {hero.Range}";
        }
    }
}
