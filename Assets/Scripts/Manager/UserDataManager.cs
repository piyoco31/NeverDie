using R3;
using UnityEngine;
using VContainer;

namespace ND.Manager
{
    public class UserDataManager
    {
        private ReactiveProperty<int> moneyRxProp = new ReactiveProperty<int>(0);
        public int Money
        {
            get { return moneyRxProp.Value; }
            set { moneyRxProp.Value = value; }
        }


    }
}


