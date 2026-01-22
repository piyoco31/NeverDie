using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

namespace ND.Manager
{
    public class UserDataManager : MonoBehaviour
    {
        #region PlayerPrefs Key
        static string moneyKey = "ND.UserData.Money";
        static string killKey = "ND.UserData.Kill";
        #endregion

        private ReactiveProperty<int> moneyRxProp = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> MoneyRxProp { get { return moneyRxProp; } }
        public int Money
        {
            get { return moneyRxProp.Value; }
            set { moneyRxProp.Value = value; }
        }

        private ReactiveProperty<int> killRxProp = new ReactiveProperty<int>(0);
        public ReactiveProperty<int> KillRxProp { get { return killRxProp; } }
        public int Kill
        {
            get { return killRxProp.Value; }
            set { killRxProp.Value = value; }
        }

        public async UniTask Init()
        {
            if (PlayerPrefs.HasKey(moneyKey))
            {
                Money = PlayerPrefs.GetInt(moneyKey);
            }

            if (PlayerPrefs.HasKey(killKey))
            {
                Kill = PlayerPrefs.GetInt(killKey);
            }

            await UniTask.CompletedTask;
        }

        void OnApplicationQuit()
        {
            SaveUserData();
        }

        public void SaveUserData()
        {
            PlayerPrefs.SetInt(moneyKey, Money);
            PlayerPrefs.SetInt(killKey, Kill);
            PlayerPrefs.Save();
        }
    }
}


