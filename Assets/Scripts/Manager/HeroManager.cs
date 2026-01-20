using System.Linq;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;
using R3;

namespace ND.Manager
{
    using Character;
    using Data;
    using Character.Hero;

    public class HeroManager : MonoBehaviour
    {
        [Inject] IObjectResolver container;

        ReactiveProperty<int> spawnCountRxProp = new(0);
        public ReactiveProperty<int> SpawnCountRxProp { get { return spawnCountRxProp; } }
        public int SpawnCount { get { return spawnCountRxProp.Value; } private set { spawnCountRxProp.Value = value; } }

        public List<Character> HeroList { get { return heroList; } }

        List<Character> heroList = new();
        Dictionary<E_HeroType, Pool<Character>> poolDic = new();
        Dictionary<E_HeroType, HeroData> dataDic = new();
        Dictionary<int, HeroPositionData> positionDataDic = new();

        Transform poolParentTr;
        HeroDataSO heroDataSO;

        public async UniTask Init(MainGameManager mainGameManager)
        {
            GameObject trObj = new();
            trObj.name = "HeroPool";
            poolParentTr = trObj.transform;

            var bgObj = await Addressables.LoadAssetAsync<GameObject>("ND_BG_Town");

            Instantiate(bgObj);

            heroDataSO = await Addressables.LoadAssetAsync<HeroDataSO>("ND_SO_HeroDataAsset");

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

            var positionDataSO = await Addressables.LoadAssetAsync<HeroPositionDataSO>("ND_SO_HeroPositionDataAsset");

            positionDataDic = positionDataSO.heroPositionDataList.ToDictionary(keySelector: m => m.idx, elementSelector: m => m);
        }

        public async UniTask<Character> SpawmHero(E_HeroType type, int idx = 0)
        {
            SpawnCount++;
            var hero = poolDic[type].Get();
            container.Inject(hero);
            hero.InitCharacter(dataDic[type], idx);
            await hero.MoveToPos(new Vector3(positionDataDic[idx].posX, positionDataDic[idx].posY, positionDataDic[idx].posZ));

            hero.transform.forward = Vector3.right;
            heroList.Add(hero);

            hero.StartAttack();

            return hero;
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

        public void UpgradeHero(UpgradeData data)
        {
            var targetHero = data.upgradeTarget switch
            {
                E_UpgradeTargetType.AttackerOnly => heroList.FindAll(x => x is Attacker),
                E_UpgradeTargetType.RangerOnly => heroList.FindAll(x => x is Ranger),
                E_UpgradeTargetType.HealerOnly => heroList.FindAll(x => x is Supporter),
                E_UpgradeTargetType.ForwardOnly => heroList.FindAll(x => x.IsForward),
                E_UpgradeTargetType.BackWardOnly => heroList.FindAll(x => !x.IsForward),
                _ => heroList
            };

            targetHero.ForEach(x => 
            {
                if (data.statType == E_HeroStatType.All)
                {
                    for (E_HeroStatType i = E_HeroStatType.Atk; i < E_HeroStatType.All; i++)
                    {
                        x.AddStat(i, data.upgradeValue);
                    }
                }
                else
                    x.AddStat(data.statType, data.upgradeValue);
            });
        }
    }
}

