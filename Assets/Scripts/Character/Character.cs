using R3;
using UnityEngine;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using System;

namespace ND.Character
{
    using Data;
    using System.Threading;

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

        protected ReactiveProperty<float> currentHpRxProp = new ReactiveProperty<float>();
        public float CurrentHp
        {
            get { return currentHpRxProp.Value; }
            set { currentHpRxProp.Value = value; }
        }

        protected ReactiveProperty<float> maxHpRxProp = new ReactiveProperty<float>();
        public float MaxHp
        {
            get { return maxHpRxProp.Value; }
            set { maxHpRxProp.Value = value; }
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

        protected ReactiveProperty<float> rangeRxProp = new ReactiveProperty<float>();
        public float Range
        {
            get { return rangeRxProp.Value; }
            set { rangeRxProp.Value = value; }
        }

        #endregion

        #region Property
        public E_HeroType HeroType { get; private set; } = E_HeroType.NotHero;
        public E_MonsterType MonsterType { get; private set; } = E_MonsterType.NotMonster;
        public bool IsHero { get { return HeroType != E_HeroType.NotHero; } }
        public bool IsDead { get; private set; } = false;
        public bool IsAttacking { get; protected set; } = false;
        public Character Target { get; protected set; }
        public string AttackAnimName { get; protected set; }

        CompositeDisposable disposables = new CompositeDisposable();
        #endregion

        #region Component

        [SerializeField] protected Animator anim;

        #endregion

        private void Awake()
        {
            anim = GetComponent<Animator>();

            stateRxProp.Subscribe(state =>
            {
                if (!IsDead)
                {
                    SetAnimByState(state);
                    IsDead = state == E_CharacterState.Death ? true : false;
                }
            }).AddTo(disposables);
        }

        public void InitCharacter(HeroData data)
        {
            Name = data.name;
            Speed = data.defaultSpeed;
            CurrentHp = data.defaultHp;
            MaxHp = data.defaultHp;
            Atk = data.defaultAtk;
            Armor = data.defaultArmor;
            Dps = data.defaultDps;
            Range = data.defaultRange;
            HeroType = data.type;
            State = E_CharacterState.Idle;

            anim.SetBool("IsMale", HeroType != E_HeroType.Girl);
            anim.SetFloat("HeroType", (int)HeroType);

            AttackAnimName = anim.runtimeAnimatorController.animationClips[(int)HeroType + 5].name;

            transform.position = new Vector3(-10, 0, 0);
            transform.rotation = Quaternion.Euler(Vector3.right);
        }

        public void InitCharacter(MonsterData data)
        {
            Name = data.name;
            Speed = data.defaultSpeed;
            CurrentHp = data.defaultHp;
            MaxHp = data.defaultHp;
            Atk = data.defaultAtk;
            Armor = data.defaultArmor;
            Dps = data.defaultDps;
            Range = data.defaultRange;
            MonsterType = data.type;
            State = E_CharacterState.Idle;

            transform.position = new Vector3(10, 0, 0);
            transform.rotation = Quaternion.Euler(Vector3.left);
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

        public async UniTask MoveToPos(Vector3 pos)
        {
            State = E_CharacterState.Run;

            Vector3 dir = pos - transform.position;
            transform.forward = dir.normalized;

            transform.DOMove(pos, 3).OnComplete(() => 
            {
                State = E_CharacterState.Idle;
            });

            await UniTask.Delay(TimeSpan.FromSeconds(3));
        }

        public void StartAttack()
        {
            //Observable.Ev
            //
            //Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(async _ =>
            //{
            //    await AttackToTarget();
            //}).AddTo(disposables);

            WaitingAttack();
        }

        protected virtual async UniTask AttackToTarget()
        {
            


        }

        async void WaitingAttack()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Dps));

            await AttackToTarget();

            WaitingAttack();
        }

        public async UniTask DamageToCharacter(float damage = 0)
        {
            await UniTask.CompletedTask;
        }

        private void OnDestroy()
        {
            stateRxProp.Dispose();
        }

        protected async UniTask WaitingAnimTime()
        { 
            
        }

        public virtual void OnAttackHit()
        { 
        
        }

        public virtual void OnAttackEnd()
        { 
            
        }
    }
}
