using UnityEngine;
using System.Collections.Generic;

namespace ND.Data
{
    [CreateAssetMenu(fileName = "MonsterDataAsset", menuName = "DataAsset/Monster")]
    public class MonsterDataSO : ScriptableObject
    {
        public TextAsset textAsset;
        public List<MonsterData> monsterDataList;
    }
}
