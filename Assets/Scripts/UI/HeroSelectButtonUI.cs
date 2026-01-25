using System;
using TMPro;
using UnityEngine;

namespace ND.UI
{
    public class HeroSelectButtonUI : MonoBehaviour
    {
        [SerializeField] TMP_Text costTxt;

        Action<E_HeroType> onSelectAction;
        E_HeroType type;

        public void SetSelectButton(E_HeroType type, Action<E_HeroType> onSelect = null)
        {
            costTxt.text = type switch
            {
                E_HeroType.Soldier => "군인\n(원거리)",
                E_HeroType.Police => "경찰\n(원거리)",
                E_HeroType.Doctor => "수의사\n(힐러)",
                E_HeroType.Girl => "마담\n(근접)",
                _ => "백수\n(근접)"
            };

            if (onSelectAction == null)
                onSelectAction = onSelect;

            this.type = type;
        }

        public void OnClickSelectButton()
        {
            if (onSelectAction != null)
                onSelectAction(type);
        }
    }
}
