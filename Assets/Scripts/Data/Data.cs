using System;
using UnityEngine;

namespace ND.Data
{
    [Serializable]
    public class HeroPositionData
    {
        public int Idx;
        public bool IsForward;
        public Vector3 Pos;
    }

    [Serializable]
    public class HeroData
    {
        public E_HeroType type;
        public string name;
        public string prefab;
        public float defaultSpeed;
        public float defaultHp;
        public float defaultAtk;
        public float defaultArmor;
        public float defaultDps;
        public float defaultRange;
        public int poolCount;
    }

    [Serializable]
    public class MonsterData
    {
        public E_MonsterType type;
        public string name;
        public string prefab;
        public float defaultSpeed;
        public float defaultHp;
        public float defaultAtk;
        public float defaultArmor;
        public float defaultDps;
        public float defaultRange;
        public int poolCount;
    }
}
