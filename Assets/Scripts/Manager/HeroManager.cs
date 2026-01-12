using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Redcode.Pools;
using UnityEngine;
using UnityEngine.AddressableAssets;
using VContainer;

namespace ND.Manager
{
    using Character;
    using Character.Hero;
    using Data;
    using DG.Tweening;

    public class HeroManager : MonoBehaviour
    {
        Dictionary<E_HeroType, Pool<Character>> poolDic = new();
        Dictionary<E_HeroType, HeroData> dataDic = new();
        Transform poolParentTr;
        HeroDataSO heroDataSO;
        
        List<Character> heroList = new();
        readonly IObjectResolver container;

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

                /*
                if (type == E_HeroType.Doctor)
                {
                    var hero = obj.GetComponent<Surportter>();
                    hero.InitCharacter(data);
                    
                }
                else if (type == E_HeroType.Police || type == E_HeroType.Soldier)
                {
                    var hero = obj.GetComponent<Ranger>();
                    hero.InitCharacter(data);
                }
                else
                {
                    var hero = obj.GetComponent<Attacker>();
                    hero.InitCharacter(data);
                }
                */

                if (!poolDic.ContainsKey(type))
                {
                    Pool<Character> pool = Pool.Create(obj.GetComponent<Character>(), data.poolCount, poolParentTr).NonLazy();
                    poolDic.Add(type, pool);
                }
            }
        }

        public async UniTask SpawmHero(E_HeroType type)
        {
            var hero = poolDic[type].Get();
            container.Inject(hero);
            hero.InitCharacter(dataDic[type]);
            heroList.Add(hero);

            await hero.MoveToPos(new Vector3(dataDic[type].startPosX, dataDic[type].startPosY, dataDic[type].startPosZ));

            hero.transform.forward = Vector3.right;

            hero.StartAttack();
        }
    }
}

