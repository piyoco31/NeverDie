using ND.Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ND.UI
{
    public class UpgradeShopButtonUI : MonoBehaviour
    {
        [SerializeField] GameObject rareImg;
        [SerializeField] Button upgradeButton;
        [SerializeField] TMP_Text upgradeNameTxt;
        [SerializeField] TMP_Text upgradeDescTxt;
        [SerializeField] TMP_Text upgradePriceTxt;

        public void SetUpgradeButton(UpgradeData data)
        {
            upgradeButton.enabled = true;

            upgradeNameTxt.text = data.name;
            upgradeDescTxt.text = data.desc;
            upgradePriceTxt.text = "$" + data.price;

            rareImg.SetActive(data.isRare);
        }

        
    }
}
