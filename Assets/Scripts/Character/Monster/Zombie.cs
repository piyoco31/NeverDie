using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;
using VContainer;

namespace ND.Character.Monster
{
    using Manager;

    public class Zombie : Character
    {
        public override void StartAttack()
        {
            base.StartAttack();

            Observable.EveryUpdate().Subscribe(x => 
            {
                if (Target == null || Target.IsDead)
                {
                    Target = heroManager.GetAttackTargetHero(this);
                }

                if (Target)
                {
                    float distance = Vector3.Distance(Target.transform.position, transform.position);

                    if (distance > Range)
                    {
                        Vector3 dir = Target.transform.position - transform.position;
                        dir.Normalize();
                        transform.position += dir * Speed * Time.deltaTime;
                        transform.forward = dir;

                        if (State != E_CharacterState.Run)
                            State = E_CharacterState.Run;
                    }
                    else if (State == E_CharacterState.Run)
                        State = E_CharacterState.Idle;
                }
            }).AddTo(disposables);
        }

        protected override async UniTask AttackToTarget()
        {
            if (Target == null || Target.IsDead)
            {
                Target = heroManager.GetAttackTargetHero(this);
            }

            if (Target != null)
            {
                float distance = Vector3.Distance(Target.transform.position, transform.position);

                if (distance <= Range)
                {
                    Vector3 dir = Target.transform.position - transform.position;

                    Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);

                    transform.DORotate(targetRotation.eulerAngles, 0.5f).OnComplete(() =>
                    {
                        State = E_CharacterState.Attack;
                    });

                    await WaitingAttackAnimTime();

                    if (Target)
                        await Target.DamageToCharacter(Atk);

                    State = E_CharacterState.Idle;
                    targetRotation = Quaternion.LookRotation(Vector3.left, Vector3.up);
                    transform.DORotate(targetRotation.eulerAngles, 0.5f);
                }
            }
        }
    }
}
