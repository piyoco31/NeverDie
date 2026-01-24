using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace ND.Character.Hero
{
    public class Ranger : Character
    {
        protected override async UniTask AttackToTarget()
        {
            if (Target == null || Target.IsDead || Vector3.Distance(Target.transform.position, transform.position) > Range)
            {
                Target = monsterManager.GetTargetMonster(this);
            }

            if (Target != null && !IsDead)
            {
                Vector3 dir = Target.transform.position - transform.position;

                Quaternion targetRotation = Quaternion.LookRotation(dir, Vector3.up);

                transform.DORotate(targetRotation.eulerAngles, 0.5f).OnComplete(() =>
                {
                    State = E_CharacterState.Attack;
                });

                isAttackFinish = false;
                await UniTask.WaitUntil(() => isAttackFinish);
                await WaitingAttackAnimTime();

                State = E_CharacterState.Idle;
                targetRotation = Quaternion.LookRotation(Vector3.right, Vector3.up);
                transform.DORotate(targetRotation.eulerAngles, 0.5f);
            }
        }

        public async override void OnAnimAttackParticle()
        {
            Vector3 pos = HeroType == E_HeroType.Police ? new Vector3(0.33f, 0.8f, 0.6f) : new Vector3(0.28f, 0.77f, 1.0f);

            await particleManager.SpawnParticle("Shoot", pos, 2.0f, transform);
        }

        public async override void OnAnimAttack()
        {
            base.OnAnimAttack();

            if (Target)
            {
                await particleManager.SpawnParticle("ShootHit", new Vector3(0.0f, 0.6f, 0.0f), 2.0f, Target.transform); ;
            }
        }
    }
}
