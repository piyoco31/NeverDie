using UnityEngine;
using System;
using System.Collections.Generic;

namespace ND.Data
{
    [CreateAssetMenu(fileName = "MonsterDataAsset", menuName = "DataAsset/Monster")]
    public class MonsterDataSO : ScriptableObject
    {
        public TextAsset textAsset;
        public List<MonsterData> monsterDataList;
    }

    [Serializable]
    public class MonsterData
    {
        public E_MonsterType type;
        public string name;
        public float defaultSpeed;
        public float defaultHp;
        public float defaultAtk;
        public float defaultArmor;
        public float defaultDps;
    }
}
