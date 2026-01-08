using UnityEngine;
using System.Collections.Generic;

namespace ND.Data
{ 
    [CreateAssetMenu(fileName = "HeroDataAsset", menuName = "DataAsset/Hero")]
    public class HeroDataSO : ScriptableObject
    {
        public TextAsset textAsset;
        public List<HeroData> heroDataList;
    }
}
