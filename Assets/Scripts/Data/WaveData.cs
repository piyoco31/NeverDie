using UnityEngine;
using System.Collections.Generic;

namespace ND.Data
{
    [CreateAssetMenu(fileName = "WaveDataAsset", menuName = "DataAsset/Wave")]
    public class WaveDataSO : ScriptableObject
    {
        public TextAsset textAsset;
        public List<WaveData> waveDataList;
    }
}
