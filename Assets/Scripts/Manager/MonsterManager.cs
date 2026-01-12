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
        Dictionary<E_MonsterType, MonsterData> dataDic = new();
        MonsterDataSO monsterDataSO;
        Transform poolParentTr;

        List<Character> monsterList = new();

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

                if (!dataDic.ContainsKey(type))
                {
                    dataDic.Add(type, data);
                }

                if (!poolDic.ContainsKey(type))
                {
                    Pool<Character> pool = Pool.Create(obj.GetComponent<Character>(), data.poolCount, poolParentTr).NonLazy();
                    poolDic.Add(type, pool);
                }
            }
        }

        public async UniTask SpawmMonster(E_MonsterType type)
        {
            var monster = poolDic[type].Get();
            monster.InitCharacter(dataDic[type]);
            monsterList.Add(monster);

            //await hero.MoveToPos(new Vector3(dataDic[type].startPosX, dataDic[type].startPosY, dataDic[type].startPosZ));
            //
            monster.transform.forward = Vector3.right;
        }

        public Character GetTargetMonster(Character attacker)
        {
            Vector3 attackerPos = attacker.transform.position;
            float distance = attacker.Range;

            foreach (var monster in monsterList)
            {
                if (!monster || monster.State == E_CharacterState.Death) continue;

                if (Vector3.Distance(attackerPos, monster.transform.position) <= distance)
                    return monster;
            }

            return null;
        }
    }
}
