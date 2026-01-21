using System.Collections.Generic;
using UnityEngine;

namespace ND.Data
{
    [CreateAssetMenu(fileName = "MonsterDataAsset", menuName = "DataAsset/Monster")]
    public class MonsterDataSO : ScriptableObject
    {
        public TextAsset textAsset;
        public List<MonsterData> monsterDataList;
    }
}
