using R3;
using UnityEngine;

namespace ND.Character
{
    using Data;

    public class Character : MonoBehaviour
    {
        #region RxProp

        protected ReactiveProperty<E_CharacterState> stateRxProp = new ReactiveProperty<E_CharacterState>(E_CharacterState.Idle);
        public E_CharacterState State 
        { 
            get { return stateRxProp.Value; }
            set { stateRxProp.Value = value; }
        }

        protected ReactiveProperty<string> nameRxProp = new ReactiveProperty<string>();
        public string Name
        {
            get { return nameRxProp.Value; }
            set { nameRxProp.Value = value; }
        }

        protected ReactiveProperty<float> speedRxProp = new ReactiveProperty<float>();
        public float Speed
        {
            get { return speedRxProp.Value; }
            set { speedRxProp.Value = value; }
        }

        protected ReactiveProperty<float> hpRxProp = new ReactiveProperty<float>();
        public float Hp
        {
            get { return hpRxProp.Value; }
            set { hpRxProp.Value = value; }
        }

        protected ReactiveProperty<float> atkRxProp = new ReactiveProperty<float>();
        public float Atk
        {
            get { return atkRxProp.Value; }
            set { atkRxProp.Value = value; }
        }

        protected ReactiveProperty<float> armorRxProp = new ReactiveProperty<float>();
        public float Armor
        {
            get { return armorRxProp.Value; }
            set { armorRxProp.Value = value; }
        }

        protected ReactiveProperty<float> dpsRxProp = new ReactiveProperty<float>();
        public float Dps
        {
            get { return dpsRxProp.Value; }
            set { dpsRxProp.Value = value; }
        }

        #endregion

        #region Property
        public E_HeroType HeroType { get; private set; } = E_HeroType.NotHero;
        public E_MonsterType MonsterType { get; private set; } = E_MonsterType.NotMonster;
        public bool IsHero { get { return HeroType != E_HeroType.NotHero; } }
        #endregion

        #region Component

        [SerializeField] protected Animator anim;

        #endregion

        private void Awake()
        {
            anim = GetComponent<Animator>();

            stateRxProp.Subscribe(state =>
            {
                SetAnimByState(state);
            });
        }

        public void InitCharacter(HeroData data)
        {
            Name = data.name;
            Speed = data.defaultSpeed;
            Hp = data.defaultHp;
            Atk = data.defaultAtk;
            Armor = data.defaultArmor;
            Dps = data.defaultDps;
            HeroType = data.type;
            State = E_CharacterState.Idle;
        }

        public void InitCharacter(MonsterData data)
        {
            Name = data.name;
            Speed = data.defaultSpeed;
            Hp = data.defaultHp;
            Atk = data.defaultAtk;
            Armor = data.defaultArmor;
            Dps = data.defaultDps;
            MonsterType = data.type;
            State = E_CharacterState.Idle;
        }

        void SetAnimByState(E_CharacterState InState)
        {
            string triggerStr = InState switch
            {
                E_CharacterState.Attack => "Attack",
                E_CharacterState.Death => "Death",
                E_CharacterState.Run => "Run",
                _ => "Idle"
            };

            anim?.SetTrigger(triggerStr);
        }

        private void OnDestroy()
        {
            stateRxProp.Dispose();
        }
    }
}
