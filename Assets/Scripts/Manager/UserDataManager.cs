using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace ND.Manager
{
    public class UserDataManager : MonoBehaviour
    {
        #region PlayerPrefs Key
        static string moneyKey = "ND.UserData.Money";
        #endregion

        private ReactiveProperty<int> moneyRxProp = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> MoneyRxProp { get { return moneyRxProp; } }
        public int Money
        {
            get { return moneyRxProp.Value; }
            set { moneyRxProp.Value = value; }
        }

        public async UniTask Init()
        {
            if (PlayerPrefs.HasKey(moneyKey))
            {
                Money = PlayerPrefs.GetInt(moneyKey);
            }

            await UniTask.CompletedTask;
        }

        void OnApplicationQuit()
        {
            PlayerPrefs.SetInt(moneyKey, Money);
            PlayerPrefs.Save();
        }
    }
}


