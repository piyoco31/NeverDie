using UnityEngine;

namespace ND.Manager
{
    using Data;
    using UnityEngine.PlayerLoop;

    public class HeroManager : MonoBehaviour
    {
        public HeroDataSO dataSO;

        public void Init()
        {
            foreach (var data in dataSO.heroDataList)
            {
                Instantiate(null, Vector3.zero, Quaternion.identity);
                // data.
            }
        }
    }
}

