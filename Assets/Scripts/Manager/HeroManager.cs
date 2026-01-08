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
   

    public class HeroManager : MonoBehaviour
    {


        [SerializeField] HeroDataSO heroDataSO;
        

        List<Character> heroList;

        public void Start()
        {
        }

        public async UniTask Init()
        {

            heroDataSO = await Addressables.LoadAssetAsync<HeroDataSO>("HeroDataAsset");

            foreach (var data in heroDataSO.heroDataList)
            {
                var obj = await Addressables.LoadAssetAsync<GameObject>(data.prefab);
                var character = Instantiate(obj, Vector3.zero, Quaternion.identity);

                if (data.type == E_HeroType.Doctor)
                {
                    var hero = character.AddComponent<Surportter>();
                    hero.InitCharacter(data);
                    
                }
                else if (data.type == E_HeroType.Police || data.type == E_HeroType.Soldier)
                {
                    var hero = character.AddComponent<Ranger>();
                    hero.InitCharacter(data);
                }
                else
                {
                    var hero = character.AddComponent<Attacker>();
                    hero.InitCharacter(data);
                }

                var pool = Pool.Create(character.GetComponent<Character>(), 10, character.transform).NonLazy();

            }
        }
    }
}

