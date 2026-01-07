using UnityEngine;
using System;
using System.Collections.Generic;

namespace ND.Data
{ 
    [CreateAssetMenu(fileName = "HeroDataAsset", menuName = "DataAsset/Hero")]
    public class HeroDataSO : ScriptableObject
    {
        public TextAsset textAsset;
        public List<HeroData> heroDataList;
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
    }
}
