using System;
using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using VContainer;

namespace ND.Character
{
    using Data;
    using Manager;
    using UI;

    public class Character : MonoBehaviour
    {
        [Inject] protected UserDataManager userDataManager;
        [Inject] protected MonsterManager monsterManager;
        [Inject] protected HeroManager heroManager;
        [Inject] protected ParticleManager particleManager;

        #region RxProp
        protected ReactiveProperty<E_CharacterState> stateRxProp = new ReactiveProperty<E_CharacterState>(E_CharacterState.Idle);
        public ReactiveProperty<E_CharacterState> StateRxProp { get { return stateRxProp; } }
        public E_CharacterState State
        {
            get { return stateRxProp.Value; }
            set { stateRxProp.Value = value; }
        }

        protected ReactiveProperty<string> nameRxProp = new ReactiveProperty<string>();
        public ReactiveProperty<string> NameRxProp { get { return nameRxProp; } }
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
        public ReactiveProperty<float> CurrentHpRxProp { get { return currentHpRxProp; } }
        public float CurrentHp
        {
            get { return currentHpRxProp.Value; }
            set { currentHpRxProp.Value = value; }
        }

        protected ReactiveProperty<float> maxHpRxProp = new ReactiveProperty<float>();
        public ReactiveProperty<float> MaxHpRxProp { get { return maxHpRxProp; } }
        public float MaxHp
        {
            get { return maxHpRxProp.Value; }
            set { maxHpRxProp.Value = value; }
        }

        protected ReactiveProperty<float> atkRxProp = new ReactiveProperty<float>();
        public ReactiveProperty<float> AtkRxProp { get { return atkRxProp; } }
        public float Atk
        {
            get { return atkRxProp.Value; }
            set { atkRxProp.Value = value; }
        }

        protected ReactiveProperty<float> armorRxProp = new ReactiveProperty<float>();
        public ReactiveProperty<float> ArmorRxProp { get { return armorRxProp; } }
        public float Armor
        {
            get { return armorRxProp.Value; }
            set { armorRxProp.Value = value; }
        }

        protected ReactiveProperty<float> dpsRxProp = new ReactiveProperty<float>();
        public ReactiveProperty<float> DpsRxProp { get { return dpsRxProp; } }
        public float Dps
        {
            get { return dpsRxProp.Value; }
            set { dpsRxProp.Value = value; }
        }

        protected ReactiveProperty<float> rangeRxProp = new ReactiveProperty<float>();
        public ReactiveProperty<float> RangeRxProp { get { return rangeRxProp; } }
        public float Range
        {
            get { return rangeRxProp.Value; }
            set { rangeRxProp.Value = value; }
        }

        protected ReactiveProperty<bool> isCoolTimeRxProp = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsCoolTimeRxProp { get { return isCoolTimeRxProp; } }
        public bool IsCoolTime
        {
            get { return isCoolTimeRxProp.Value; }
            private set { isCoolTimeRxProp.Value = value; }
        }

        protected ReactiveProperty<bool> isDeadRxProp = new ReactiveProperty<bool>();
        public ReactiveProperty<bool> IsDeadRxProp { get { return isDeadRxProp; } }
        public bool IsDead
        {
            get { return isDeadRxProp.Value; }
            private set { isDeadRxProp.Value = value; }
        }

        #endregion

        #region Property
        public E_HeroType HeroType { get; private set; } = E_HeroType.NotHero;
        public E_MonsterType MonsterType { get; private set; } = E_MonsterType.NotMonster;
        public Character Target { get; protected set; }
        public int PosIdx { get; private set; }
        public bool IsForward { get { return PosIdx < 3; } }
        public int RewardMoney { get; private set; }

        protected string attackAnimName;
        protected CompositeDisposable disposables = new CompositeDisposable();
        protected bool isAttackFinish = true;
        #endregion

        #region Component
        [SerializeField] protected Animator anim;
        [SerializeField] protected MonsterHpBarUI monsterHpBarUI;
        #endregion

        private void Awake()
        {
            anim = GetComponent<Animator>();
            monsterHpBarUI = GetComponentInChildren<MonsterHpBarUI>();
        }

        private void OnDestroy()
        {
            transform.DOKill();
            disposables.Dispose();
        }

        public async UniTask InitCharacter(HeroData data, int idx = 0)
        {
            PosIdx = idx;
            IsDead = false;
            isAttackFinish = true;

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

            await SubscribeRxProp();
        }

