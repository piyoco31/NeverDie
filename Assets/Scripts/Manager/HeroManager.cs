using System.Linq;
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
    
    public class HeroManager : MonoBehaviour
    {
        List<Character> heroList = new();
        Dictionary<E_HeroType, Pool<Character>> poolDic = new();
        Dictionary<E_HeroType, HeroData> dataDic = new();
        Dictionary<int, HeroPositionData> positionDataDic = new();

        Transform poolParentTr;
        HeroDataSO heroDataSO;
        IObjectResolver container;

        public HeroManager(IObjectResolver container)
        {
            this.container = container;
        }

        public async UniTask Init()
        {
            GameObject trObj = new();
            trObj.name = "HeroPool";
            poolParentTr = trObj.transform;

            var bgObj = await Addressables.LoadAssetAsync<GameObject>("ND_Bg");

            Instantiate(bgObj);

            heroDataSO = await Addressables.LoadAssetAsync<HeroDataSO>("HeroDataAsset");

            foreach (var data in heroDataSO.heroDataList)
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

            var positionDataSO = await Addressables.LoadAssetAsync<HeroPositionDataSO>("HeroPositionDataAsset");

            positionDataDic = positionDataSO.heroPositionDataList.ToDictionary(keySelector: m => m.Idx, elementSelector: m => m);
        }

        public async UniTask SpawmHero(E_HeroType type, int idx = 0)
        {
            var hero = poolDic[type].Get();
            container.Inject(hero);
            hero.InitCharacter(dataDic[type], idx);

            await hero.MoveToPos(positionDataDic[idx].Pos);

            hero.transform.forward = Vector3.right;
            heroList.Add(hero);

            hero.StartAttack();
        }

        public Character GetAttackTargetHero(Character attacker)
        {
            Vector3 attackerPos = attacker.transform.position;
            float distance = attacker.Range;

            // 사망하지 않고 유효한 캐릭터를 찾아낸다.
            List<Character> list = heroList.FindAll(a => a && !a.IsDead);

            if (attacker.MonsterType == E_MonsterType.Zombie) // 좀비라면 전위 배치 캐릭터부터 공격한다.
            {
                var forwardList = list.FindAll(a => a.IsForward);

                if (forwardList.Count > 0)
                    list = forwardList;
            }
            else // 군인이라면 후위 배치 캐릭터부터 공격한다.
            {
                var backwardList = list.FindAll(a => !a.IsForward);

                if (backwardList.Count > 0)
                    list = backwardList;
            }

            var random = new System.Random();
            return list.OrderBy(x => random.Next()).FirstOrDefault();
        }

        public Character GetHealTargetHero()
        {
            // 사망하지 않고 유효한 캐릭터를 찾아낸다.
            List<Character> list = heroList.FindAll(a => a && !a.IsDead);

            return list.OrderByDescending(a => a.MaxHp - a.CurrentHp).FirstOrDefault();
        }
    }
}

