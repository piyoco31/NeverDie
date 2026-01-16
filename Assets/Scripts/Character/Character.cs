using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using VContainer;

namespace ND.Character
{
    using Data;
    using Manager;

    public class Character : MonoBehaviour
    {
        [Inject] UserDataManager userDataManager;

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
        public bool IsDead { get; private set; } = false;
        public Character Target { get; protected set; }
        public int PosIdx { get; private set; }
        public bool IsForward { get { return PosIdx < 3; } }


        protected string attackAnimName;
        protected CompositeDisposable disposables = new CompositeDisposable();
        protected int rewardMoney = 0;
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

                    if (IsDead)
                    {
                        if (HeroType == E_HeroType.NotHero)
                            userDataManager.Money += rewardMoney;

                        disposables.Dispose();
                    }
                }
            }).AddTo(disposables);
        }

        private void OnDestroy()
        {
            stateRxProp.Dispose();
            disposables.Dispose();
        }

        public void InitCharacter(HeroData data, int idx = 0)
        {
            PosIdx = idx;
            IsDead = false;

            Name = data.name;
            Speed = data.defaultSpeed;
            CurrentHp = data.defaultHp;
            MaxHp = data.defaultHp;
            Atk = data.defaultAtk;
            Armor = data.defaultArmor;
            Dps = data.defaultDps;
            Range = data.defaultRange;
            HeroType = data.type;
            MonsterType = E_MonsterType.NotMonster;
            State = E_CharacterState.Idle;

            anim.SetBool("IsMale", HeroType != E_HeroType.Girl);
            anim.SetFloat("HeroType", (int)HeroType);

            attackAnimName = anim.runtimeAnimatorController.animationClips[(int)HeroType + 5].name;

            transform.position = new Vector3(-10, 0, 0);
            transform.rotation = Quaternion.Euler(Vector3.right);
        }

        public void InitCharacter(MonsterData data)
        {
            IsDead = false;

            Name = data.name;
            Speed = data.defaultSpeed;
            CurrentHp = data.defaultHp;
            MaxHp = data.defaultHp;
            Atk = data.defaultAtk;
            Armor = data.defaultArmor;
            Dps = data.defaultDps;
            Range = data.defaultRange;
            MonsterType = data.type;
            rewardMoney = data.rewardMoney;
            HeroType = E_HeroType.NotHero;
            State = E_CharacterState.Idle;
            
            bool IsZombie = MonsterType == E_MonsterType.Zombie;

            anim.SetFloat("MonsterType", IsZombie ? 0 : 1);

            attackAnimName = anim.runtimeAnimatorController.animationClips[IsZombie ? 4 : 5].name;

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

            transform.DOMove(pos, Speed).OnComplete(() => 
            {
                State = E_CharacterState.Idle;
            });

            await UniTask.Delay(TimeSpan.FromSeconds(3));
        }

        public virtual void StartAttack()
        {
            WaitingAttack();
        }

        protected virtual async UniTask AttackToTarget()
        {
            await UniTask.CompletedTask;
        }

        async void WaitingAttack()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(Dps));

            await AttackToTarget();

            if (!IsDead)
                WaitingAttack();
        }

        public async UniTask DamageToCharacter(float damage = 0)
        {
            float AddDamage = damage;

            // 이 외에 조건은 체력 회복임
            if (damage > 0)
                AddDamage = Mathf.Clamp(damage - Armor, 1, damage);

            CurrentHp = Mathf.Clamp(CurrentHp - AddDamage, 0, MaxHp);

            if (CurrentHp <= 0)
                State = E_CharacterState.Death;

            await UniTask.CompletedTask;
        }

        protected async UniTask WaitingAttackAnimTime()
        {
            await UniTask.WaitUntil(() => attackAnimName == anim?.GetCurrentAnimatorClipInfo(0)[0].clip.name);

            float waitTime = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;

            await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        }
    }
}
