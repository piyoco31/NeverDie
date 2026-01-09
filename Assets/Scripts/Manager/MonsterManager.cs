using Cysharp.Threading.Tasks;
using Redcode.Pools;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ND.Manager
{
    using Character;
    using Character.Monster;
    using Data;
    

    public class MonsterManager : MonoBehaviour
    {
        Dictionary<E_MonsterType, Pool<Character>> poolDic = new();
        MonsterDataSO monsterDataSO;
        Transform poolParentTr;
    
        public async UniTask Init()
        {
            GameObject trObj = new();
            trObj.name = "MonsterPool";
            poolParentTr = trObj.transform;

            monsterDataSO = await Addressables.LoadAssetAsync<MonsterDataSO>("MonsterDataAsset");
    
            foreach (var data in monsterDataSO.monsterDataList)
            {
                var obj = await Addressables.LoadAssetAsync<GameObject>(data.prefab);

                var type = data.type;
                
                if (type == E_MonsterType.Zombie)
                {
                    var monster = obj.GetComponent<Zombie>();
                    monster.InitCharacter(data);
                }
                else
                {
                    var monster = obj.GetComponent<Soldier>();
                    monster.InitCharacter(data);
                }

                if (!poolDic.ContainsKey(type))
                {
                    Pool<Character> pool = Pool.Create(obj.GetComponent<Character>(), data.poolCount, poolParentTr).NonLazy();
                    poolDic.Add(type, pool);
                }
            }
        }
    }
}
