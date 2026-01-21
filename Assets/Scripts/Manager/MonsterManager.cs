using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace ND.Manager
{
    using Character;
    using Data;

    public class MonsterManager : MonoBehaviour
    {
        [Inject] IObjectResolver container;

        List<Character> monsterList = new();
        Dictionary<E_MonsterType, Pool<Character>> poolDic = new();
        Dictionary<E_MonsterType, MonsterData> dataDic = new();
        MonsterDataSO monsterDataSO;
        Transform poolParentTr;
        MainGameManager mainGameManager;

        public async UniTask Init(MainGameManager mainGameManager)
        {
            this.mainGameManager = mainGameManager;

            GameObject trObj = new();
            trObj.name = "MonsterPool";
            poolParentTr = trObj.transform;

            monsterDataSO = await Addressables.LoadAssetAsync<MonsterDataSO>("ND_SO_MonsterDataAsset");
    
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
            container.Inject(monster);
            await monster.InitCharacter(dataDic[type]);
            monsterList.Add(monster);

            monster.transform.position = new Vector3(10, 0, Random.Range(-1.8f, 2.0f));
            monster.transform.forward = Vector3.left;
            
            monster.StartAttack();
        }

        public Character GetTargetMonster(Character attacker)
        {
            Vector3 attackerPos = attacker.transform.position;
            float distance = attacker.Range;

            foreach (var monster in monsterList)
            {
                if (!monster || monster.IsDead) continue;

                if (Vector3.Distance(attackerPos, monster.transform.position) <= distance)
                    return monster;
            }

            return null;
        }

        public async UniTask DeSpawnMonster(Character monster)
        {
            await UniTask.Delay(System.TimeSpan.FromSeconds(3));

            poolDic[monster.MonsterType].Take(monster);
            monsterList.Remove(monster);
            mainGameManager.TotalWaveMonsterCount--;
        }
    }
}
