using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ND.Manager
{
    using Data;
    using Character.Monster;
    using Character;
    using Redcode.Pools;

    public class MonsterManager : MonoBehaviour
    {
        Pool<Character> zombiePool;

        [SerializeField] MonsterDataSO monsterDataSO;
    
        public void Start()
        {
        }
    
        public async UniTask Init()
        {
            monsterDataSO = await Addressables.LoadAssetAsync<MonsterDataSO>("MonsterDataAsset");
    
            foreach (var data in monsterDataSO.monsterDataList)
            {
                var obj = await Addressables.LoadAssetAsync<GameObject>(data.prefab);
                var character = Instantiate(obj, Vector3.zero, Quaternion.identity);
    
                if (data.type == E_MonsterType.Zombie)
                {
                    var monster = character.AddComponent<Zombie>();
                    monster.InitCharacter(data);
                }
                else
                {
                    var monster = character.AddComponent<Soldier>();
                    monster.InitCharacter(data);
                }
            }
        }
    }
}
