using System.Collections.Generic;
using UnityEngine;

namespace ND.UI
{
    using Character;

    public class HeroStatUI : MonoBehaviour
    {
        [SerializeField] GameObject heroDetailStatRoot;
        [SerializeField] List<HeroDetailStatUI> heroDetailStatList;

        private void Awake()
        {
            OnActiveDetailStat(false);
        }

        public void OnActiveDetailStat(bool isOn)
        {
            heroDetailStatRoot.SetActive(isOn);
        }

        public void SetHeroDetailStat(Character hero)
        {
            var detailUI = heroDetailStatList.Find(x => !x.IsInitComplete);
            detailUI.SetHeroDerailStat(hero);
        }
    }
}


