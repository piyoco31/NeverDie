using UnityEngine;
using System.Collections.Generic;

namespace ND.Data
{
    [CreateAssetMenu(fileName = "ParticleDataAsset", menuName = "DataAsset/Particle")]
    public class ParticleDataSO : ScriptableObject
    {
        public TextAsset textAsset;
        public List<ParticleData> particleDataList;
    }
}
