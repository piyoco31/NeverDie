using System.Collections.Generic;
using UnityEngine;

namespace ND.Data
{ 
    [CreateAssetMenu(fileName = "UpgradeDataAsset", menuName = "DataAsset/Upgrade")]
    public class UpgradeDataSO : ScriptableObject
    {
        public TextAsset textAsset;
        public List<UpgradeData> upgradeDataList;
    }
}
