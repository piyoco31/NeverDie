using System;
using UnityEngine;

namespace ND.Data
{
    [Serializable]
    public class HeroPositionData
    {
        public int idx;
        public bool isForward;
        public float posX;
        public float posY;
        public float posZ;
        public float heroUIPosY;
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
        public int rewardMoney;
    }

    [Serializable]
    public class UpgradeData
    {
        public int idx;
        public bool isRare;
        public string name;
        public string desc;
        public E_UpgradeTargetType upgradeTarget;
        public E_HeroStatType statType;
        public int price;
        public float upgradeValue;
    }
}
