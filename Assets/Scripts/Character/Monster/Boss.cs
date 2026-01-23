using Cysharp.Threading.Tasks;
using DG.Tweening;
using R3;
using UnityEngine;

namespace ND.Character.Monster
{
    public class Boss : Soldier
    {
        protected override async UniTask AttackToTarget()
        {
            if (Target == null || Target.IsDead)
            {
                Target = heroManager.GetAttackTargetHero(this);
            }

            if (Target != null && !IsDead)
            {
                float distance = Vector3.Distance(Target.transform.position, transform.position);

                if (distance <= Range)
                {
                    Vector3 leftSide = Vector3.left + Vector3.forward;
                    Vector3 rightSide = Vector3.left + Vector3.back;

                    isAttackFinish = false;
                    State = E_CharacterState.Idle;

                    Sequence sequence = DOTween.Sequence();
                    sequence.Append(transform.DORotate(GetLookRotationAngle(leftSide), 0.5f)) // 전체 공격 준비
                              .AppendCallback(() =>
                              {
                                  State = E_CharacterState.Attack;
                                  _ = particleManager.SpawnParticle("BossAttack", new Vector3(0.3f, 0.7f, 0.8f), 3, transform);
                              }) 
                              .Append(transform.DORotate(GetLookRotationAngle(rightSide), 1.0f)) // 1차 공격 시작
                              .AppendCallback(() => OnBossAttack())
                              .Append(transform.DORotate(GetLookRotationAngle(leftSide), 1.0f)) // 2차 공격 시작
                              .AppendCallback(() => OnBossAttack())
                              .Append(transform.DORotate(GetLookRotationAngle(rightSide), 1.0f)) // 3차 공격 시작
                              .Append(transform.DORotate(GetLookRotationAngle(Vector3.left), 0.5f))
                              .OnComplete(() =>
                              {
                                  OnBossAttack();
                                  isAttackFinish = true;
                              });

                    await UniTask.WaitUntil(() => isAttackFinish);
                    await WaitingAttackAnimTime();

                    State = E_CharacterState.Idle;                }
            }
        }

        public async override void OnAnimAttack()
        {
            await UniTask.CompletedTask;
        }

        public void OnBossAttack()
        {
            var list = heroManager.GetBossAttackTargetList();

            if (list != null)
            {
                list.ForEach(a =>
                {
                    if (a && !a.IsDead)
                    {
                        _ = a.DamageToCharacter(Atk);
                    }
                });
            }
        }
    }
}
