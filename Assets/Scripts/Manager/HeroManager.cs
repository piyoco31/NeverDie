using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace ND.Manager
{
    using Character.Hero;
    using Data;

    public class HeroManager : MonoBehaviour
    {
        public HeroDataSO dataSO;

        public void Start()
        {
            Init();
        }

        public async void Init()
        {
            dataSO = await Addressables.LoadAssetAsync<HeroDataSO>("HeroDataAsset");

            foreach (var data in dataSO.heroDataList)
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
            }
        }
    }
}

