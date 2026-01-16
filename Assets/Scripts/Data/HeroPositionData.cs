using UnityEngine;
using System.Collections.Generic;

namespace ND.Data
{ 
    [CreateAssetMenu(fileName = "HeroPositionDataAsset", menuName = "DataAsset/HeroPosition")]
    public class HeroPositionDataSO : ScriptableObject
    {
        public TextAsset textAsset;
        public List<HeroPositionData> heroPositionDataList;
    }
}