        public async UniTask InitCharacter(MonsterData data)
        {
            IsDead = false;
            isAttackFinish = true;

            Name = data.name;
            Speed = data.defaultSpeed;
            CurrentHp = data.defaultHp;
            MaxHp = data.defaultHp;
            Atk = data.defaultAtk;
            Armor = data.defaultArmor;
            Dps = data.defaultDps;
            Range = data.defaultRange;
            MonsterType = data.type;
            RewardMoney = data.rewardMoney;
            HeroType = E_HeroType.NotHero;
            State = E_CharacterState.Idle;

            bool IsZombie = MonsterType == E_MonsterType.Zombie;

            anim.SetFloat("MonsterType", IsZombie ? 0 : 1);

            attackAnimName = anim.runtimeAnimatorController.animationClips[IsZombie ? 4 : 5].name;

            transform.position = new Vector3(10, 0, 0);
            transform.rotation = Quaternion.Euler(Vector3.left);

            monsterHpBarUI.Init(this);

            await SubscribeRxProp();
        }

        public async UniTask ReSpawnCharacter()
        {
            IsDead = false;
            CurrentHp = MaxHp;
            isAttackFinish = true;
            State = E_CharacterState.Idle;

            transform.rotation = Quaternion.Euler(Vector3.right);

            await SubscribeRxProp();
        }

        async UniTask SubscribeRxProp()
        {
            stateRxProp.Subscribe(state =>
            {
                if (!IsDead)
                {
                    SetAnimByState(state);
                    IsDead = state == E_CharacterState.Death ? true : false;

                    if (IsDead)
                    {
                        if (HeroType == E_HeroType.NotHero)
                        {
                            _ = monsterManager.DeSpawnMonster(this);
                        }
                        else
                        {
                            heroManager.CheckGameOver();
                        }

                        transform.DOKill();
                        disposables.Clear();
                    }
                }
            }).AddTo(disposables);

            await UniTask.CompletedTask;
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
            IsCoolTime = true;
            await UniTask.Delay(TimeSpan.FromSeconds(Dps));
            IsCoolTime = false;

            await AttackToTarget();

            if (!IsDead)
                WaitingAttack();
        }

        public async virtual void OnAnimAttack()
        {
            if (Target)
            {
                _ = Target.DamageToCharacter(Atk);
            }

            isAttackFinish = true;

            await UniTask.CompletedTask;
        }

        public async virtual void OnAnimAttackParticle()
        {
            await UniTask.CompletedTask;
        }

        public async UniTask DamageToCharacter(float damage = 0)
        {
            float AddDamage = damage;

            // 이 외에 조건은 체력 회복임
            if (damage > 0)
                AddDamage = Mathf.Clamp(damage - Armor, 1, damage);

            await particleManager.SpawnDamageText(transform, AddDamage);

            CurrentHp = Mathf.Clamp(CurrentHp - AddDamage, 0, MaxHp);

            if (monsterHpBarUI)
                monsterHpBarUI.ShowTime = 3.0f;

            if (CurrentHp <= 0)
                State = E_CharacterState.Death;
        }

        protected async UniTask WaitingAttackAnimTime()
        {
            await UniTask.WaitUntil(() => IsSameAnim());

            float waitTime = anim.GetCurrentAnimatorClipInfo(0)[0].clip.length;

            await UniTask.Delay(TimeSpan.FromSeconds(waitTime));
        }

        public void AddStat(E_HeroStatType type, float value)
        {
            switch (type)
            {
                case E_HeroStatType.Atk:
                    Atk = Mathf.Clamp(Atk += value, 1, float.MaxValue);
                    break;
                case E_HeroStatType.MaxHp:
                    MaxHp = Mathf.Clamp(MaxHp += value, 1, float.MaxValue);
                    break;
                case E_HeroStatType.CurrentHp:
                    if (!IsDead)
                        CurrentHp = Mathf.Clamp(CurrentHp += value, 0, MaxHp);
                    break;
                case E_HeroStatType.Armor:
                    Armor = Mathf.Clamp(Armor += value, 0, float.MaxValue);
                    break;
                case E_HeroStatType.Dps:
                    Dps = Mathf.Clamp(Dps -= value, 0.5f, float.MaxValue);
                    break;
                case E_HeroStatType.Range:
                    Range = Mathf.Clamp(Range += value, 0.5f, 10.0f);
                    break;
            }
        }

        bool IsSameAnim()
        {
            if (anim && anim.GetCurrentAnimatorClipInfo(0).Count() > 0)
            {
                return attackAnimName == anim?.GetCurrentAnimatorClipInfo(0)[0].clip.name;
            }

            return false;
        }

        protected Vector3 GetLookRotationAngle(Vector3 dir)
        {
            return Quaternion.LookRotation(dir, Vector3.up).eulerAngles;
        }
    }
}
