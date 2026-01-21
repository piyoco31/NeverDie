using System;
using R3;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI
{
    using Data;

    public class UpgradeShopButtonUI : MonoBehaviour
    {
        [SerializeField] GameObject soldOutImg;
        [SerializeField] GameObject rareImg;
        [SerializeField] Button upgradeButton;
        [SerializeField] TMP_Text upgradeNameTxt;
        [SerializeField] TMP_Text upgradeDescTxt;
        [SerializeField] TMP_Text upgradePriceTxt;

        public ReactiveProperty<bool> buttonEnableRxProp = new(false);
        public bool IsEnable { get { return buttonEnableRxProp.Value; } set { buttonEnableRxProp.Value = value; }  }

        Action<UpgradeShopButtonUI, int> onSelectAction;
        int idx;

        private void Awake()
        {
            buttonEnableRxProp.Subscribe(x =>
            {
                upgradeButton.enabled = x;
                soldOutImg.SetActive(!x);
            }).AddTo(this);
        }

        public void SetUpgradeButton(UpgradeData data, Action<UpgradeShopButtonUI, int> onSelect = null)
        {
            IsEnable = true;

            idx = data.idx;
            upgradeNameTxt.text = data.name;
            upgradeDescTxt.text = data.desc;
            upgradePriceTxt.text = "$" + data.price;

            rareImg.SetActive(data.isRare);

            if (onSelectAction == null)
                onSelectAction = onSelect;
        }

        public void OnClickUpgradeButton()
        {
            if (onSelectAction != null)
                onSelectAction(this, idx);
        }
    }
}
